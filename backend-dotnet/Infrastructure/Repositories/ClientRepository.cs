using Dapper;
using Npgsql;
using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ClientRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("DATABASE_URL") 
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    full_name as FullName,
                    email as Email,
                    phone as Phone,
                    address as Address,
                    birthday as Birthday,
                    notes as Notes,
                    created_at as CreatedAt
                FROM clients 
                ORDER BY created_at DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Client>(sql);
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    full_name as FullName,
                    email as Email,
                    phone as Phone,
                    address as Address,
                    birthday as Birthday,
                    notes as Notes,
                    created_at as CreatedAt
                FROM clients 
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Client>(sql, new { Id = id });
        }

        public async Task<Client> CreateAsync(CreateClientDto clientDto)
        {
            const string sql = @"
                INSERT INTO clients (full_name, email, phone, address, birthday, notes, created_at)
                VALUES (@FullName, @Email, @Phone, @Address, @Birthday, @Notes, @CreatedAt)
                RETURNING 
                    id as Id,
                    full_name as FullName,
                    email as Email,
                    phone as Phone,
                    address as Address,
                    birthday as Birthday,
                    notes as Notes,
                    created_at as CreatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleAsync<Client>(sql, new
            {
                clientDto.FullName,
                clientDto.Email,
                clientDto.Phone,
                clientDto.Address,
                clientDto.Birthday,
                clientDto.Notes,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<Client?> UpdateAsync(int id, CreateClientDto clientDto)
        {
            const string sql = @"
                UPDATE clients 
                SET 
                    full_name = @FullName,
                    email = @Email,
                    phone = @Phone,
                    address = @Address,
                    birthday = @Birthday,
                    notes = @Notes
                WHERE id = @Id
                RETURNING 
                    id as Id,
                    full_name as FullName,
                    email as Email,
                    phone as Phone,
                    address as Address,
                    birthday as Birthday,
                    notes as Notes,
                    created_at as CreatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Client>(sql, new
            {
                Id = id,
                clientDto.FullName,
                clientDto.Email,
                clientDto.Phone,
                clientDto.Address,
                clientDto.Birthday,
                clientDto.Notes
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM clients WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Client>> SearchAsync(string searchTerm)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    full_name as FullName,
                    email as Email,
                    phone as Phone,
                    address as Address,
                    birthday as Birthday,
                    notes as Notes,
                    created_at as CreatedAt
                FROM clients 
                WHERE 
                    full_name ILIKE @SearchTerm 
                    OR email ILIKE @SearchTerm 
                    OR phone ILIKE @SearchTerm
                ORDER BY created_at DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Client>(sql, new { SearchTerm = $"%{searchTerm}%" });
        }
    }
}