using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly string _connectionString;

        public PackageRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<IEnumerable<PackageResponse>> GetAllPackagesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT 
                    p.*,
                    COALESCE(ps.purchase_count, 0) as TotalPurchases,
                    COALESCE(ps.total_revenue, 0) as TotalRevenue,
                    COALESCE(pr.avg_rating, 0) as AverageRating,
                    COALESCE(pr.review_count, 0) as ReviewCount
                FROM packages p
                LEFT JOIN (
                    SELECT 
                        package_id,
                        COUNT(*) as purchase_count,
                        SUM(amount_paid) as total_revenue
                    FROM package_purchases 
                    GROUP BY package_id
                ) ps ON p.id = ps.package_id
                LEFT JOIN (
                    SELECT 
                        package_id,
                        AVG(rating) as avg_rating,
                        COUNT(*) as review_count
                    FROM package_reviews 
                    GROUP BY package_id
                ) pr ON p.id = pr.package_id
                ORDER BY p.created_at DESC";

            var packages = await connection.QueryAsync<PackageModel>(sql);
            
            var result = new List<PackageResponse>();
            foreach (var package in packages)
            {
                var services = await GetPackageServicesAsync(package.Id);
                var packageResponse = MapToPackageResponse(package, services);
                result.Add(packageResponse);
            }
            
            return result;
        }

        public async Task<PackageDetailedModel?> GetPackageByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT 
                    p.*,
                    COALESCE(ps.purchase_count, 0) as TotalPurchases,
                    COALESCE(ps.total_revenue, 0) as TotalRevenue,
                    COALESCE(pr.avg_rating, 0) as AverageRating,
                    COALESCE(pr.review_count, 0) as ReviewCount
                FROM packages p
                LEFT JOIN (
                    SELECT 
                        package_id,
                        COUNT(*) as purchase_count,
                        SUM(amount_paid) as total_revenue
                    FROM package_purchases 
                    WHERE package_id = @Id
                    GROUP BY package_id
                ) ps ON p.id = ps.package_id
                LEFT JOIN (
                    SELECT 
                        package_id,
                        AVG(rating) as avg_rating,
                        COUNT(*) as review_count
                    FROM package_reviews 
                    WHERE package_id = @Id
                    GROUP BY package_id
                ) pr ON p.id = pr.package_id
                WHERE p.id = @Id";

            var package = await connection.QueryFirstOrDefaultAsync<PackageDetailedModel>(sql, new { Id = id });
            if (package == null) return null;

            package.Services = (await GetPackageServicesAsync(id)).ToList();
            return package;
        }

        public async Task<PackageResponse> CreatePackageAsync(CreatePackageRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();

            try
            {
                const string insertSql = @"
                    INSERT INTO packages (
                        name, description, category, original_price, discount_percentage, 
                        final_price, duration_days, validity_days, is_active, service_ids, 
                        image_url, terms, max_usages, is_popular, created_at, updated_at
                    ) VALUES (
                        @Name, @Description, @Category, @OriginalPrice, @DiscountPercentage,
                        @FinalPrice, @DurationDays, @ValidityDays, @IsActive, @ServiceIds,
                        @ImageUrl, @Terms, @MaxUsages, @IsPopular, @CreatedAt, @UpdatedAt
                    ) RETURNING *";

                var finalPrice = request.OriginalPrice * (1 - request.DiscountPercentage / 100);
                var now = DateTime.UtcNow;

                var newPackage = await connection.QueryFirstAsync<PackageModel>(insertSql, new
                {
                    request.Name,
                    request.Description,
                    request.Category,
                    request.OriginalPrice,
                    request.DiscountPercentage,
                    FinalPrice = finalPrice,
                    request.DurationDays,
                    request.ValidityDays,
                    request.IsActive,
                    ServiceIds = request.ServiceIds.ToArray(),
                    request.ImageUrl,
                    request.Terms,
                    request.MaxUsages,
                    request.IsPopular,
                    CreatedAt = now,
                    UpdatedAt = now
                }, transaction);

                // Insert package-service relationships
                if (request.ServiceIds.Any())
                {
                    const string relationSql = @"
                        INSERT INTO package_services (package_id, service_id) 
                        VALUES (@PackageId, @ServiceId)";

                    foreach (var serviceId in request.ServiceIds)
                    {
                        await connection.ExecuteAsync(relationSql, new { PackageId = newPackage.Id, ServiceId = serviceId }, transaction);
                    }
                }

                transaction.Commit();

                var services = await GetPackageServicesAsync(newPackage.Id);
                return MapToPackageResponse(newPackage, services);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<PackageResponse?> UpdatePackageAsync(int id, UpdatePackageRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();

            try
            {
                const string updateSql = @"
                    UPDATE packages SET 
                        name = @Name,
                        description = @Description,
                        category = @Category,
                        original_price = @OriginalPrice,
                        discount_percentage = @DiscountPercentage,
                        final_price = @FinalPrice,
                        duration_days = @DurationDays,
                        validity_days = @ValidityDays,
                        is_active = @IsActive,
                        service_ids = @ServiceIds,
                        image_url = @ImageUrl,
                        terms = @Terms,
                        max_usages = @MaxUsages,
                        is_popular = @IsPopular,
                        updated_at = @UpdatedAt
                    WHERE id = @Id 
                    RETURNING *";

                var finalPrice = request.OriginalPrice * (1 - request.DiscountPercentage / 100);

                var updatedPackage = await connection.QueryFirstOrDefaultAsync<PackageModel>(updateSql, new
                {
                    Id = id,
                    request.Name,
                    request.Description,
                    request.Category,
                    request.OriginalPrice,
                    request.DiscountPercentage,
                    FinalPrice = finalPrice,
                    request.DurationDays,
                    request.ValidityDays,
                    request.IsActive,
                    ServiceIds = request.ServiceIds.ToArray(),
                    request.ImageUrl,
                    request.Terms,
                    request.MaxUsages,
                    request.IsPopular,
                    UpdatedAt = DateTime.UtcNow
                }, transaction);

                if (updatedPackage == null) return null;

                // Update package-service relationships
                await connection.ExecuteAsync("DELETE FROM package_services WHERE package_id = @PackageId", new { PackageId = id }, transaction);
                
                if (request.ServiceIds.Any())
                {
                    const string relationSql = @"
                        INSERT INTO package_services (package_id, service_id) 
                        VALUES (@PackageId, @ServiceId)";

                    foreach (var serviceId in request.ServiceIds)
                    {
                        await connection.ExecuteAsync(relationSql, new { PackageId = id, ServiceId = serviceId }, transaction);
                    }
                }

                transaction.Commit();

                var services = await GetPackageServicesAsync(id);
                return MapToPackageResponse(updatedPackage, services);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeletePackageAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync("DELETE FROM package_services WHERE package_id = @Id", new { Id = id }, transaction);
                var result = await connection.ExecuteAsync("DELETE FROM packages WHERE id = @Id", new { Id = id }, transaction);
                
                transaction.Commit();
                return result > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<PackageStatsResponse> GetPackageStatsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            const string statsSql = @"
                SELECT 
                    COUNT(*) as TotalPackages,
                    COUNT(CASE WHEN is_active = true THEN 1 END) as ActivePackages,
                    COUNT(CASE WHEN is_active = false THEN 1 END) as InactivePackages,
                    COALESCE(SUM(CASE WHEN is_active = true THEN final_price END), 0) as TotalValue,
                    COALESCE(AVG(CASE WHEN is_active = true THEN final_price END), 0) as AveragePackagePrice,
                    COALESCE(AVG(CASE WHEN is_active = true THEN discount_percentage END), 0) as AverageDiscount
                FROM packages";

            var stats = await connection.QueryFirstAsync<PackageStatsResponse>(statsSql);

            // Get category breakdown
            const string categorySql = @"
                SELECT 
                    category as Category,
                    COUNT(*) as PackageCount,
                    COALESCE(SUM(final_price), 0) as TotalRevenue,
                    COALESCE(AVG(final_price), 0) as AveragePrice
                FROM packages 
                WHERE is_active = true
                GROUP BY category";

            stats.CategoryBreakdown = (await connection.QueryAsync<CategoryPackageStats>(categorySql)).ToList();

            // Get most popular packages
            const string popularSql = @"
                SELECT 
                    p.id,
                    p.name,
                    COALESCE(pp.purchase_count, 0) as PurchaseCount,
                    COALESCE(pp.total_revenue, 0) as Revenue,
                    COALESCE(pr.avg_rating, 0) as Rating
                FROM packages p
                LEFT JOIN (
                    SELECT package_id, COUNT(*) as purchase_count, SUM(amount_paid) as total_revenue
                    FROM package_purchases GROUP BY package_id
                ) pp ON p.id = pp.package_id
                LEFT JOIN (
                    SELECT package_id, AVG(rating) as avg_rating
                    FROM package_reviews GROUP BY package_id
                ) pr ON p.id = pr.package_id
                WHERE p.is_active = true
                ORDER BY pp.purchase_count DESC, pr.avg_rating DESC
                LIMIT 5";

            stats.MostPopular = (await connection.QueryAsync<PopularPackage>(popularSql)).ToList();

            return stats;
        }

        public async Task<IEnumerable<PackageResponse>> GetPackagesByCategoryAsync(string category)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT * FROM packages 
                WHERE category = @Category AND is_active = true 
                ORDER BY created_at DESC";

            var packages = await connection.QueryAsync<PackageModel>(sql, new { Category = category });
            
            var result = new List<PackageResponse>();
            foreach (var package in packages)
            {
                var services = await GetPackageServicesAsync(package.Id);
                result.Add(MapToPackageResponse(package, services));
            }
            
            return result;
        }

        public async Task<IEnumerable<PackageResponse>> SearchPackagesAsync(string searchTerm)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT * FROM packages 
                WHERE (
                    LOWER(name) LIKE LOWER(@SearchTerm) OR 
                    LOWER(description) LIKE LOWER(@SearchTerm) OR 
                    LOWER(category) LIKE LOWER(@SearchTerm)
                ) AND is_active = true 
                ORDER BY created_at DESC";

            var packages = await connection.QueryAsync<PackageModel>(sql, new { SearchTerm = $"%{searchTerm}%" });
            
            var result = new List<PackageResponse>();
            foreach (var package in packages)
            {
                var services = await GetPackageServicesAsync(package.Id);
                result.Add(MapToPackageResponse(package, services));
            }
            
            return result;
        }

        public async Task<IEnumerable<string>> GetPackageCategoriesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT DISTINCT category 
                FROM packages 
                WHERE is_active = true 
                ORDER BY category";

            return await connection.QueryAsync<string>(sql);
        }

        public async Task<bool> PackageExistsAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = "SELECT COUNT(*) FROM packages WHERE id = @Id";
            var count = await connection.QueryFirstAsync<int>(sql, new { Id = id });
            
            return count > 0;
        }

        public async Task<bool> PackageNameExistsAsync(string name, int? excludeId = null)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            var sql = "SELECT COUNT(*) FROM packages WHERE LOWER(name) = LOWER(@Name)";
            var parameters = new { Name = name };
            
            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Name = name, ExcludeId = excludeId.Value };
            }
            
            var count = await connection.QueryFirstAsync<int>(sql, parameters);
            return count > 0;
        }

        private async Task<IEnumerable<ServiceModel>> GetPackageServicesAsync(int packageId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT s.* 
                FROM services s
                INNER JOIN package_services ps ON s.id = ps.service_id
                WHERE ps.package_id = @PackageId";

            return await connection.QueryAsync<ServiceModel>(sql, new { PackageId = packageId });
        }

        private static PackageResponse MapToPackageResponse(PackageModel package, IEnumerable<ServiceModel> services)
        {
            return new PackageResponse
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Category = package.Category,
                OriginalPrice = package.OriginalPrice,
                DiscountPercentage = package.DiscountPercentage,
                FinalPrice = package.FinalPrice,
                DurationDays = package.DurationDays,
                ValidityDays = package.ValidityDays,
                IsActive = package.IsActive,
                CreatedAt = package.CreatedAt,
                UpdatedAt = package.UpdatedAt,
                Services = services.ToList(),
                ImageUrl = package.ImageUrl,
                Terms = package.Terms,
                MaxUsages = package.MaxUsages,
                IsPopular = package.IsPopular,
                TotalSavings = package.OriginalPrice - package.FinalPrice
            };
        }
    }

    public class ClinicInfoRepository : IClinicInfoRepository
    {
        private readonly string _connectionString;

        public ClinicInfoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<ClinicInfoResponse?> GetClinicInfoAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = "SELECT * FROM clinic_info LIMIT 1";
            var clinicInfo = await connection.QueryFirstOrDefaultAsync<ClinicInfoModel>(sql);

            if (clinicInfo == null)
            {
                return await CreateDefaultClinicInfoAsync();
            }

            return MapToClinicInfoResponse(clinicInfo);
        }

        public async Task<ClinicInfoResponse> UpdateClinicInfoAsync(UpdateClinicInfoRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            var existingInfo = await connection.QueryFirstOrDefaultAsync<ClinicInfoModel>("SELECT * FROM clinic_info LIMIT 1");
            
            if (existingInfo == null)
            {
                // Create new clinic info
                const string insertSql = @"
                    INSERT INTO clinic_info (
                        name, description, address, phone, email, website, logo_url, 
                        opening_hours, specialties, social_media, is_active, created_at, updated_at
                    ) VALUES (
                        @Name, @Description, @Address, @Phone, @Email, @Website, @LogoUrl,
                        @OpeningHours, @Specialties, @SocialMedia, true, @CreatedAt, @UpdatedAt
                    ) RETURNING *";

                var now = DateTime.UtcNow;
                var newInfo = await connection.QueryFirstAsync<ClinicInfoModel>(insertSql, new
                {
                    request.Name,
                    request.Description,
                    request.Address,
                    request.Phone,
                    request.Email,
                    request.Website,
                    request.LogoUrl,
                    request.OpeningHours,
                    request.Specialties,
                    request.SocialMedia,
                    CreatedAt = now,
                    UpdatedAt = now
                });

                return MapToClinicInfoResponse(newInfo);
            }
            else
            {
                // Update existing clinic info
                const string updateSql = @"
                    UPDATE clinic_info SET 
                        name = @Name,
                        description = @Description,
                        address = @Address,
                        phone = @Phone,
                        email = @Email,
                        website = @Website,
                        logo_url = @LogoUrl,
                        opening_hours = @OpeningHours,
                        specialties = @Specialties,
                        social_media = @SocialMedia,
                        updated_at = @UpdatedAt
                    WHERE id = @Id 
                    RETURNING *";

                var updatedInfo = await connection.QueryFirstAsync<ClinicInfoModel>(updateSql, new
                {
                    Id = existingInfo.Id,
                    request.Name,
                    request.Description,
                    request.Address,
                    request.Phone,
                    request.Email,
                    request.Website,
                    request.LogoUrl,
                    request.OpeningHours,
                    request.Specialties,
                    request.SocialMedia,
                    UpdatedAt = DateTime.UtcNow
                });

                return MapToClinicInfoResponse(updatedInfo);
            }
        }

        public async Task<ClinicStatsResponse> GetClinicStatsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            const string sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM clients) as TotalPatients,
                    (SELECT COUNT(*) FROM appointments) as TotalAppointments,
                    (SELECT COALESCE(SUM(amount), 0) FROM financial_transactions WHERE EXTRACT(MONTH FROM created_at) = EXTRACT(MONTH FROM CURRENT_DATE)) as MonthlyRevenue,
                    (SELECT COUNT(*) FROM staff WHERE is_active = true) as ActiveStaff,
                    (SELECT COUNT(*) FROM services WHERE is_active = true) as TotalServices";

            var stats = await connection.QueryFirstAsync<ClinicStatsResponse>(sql);
            
            // Calculate years in operation (using creation date of first record or default)
            var establishedDate = await connection.QueryFirstOrDefaultAsync<DateTime?>("SELECT MIN(created_at) FROM clinic_info");
            stats.EstablishedDate = establishedDate ?? DateTime.UtcNow.AddYears(-1);
            stats.YearsInOperation = DateTime.UtcNow.Year - stats.EstablishedDate.Year;

            return stats;
        }

        public async Task<bool> ClinicInfoExistsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = "SELECT COUNT(*) FROM clinic_info";
            var count = await connection.QueryFirstAsync<int>(sql);
            
            return count > 0;
        }

        public async Task<ClinicInfoResponse> CreateDefaultClinicInfoAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string insertSql = @"
                INSERT INTO clinic_info (
                    name, description, address, phone, email, website, logo_url, 
                    opening_hours, specialties, social_media, is_active, created_at, updated_at
                ) VALUES (
                    'Clínica DentalSpa', 
                    'Clínica odontológica especializada em tratamentos estéticos e funcionais.',
                    'Rua Principal, 123 - Centro', 
                    '(11) 99999-9999', 
                    'contato@dentalspa.com.br', 
                    'https://dentalspa.com.br',
                    '', 
                    'Segunda a Sexta: 8h às 18h, Sábado: 8h às 12h', 
                    'Odontologia Geral, Estética Dental, Ortodontia, Harmonização Facial', 
                    '{}', 
                    true, 
                    @CreatedAt, 
                    @UpdatedAt
                ) RETURNING *";

            var now = DateTime.UtcNow;
            var defaultInfo = await connection.QueryFirstAsync<ClinicInfoModel>(insertSql, new
            {
                CreatedAt = now,
                UpdatedAt = now
            });

            return MapToClinicInfoResponse(defaultInfo);
        }

        private static ClinicInfoResponse MapToClinicInfoResponse(ClinicInfoModel model)
        {
            return new ClinicInfoResponse
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                Website = model.Website,
                LogoUrl = model.LogoUrl,
                OpeningHours = model.OpeningHours,
                Specialties = model.Specialties,
                SocialMedia = model.SocialMedia,
                IsActive = model.IsActive,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }
    }
}