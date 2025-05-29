using Dapper;
using Npgsql;
using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("DATABASE_URL") 
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    price as Price,
                    duration as Duration,
                    is_active as IsActive,
                    created_at as CreatedAt
                FROM services 
                WHERE is_active = true
                ORDER BY name";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Service>(sql);
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    price as Price,
                    duration as Duration,
                    is_active as IsActive,
                    created_at as CreatedAt
                FROM services 
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Service>(sql, new { Id = id });
        }

        public async Task<Service> CreateAsync(CreateServiceDto serviceDto)
        {
            const string sql = @"
                INSERT INTO services (name, category, description, price, duration, is_active, created_at)
                VALUES (@Name, @Category, @Description, @Price, @Duration, @IsActive, @CreatedAt)
                RETURNING 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    price as Price,
                    duration as Duration,
                    is_active as IsActive,
                    created_at as CreatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleAsync<Service>(sql, new
            {
                serviceDto.Name,
                serviceDto.Category,
                serviceDto.Description,
                serviceDto.Price,
                serviceDto.Duration,
                serviceDto.IsActive,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<Service?> UpdateAsync(int id, CreateServiceDto serviceDto)
        {
            const string sql = @"
                UPDATE services 
                SET 
                    name = @Name,
                    category = @Category,
                    description = @Description,
                    price = @Price,
                    duration = @Duration,
                    is_active = @IsActive
                WHERE id = @Id
                RETURNING 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    price as Price,
                    duration as Duration,
                    is_active as IsActive,
                    created_at as CreatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Service>(sql, new
            {
                Id = id,
                serviceDto.Name,
                serviceDto.Category,
                serviceDto.Description,
                serviceDto.Price,
                serviceDto.Duration,
                serviceDto.IsActive
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE services SET is_active = false WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Service>> SearchAsync(string searchTerm)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    price as Price,
                    duration as Duration,
                    is_active as IsActive,
                    created_at as CreatedAt
                FROM services 
                WHERE 
                    is_active = true AND
                    (name ILIKE @SearchTerm 
                    OR category ILIKE @SearchTerm 
                    OR description ILIKE @SearchTerm)
                ORDER BY name";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Service>(sql, new { SearchTerm = $"%{searchTerm}%" });
        }

        public async Task<IEnumerable<Service>> GetByCategoryAsync(string category)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    price as Price,
                    duration as Duration,
                    is_active as IsActive,
                    created_at as CreatedAt
                FROM services 
                WHERE category = @Category AND is_active = true
                ORDER BY name";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Service>(sql, new { Category = category });
        }
    }
}