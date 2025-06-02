using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public interface IServiceRepository
    {
        Task<IEnumerable<ServiceModel>> GetAllAsync();
        Task<ServiceModel?> GetByIdAsync(int id);
        Task<ServiceModel> CreateAsync(CreateServiceRequest request);
        Task<ServiceModel?> UpdateAsync(int id, UpdateServiceRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ServiceModel>> GetByCategoryAsync(string category);
        Task<ServiceStatsResponse> GetStatsAsync();
        Task<IEnumerable<string>> GetCategoriesAsync();
    }

    public class ServiceRepository : IServiceRepository
    {
        private readonly string _connectionString;

        public ServiceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ServiceModel>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, name, category, description, price, duration_minutes, is_active, created_at, updated_at
                FROM services 
                ORDER BY name";

            return await connection.QueryAsync<ServiceModel>(sql);
        }

        public async Task<ServiceModel?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, name, category, description, price, duration_minutes, is_active, created_at, updated_at
                FROM services 
                WHERE id = @Id";

            return await connection.QueryFirstOrDefaultAsync<ServiceModel>(sql, new { Id = id });
        }

        public async Task<ServiceModel> CreateAsync(CreateServiceRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO services (name, category, description, price, duration_minutes, is_active, created_at)
                VALUES (@Name, @Category, @Description, @Price, @DurationMinutes, @IsActive, @CreatedAt)
                RETURNING id, name, category, description, price, duration_minutes, is_active, created_at, updated_at";

            var service = await connection.QueryFirstAsync<ServiceModel>(sql, new
            {
                request.Name,
                request.Category,
                request.Description,
                request.Price,
                request.DurationMinutes,
                request.IsActive,
                CreatedAt = DateTime.UtcNow
            });

            return service;
        }

        public async Task<ServiceModel?> UpdateAsync(int id, UpdateServiceRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE services 
                SET name = @Name, 
                    category = @Category, 
                    description = @Description, 
                    price = @Price, 
                    duration_minutes = @DurationMinutes, 
                    is_active = @IsActive, 
                    updated_at = @UpdatedAt
                WHERE id = @Id
                RETURNING id, name, category, description, price, duration_minutes, is_active, created_at, updated_at";

            return await connection.QueryFirstOrDefaultAsync<ServiceModel>(sql, new
            {
                Id = id,
                request.Name,
                request.Category,
                request.Description,
                request.Price,
                request.DurationMinutes,
                request.IsActive,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "DELETE FROM services WHERE id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<ServiceModel>> GetByCategoryAsync(string category)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, name, category, description, price, duration_minutes, is_active, created_at, updated_at
                FROM services 
                WHERE category = @Category AND is_active = true
                ORDER BY name";

            return await connection.QueryAsync<ServiceModel>(sql, new { Category = category });
        }

        public async Task<ServiceStatsResponse> GetStatsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string statsSql = @"
                SELECT 
                    COUNT(*) as TotalServices,
                    COUNT(CASE WHEN is_active = true THEN 1 END) as ActiveServices,
                    COUNT(CASE WHEN is_active = false THEN 1 END) as InactiveServices,
                    COALESCE(AVG(price), 0) as AveragePrice,
                    COALESCE(AVG(duration_minutes), 0) as AverageDuration
                FROM services";

            const string categorySql = @"
                SELECT 
                    category as Category,
                    COUNT(*) as Count,
                    COALESCE(AVG(price), 0) as AveragePrice
                FROM services 
                WHERE is_active = true
                GROUP BY category
                ORDER BY category";

            var stats = await connection.QueryFirstAsync<ServiceStatsResponse>(statsSql);
            var categoryStats = await connection.QueryAsync<CategoryStats>(categorySql);
            
            stats.CategoryBreakdown = categoryStats.ToList();
            return stats;
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT DISTINCT category 
                FROM services 
                WHERE is_active = true
                ORDER BY category";

            return await connection.QueryAsync<string>(sql);
        }
    }
}