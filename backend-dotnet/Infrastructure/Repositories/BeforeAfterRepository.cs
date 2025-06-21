using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace DentalSpa.Infrastructure.Repositories
{
    public class BeforeAfterRepository : IBeforeAfterRepository
    {
        private readonly IDbConnection _connection;

        public BeforeAfterRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<BeforeAfter>> GetAllAsync()
        {
            const string sql = "SELECT * FROM before_after WHERE is_active = 1";
            return await Task.FromResult(_connection.Query<BeforeAfter>(sql));
        }

        public async Task<BeforeAfter?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM before_after WHERE id = @Id AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirstOrDefault<BeforeAfter>(sql, new { Id = id }));
        }

        public async Task<BeforeAfter> CreateAsync(BeforeAfter beforeAfter)
        {
            const string sql = @"
                INSERT INTO before_after (patient_id, before_image, after_image, description, is_active, created_at, updated_at)
                VALUES (@PatientId, @BeforeImage, @AfterImage, @Description, 1, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            
            beforeAfter.CreatedAt = DateTime.UtcNow;
            beforeAfter.UpdatedAt = DateTime.UtcNow;
            
            var id = await Task.FromResult(_connection.QuerySingle<int>(sql, beforeAfter));
            beforeAfter.Id = id;
            return beforeAfter;
        }

        public async Task<BeforeAfter?> UpdateAsync(int id, BeforeAfter beforeAfter)
        {
            const string sql = @"
                UPDATE before_after 
                SET patient_id = @PatientId, before_image = @BeforeImage, 
                    after_image = @AfterImage, description = @Description, updated_at = @UpdatedAt
                WHERE id = @Id AND is_active = 1";
            
            beforeAfter.Id = id;
            beforeAfter.UpdatedAt = DateTime.UtcNow;
            
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, beforeAfter));
            return rowsAffected > 0 ? beforeAfter : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE before_after SET is_active = 0 WHERE id = @Id";
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, new { Id = id }));
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<BeforeAfter>> SearchAsync(string searchTerm)
        {
            const string sql = @"
                SELECT ba.* FROM before_after ba
                INNER JOIN patients p ON ba.patient_id = p.id
                WHERE ba.is_active = 1 AND p.name LIKE @SearchTerm";
            
            return await Task.FromResult(_connection.Query<BeforeAfter>(sql, new { SearchTerm = $"%{searchTerm}%" }));
        }

        public async Task<IEnumerable<BeforeAfter>> GetPublicAsync()
        {
            const string sql = "SELECT * FROM before_after WHERE is_public = 1 AND is_active = 1";
            return await Task.FromResult(_connection.Query<BeforeAfter>(sql));
        }

        public async Task<IEnumerable<BeforeAfter>> GetByServiceAsync(int serviceId)
        {
            const string sql = "SELECT * FROM before_after WHERE service_id = @ServiceId AND is_active = 1";
            return await Task.FromResult(_connection.Query<BeforeAfter>(sql, new { ServiceId = serviceId }));
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string sql = "SELECT 1 FROM before_after WHERE id = @Id AND is_active = 1";
            var result = await Task.FromResult(_connection.QueryFirstOrDefault<int>(sql, new { Id = id }));
            return result == 1;
        }
    }
}