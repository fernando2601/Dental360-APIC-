using Dapper;
using Npgsql;
using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public StaffRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("DATABASE_URL") 
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    specialization as Specialization,
                    email as Email,
                    phone as Phone,
                    bio as Bio,
                    address as Address,
                    salary as Salary,
                    hire_date as HireDate,
                    is_active as IsActive,
                    created_at as CreatedAt
                FROM staff 
                WHERE is_active = true
                ORDER BY name";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Staff>(sql);
        }

        public async Task<Staff?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    specialization as Specialization,
                    email as Email,
                    phone as Phone,
                    bio as Bio,
                    address as Address,
                    salary as Salary,
                    hire_date as HireDate,
                    is_active as IsActive,
                    created_at as CreatedAt
                FROM staff 
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Staff>(sql, new { Id = id });
        }

        public async Task<Staff> CreateAsync(CreateStaffDto staffDto)
        {
            const string sql = @"
                INSERT INTO staff (name, specialization, email, phone, bio, address, salary, hire_date, is_active, created_at)
                VALUES (@Name, @Specialization, @Email, @Phone, @Bio, @Address, @Salary, @HireDate, @IsActive, @CreatedAt)
                RETURNING 
                    id as Id,
                    name as Name,
                    specialization as Specialization,
                    email as Email,
                    phone as Phone,
                    bio as Bio,
                    address as Address,
                    salary as Salary,
                    hire_date as HireDate,
                    is_active as IsActive,
                    created_at as CreatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleAsync<Staff>(sql, new
            {
                staffDto.Name,
                staffDto.Specialization,
                staffDto.Email,
                staffDto.Phone,
                staffDto.Bio,
                staffDto.Address,
                staffDto.Salary,
                staffDto.HireDate,
                staffDto.IsActive,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<Staff?> UpdateAsync(int id, CreateStaffDto staffDto)
        {
            const string sql = @"
                UPDATE staff 
                SET 
                    name = @Name,
                    specialization = @Specialization,
                    email = @Email,
                    phone = @Phone,
                    bio = @Bio,
                    address = @Address,
                    salary = @Salary,
                    hire_date = @HireDate,
                    is_active = @IsActive
                WHERE id = @Id
                RETURNING 
                    id as Id,
                    name as Name,
                    specialization as Specialization,
                    email as Email,
                    phone as Phone,
                    bio as Bio,
                    address as Address,
                    salary as Salary,
                    hire_date as HireDate,
                    is_active as IsActive,
                    created_at as CreatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Staff>(sql, new
            {
                Id = id,
                staffDto.Name,
                staffDto.Specialization,
                staffDto.Email,
                staffDto.Phone,
                staffDto.Bio,
                staffDto.Address,
                staffDto.Salary,
                staffDto.HireDate,
                staffDto.IsActive
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE staff SET is_active = false WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Staff>> SearchAsync(string searchTerm)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    specialization as Specialization,
                    email as Email,
                    phone as Phone,
                    bio as Bio,
                    address as Address,
                    salary as Salary,
                    hire_date as HireDate,
                    is_active as IsActive,
                    created_at as CreatedAt
                FROM staff 
                WHERE 
                    is_active = true AND
                    (name ILIKE @SearchTerm 
                    OR specialization ILIKE @SearchTerm 
                    OR email ILIKE @SearchTerm 
                    OR phone ILIKE @SearchTerm)
                ORDER BY name";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Staff>(sql, new { SearchTerm = $"%{searchTerm}%" });
        }

        public async Task<IEnumerable<Staff>> GetBySpecializationAsync(string specialization)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    specialization as Specialization,
                    email as Email,
                    phone as Phone,
                    bio as Bio,
                    address as Address,
                    salary as Salary,
                    hire_date as HireDate,
                    is_active as IsActive,
                    created_at as CreatedAt
                FROM staff 
                WHERE specialization = @Specialization AND is_active = true
                ORDER BY name";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Staff>(sql, new { Specialization = specialization });
        }
    }
}