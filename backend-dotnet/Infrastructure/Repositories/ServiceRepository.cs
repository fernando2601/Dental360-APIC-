using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace DentalSpa.Infrastructure.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly IDbConnection _connection;

        public ServiceRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            const string sql = "SELECT * FROM services WHERE is_active = 1";
            return await Task.FromResult(_connection.Query<Service>(sql));
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM services WHERE id = @Id AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirstOrDefault<Service>(sql, new { Id = id }));
        }

        public async Task<Service> CreateAsync(Service service)
        {
            const string sql = @"
                INSERT INTO services (name, description, price, duration, is_active, created_at, updated_at)
                VALUES (@Name, @Description, @Price, @Duration, 1, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            
            service.CreatedAt = DateTime.UtcNow;
            service.UpdatedAt = DateTime.UtcNow;
            
            var id = await Task.FromResult(_connection.QuerySingle<int>(sql, service));
            service.Id = id;
            return service;
        }

        public async Task<Service?> UpdateAsync(int id, Service service)
        {
            const string sql = @"
                UPDATE services 
                SET name = @Name, description = @Description, 
                    price = @Price, duration = @Duration, updated_at = @UpdatedAt
                WHERE id = @Id AND is_active = 1";
            
            service.Id = id;
            service.UpdatedAt = DateTime.UtcNow;
            
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, service));
            return rowsAffected > 0 ? service : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE services SET is_active = 0 WHERE id = @Id";
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, new { Id = id }));
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Service>> SearchAsync(string searchTerm)
        {
            const string sql = "SELECT * FROM services WHERE is_active = 1 AND name LIKE @SearchTerm";
            return await Task.FromResult(_connection.Query<Service>(sql, new { SearchTerm = $"%{searchTerm}%" }));
        }

        public async Task<IEnumerable<Service>> GetByCategoryAsync(string category)
        {
            const string sql = "SELECT * FROM services WHERE category = @Category AND is_active = 1";
            return await Task.FromResult(_connection.Query<Service>(sql, new { Category = category }));
        }
    }
}