using Dapper;
using Npgsql;
using ClinicApi.Models;
using ClinicApi.Repositories;

namespace ClinicApi.Repositories
{
    public class BeforeAfterRepository : IBeforeAfterRepository
    {
        private readonly string _connectionString;

        public BeforeAfterRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<IEnumerable<BeforeAfterResponse>> GetAllBeforeAfterAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT 
                    ba.*,
                    COALESCE(AVG(r.rating), 0) as Rating,
                    COUNT(r.id) as RatingCount
                FROM before_after ba
                LEFT JOIN before_after_ratings r ON ba.id = r.before_after_id
                GROUP BY ba.id
                ORDER BY ba.created_at DESC";

            var results = await connection.QueryAsync<BeforeAfterModel>(sql);
            
            return results.Select(MapToBeforeAfterResponse);
        }

        public async Task<BeforeAfterModel?> GetBeforeAfterByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT 
                    ba.*,
                    COALESCE(AVG(r.rating), 0) as Rating,
                    COUNT(r.id) as RatingCount
                FROM before_after ba
                LEFT JOIN before_after_ratings r ON ba.id = r.before_after_id
                WHERE ba.id = @Id
                GROUP BY ba.id";

            return await connection.QueryFirstOrDefaultAsync<BeforeAfterModel>(sql, new { Id = id });
        }

        public async Task<BeforeAfterResponse> CreateBeforeAfterAsync(CreateBeforeAfterRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string insertSql = @"
                INSERT INTO before_after (
                    title, description, treatment_type, before_image_url, after_image_url,
                    patient_id, patient_age, patient_gender, treatment_date, dentist_name,
                    treatment_details, is_public, is_active, tags, created_at, updated_at
                ) VALUES (
                    @Title, @Description, @TreatmentType, @BeforeImageUrl, @AfterImageUrl,
                    @PatientId, @PatientAge, @PatientGender, @TreatmentDate, @DentistName,
                    @TreatmentDetails, @IsPublic, @IsActive, @Tags, @CreatedAt, @UpdatedAt
                ) RETURNING *";

            var now = DateTime.UtcNow;
            var newBeforeAfter = await connection.QueryFirstAsync<BeforeAfterModel>(insertSql, new
            {
                request.Title,
                request.Description,
                request.TreatmentType,
                request.BeforeImageUrl,
                request.AfterImageUrl,
                request.PatientId,
                request.PatientAge,
                request.PatientGender,
                request.TreatmentDate,
                request.DentistName,
                request.TreatmentDetails,
                request.IsPublic,
                request.IsActive,
                Tags = request.Tags.ToArray(),
                CreatedAt = now,
                UpdatedAt = now
            });

            return MapToBeforeAfterResponse(newBeforeAfter);
        }

        public async Task<BeforeAfterResponse?> UpdateBeforeAfterAsync(int id, UpdateBeforeAfterRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string updateSql = @"
                UPDATE before_after SET 
                    title = @Title,
                    description = @Description,
                    treatment_type = @TreatmentType,
                    before_image_url = @BeforeImageUrl,
                    after_image_url = @AfterImageUrl,
                    patient_id = @PatientId,
                    patient_age = @PatientAge,
                    patient_gender = @PatientGender,
                    treatment_date = @TreatmentDate,
                    dentist_name = @DentistName,
                    treatment_details = @TreatmentDetails,
                    is_public = @IsPublic,
                    is_active = @IsActive,
                    tags = @Tags,
                    updated_at = @UpdatedAt
                WHERE id = @Id 
                RETURNING *";

            var updatedBeforeAfter = await connection.QueryFirstOrDefaultAsync<BeforeAfterModel>(updateSql, new
            {
                Id = id,
                request.Title,
                request.Description,
                request.TreatmentType,
                request.BeforeImageUrl,
                request.AfterImageUrl,
                request.PatientId,
                request.PatientAge,
                request.PatientGender,
                request.TreatmentDate,
                request.DentistName,
                request.TreatmentDetails,
                request.IsPublic,
                request.IsActive,
                Tags = request.Tags.ToArray(),
                UpdatedAt = DateTime.UtcNow
            });

            return updatedBeforeAfter != null ? MapToBeforeAfterResponse(updatedBeforeAfter) : null;
        }

        public async Task<bool> DeleteBeforeAfterAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync("DELETE FROM before_after_ratings WHERE before_after_id = @Id", new { Id = id }, transaction);
                var result = await connection.ExecuteAsync("DELETE FROM before_after WHERE id = @Id", new { Id = id }, transaction);
                
                transaction.Commit();
                return result > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<BeforeAfterStatsResponse> GetBeforeAfterStatsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            const string statsSql = @"
                SELECT 
                    COUNT(*) as TotalCases,
                    COUNT(CASE WHEN is_public = true THEN 1 END) as PublicCases,
                    COUNT(CASE WHEN is_public = false THEN 1 END) as PrivateCases,
                    COALESCE(SUM(view_count), 0) as TotalViews
                FROM before_after WHERE is_active = true";

            var stats = await connection.QueryFirstAsync<BeforeAfterStatsResponse>(statsSql);

            // Get rating stats
            const string ratingSql = @"
                SELECT 
                    COALESCE(AVG(rating), 0) as AverageRating,
                    COUNT(*) as TotalRatings
                FROM before_after_ratings r
                INNER JOIN before_after ba ON r.before_after_id = ba.id
                WHERE ba.is_active = true";

            var ratingStats = await connection.QueryFirstAsync<dynamic>(ratingSql);
            stats.AverageRating = ratingStats.AverageRating;
            stats.TotalRatings = ratingStats.TotalRatings;

            // Get treatment type breakdown
            const string treatmentSql = @"
                SELECT 
                    treatment_type as TreatmentType,
                    COUNT(*) as CaseCount,
                    COALESCE(AVG(r.rating), 0) as AverageRating,
                    COALESCE(SUM(ba.view_count), 0) as TotalViews
                FROM before_after ba
                LEFT JOIN before_after_ratings r ON ba.id = r.before_after_id
                WHERE ba.is_active = true
                GROUP BY treatment_type";

            stats.TreatmentBreakdown = (await connection.QueryAsync<TreatmentTypeStats>(treatmentSql)).ToList();

            // Get most viewed cases
            const string popularSql = @"
                SELECT 
                    ba.id,
                    ba.title,
                    ba.treatment_type,
                    ba.view_count,
                    ba.before_image_url,
                    ba.after_image_url,
                    COALESCE(AVG(r.rating), 0) as Rating
                FROM before_after ba
                LEFT JOIN before_after_ratings r ON ba.id = r.before_after_id
                WHERE ba.is_active = true AND ba.is_public = true
                GROUP BY ba.id, ba.title, ba.treatment_type, ba.view_count, ba.before_image_url, ba.after_image_url
                ORDER BY ba.view_count DESC
                LIMIT 5";

            stats.MostViewed = (await connection.QueryAsync<PopularCase>(popularSql)).ToList();

            return stats;
        }

        public async Task<IEnumerable<BeforeAfterResponse>> GetPublicBeforeAfterAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT 
                    ba.*,
                    COALESCE(AVG(r.rating), 0) as Rating,
                    COUNT(r.id) as RatingCount
                FROM before_after ba
                LEFT JOIN before_after_ratings r ON ba.id = r.before_after_id
                WHERE ba.is_public = true AND ba.is_active = true
                GROUP BY ba.id
                ORDER BY ba.view_count DESC, ba.created_at DESC";

            var results = await connection.QueryAsync<BeforeAfterModel>(sql);
            
            return results.Select(MapToBeforeAfterResponse);
        }

        public async Task<IEnumerable<BeforeAfterResponse>> GetBeforeAfterByTreatmentTypeAsync(string treatmentType)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT 
                    ba.*,
                    COALESCE(AVG(r.rating), 0) as Rating,
                    COUNT(r.id) as RatingCount
                FROM before_after ba
                LEFT JOIN before_after_ratings r ON ba.id = r.before_after_id
                WHERE ba.treatment_type = @TreatmentType AND ba.is_active = true
                GROUP BY ba.id
                ORDER BY ba.created_at DESC";

            var results = await connection.QueryAsync<BeforeAfterModel>(sql, new { TreatmentType = treatmentType });
            
            return results.Select(MapToBeforeAfterResponse);
        }

        public async Task<IEnumerable<BeforeAfterResponse>> SearchBeforeAfterAsync(string searchTerm)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT 
                    ba.*,
                    COALESCE(AVG(r.rating), 0) as Rating,
                    COUNT(r.id) as RatingCount
                FROM before_after ba
                LEFT JOIN before_after_ratings r ON ba.id = r.before_after_id
                WHERE (
                    LOWER(ba.title) LIKE LOWER(@SearchTerm) OR 
                    LOWER(ba.description) LIKE LOWER(@SearchTerm) OR 
                    LOWER(ba.treatment_type) LIKE LOWER(@SearchTerm) OR
                    LOWER(ba.dentist_name) LIKE LOWER(@SearchTerm)
                ) AND ba.is_active = true
                GROUP BY ba.id
                ORDER BY ba.created_at DESC";

            var results = await connection.QueryAsync<BeforeAfterModel>(sql, new { SearchTerm = $"%{searchTerm}%" });
            
            return results.Select(MapToBeforeAfterResponse);
        }

        public async Task<IEnumerable<string>> GetTreatmentTypesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT DISTINCT treatment_type 
                FROM before_after 
                WHERE is_active = true 
                ORDER BY treatment_type";

            return await connection.QueryAsync<string>(sql);
        }

        public async Task<bool> IncrementViewCountAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                UPDATE before_after 
                SET view_count = view_count + 1 
                WHERE id = @Id AND is_active = true";

            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        public async Task<bool> AddRatingAsync(int beforeAfterId, CreateRatingRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                INSERT INTO before_after_ratings (
                    before_after_id, user_name, user_email, rating, comment, created_at
                ) VALUES (
                    @BeforeAfterId, @UserName, @UserEmail, @Rating, @Comment, @CreatedAt
                )";

            var result = await connection.ExecuteAsync(sql, new
            {
                BeforeAfterId = beforeAfterId,
                request.UserName,
                request.UserEmail,
                request.Rating,
                request.Comment,
                CreatedAt = DateTime.UtcNow
            });

            return result > 0;
        }

        public async Task<IEnumerable<BeforeAfterRatingModel>> GetRatingsAsync(int beforeAfterId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = @"
                SELECT * FROM before_after_ratings 
                WHERE before_after_id = @BeforeAfterId 
                ORDER BY created_at DESC";

            return await connection.QueryAsync<BeforeAfterRatingModel>(sql, new { BeforeAfterId = beforeAfterId });
        }

        public async Task<bool> BeforeAfterExistsAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            const string sql = "SELECT COUNT(*) FROM before_after WHERE id = @Id";
            var count = await connection.QueryFirstAsync<int>(sql, new { Id = id });
            
            return count > 0;
        }

        private static BeforeAfterResponse MapToBeforeAfterResponse(BeforeAfterModel model)
        {
            return new BeforeAfterResponse
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                TreatmentType = model.TreatmentType,
                BeforeImageUrl = model.BeforeImageUrl,
                AfterImageUrl = model.AfterImageUrl,
                PatientId = model.PatientId,
                PatientAge = model.PatientAge,
                PatientGender = model.PatientGender,
                TreatmentDate = model.TreatmentDate,
                DentistName = model.DentistName,
                TreatmentDetails = model.TreatmentDetails,
                IsPublic = model.IsPublic,
                IsActive = model.IsActive,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                ViewCount = model.ViewCount,
                Rating = model.Rating,
                RatingCount = model.RatingCount,
                Tags = model.Tags
            };
        }
    }
}