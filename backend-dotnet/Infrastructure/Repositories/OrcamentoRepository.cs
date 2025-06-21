using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace DentalSpa.Infrastructure.Repositories
{
    public class OrcamentoRepository : IOrcamentoRepository
    {
        private readonly IDbConnection _connection;

        public OrcamentoRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Orcamento>> GetAllAsync()
        {
            const string sql = "SELECT * FROM orcamentos WHERE is_active = 1";
            return await Task.FromResult(_connection.Query<Orcamento>(sql));
        }

        public async Task<Orcamento?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM orcamentos WHERE id = @Id AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirstOrDefault<Orcamento>(sql, new { Id = id }));
        }

        public async Task<Orcamento> CreateAsync(Orcamento orcamento)
        {
            const string sql = @"
                INSERT INTO orcamentos (patient_id, total_value, status, is_active, created_at, updated_at)
                VALUES (@PatientId, @TotalValue, @Status, 1, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            
            orcamento.CreatedAt = DateTime.UtcNow;
            orcamento.UpdatedAt = DateTime.UtcNow;
            
            var id = await Task.FromResult(_connection.QuerySingle<int>(sql, orcamento));
            orcamento.Id = id;
            return orcamento;
        }

        public async Task<Orcamento?> UpdateAsync(int id, Orcamento orcamento)
        {
            const string sql = @"
                UPDATE orcamentos 
                SET patient_id = @PatientId, total_value = @TotalValue, 
                    status = @Status, updated_at = @UpdatedAt
                WHERE id = @Id AND is_active = 1";
            
            orcamento.Id = id;
            orcamento.UpdatedAt = DateTime.UtcNow;
            
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, orcamento));
            return rowsAffected > 0 ? orcamento : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE orcamentos SET is_active = 0 WHERE id = @Id";
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, new { Id = id }));
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Orcamento>> SearchAsync(string searchTerm)
        {
            const string sql = @"
                SELECT o.* FROM orcamentos o
                INNER JOIN patients p ON o.patient_id = p.id
                WHERE o.is_active = 1 AND p.name LIKE @SearchTerm";
            
            return await Task.FromResult(_connection.Query<Orcamento>(sql, new { SearchTerm = $"%{searchTerm}%" }));
        }
    }
} 