using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

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
            var orcamentos = new List<Orcamento>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, paciente_id, valor_total, status, created_at, updated_at FROM orcamentos WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orcamentos.Add(new Orcamento
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            PacienteId = reader.GetInt32(reader.GetOrdinal("paciente_id")),
                            ValorTotal = reader.GetDecimal(reader.GetOrdinal("valor_total")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(orcamentos);
        }

        public async Task<Orcamento?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, paciente_id, valor_total, status, created_at, updated_at FROM orcamentos WHERE id = @Id AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Orcamento
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            PacienteId = reader.GetInt32(reader.GetOrdinal("paciente_id")),
                            ValorTotal = reader.GetDecimal(reader.GetOrdinal("valor_total")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<Orcamento?>(null);
        }

        public async Task<Orcamento> CreateAsync(Orcamento orcamento)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO orcamentos (paciente_id, valor_total, status, created_at, updated_at, is_active) 
                                   VALUES (@PacienteId, @ValorTotal, @Status, @CreatedAt, @UpdatedAt, 1); 
                                   SELECT CAST(SCOPE_IDENTITY() as int)";
                cmd.Parameters.Add(CreateParameter("@PacienteId", orcamento.PacienteId));
                cmd.Parameters.Add(CreateParameter("@ValorTotal", orcamento.ValorTotal));
                cmd.Parameters.Add(CreateParameter("@Status", orcamento.Status));
                cmd.Parameters.Add(CreateParameter("@CreatedAt", DateTime.Now));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                orcamento.Id = id;
                return await Task.FromResult(orcamento);
            }
        }

        public async Task<Orcamento?> UpdateAsync(int id, Orcamento orcamento)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE orcamentos SET paciente_id = @PacienteId, valor_total = @ValorTotal, status = @Status, updated_at = @UpdatedAt 
                                   WHERE id = @Id AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                cmd.Parameters.Add(CreateParameter("@PacienteId", orcamento.PacienteId));
                cmd.Parameters.Add(CreateParameter("@ValorTotal", orcamento.ValorTotal));
                cmd.Parameters.Add(CreateParameter("@Status", orcamento.Status));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? orcamento : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE orcamentos SET is_active = 0 WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<Orcamento>> GetOrcamentosByPacienteAsync(int pacienteId)
        {
            var orcamentos = new List<Orcamento>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, paciente_id, valor_total, status, created_at, updated_at FROM orcamentos WHERE paciente_id = @PacienteId AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@PacienteId";
                param.Value = pacienteId;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orcamentos.Add(new Orcamento
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            PacienteId = reader.GetInt32(reader.GetOrdinal("paciente_id")),
                            ValorTotal = reader.GetDecimal(reader.GetOrdinal("valor_total")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(orcamentos);
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE orcamentos SET status = @Status, updated_at = @UpdatedAt WHERE id = @Id AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                cmd.Parameters.Add(CreateParameter("@Status", status));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        private IDbDataParameter CreateParameter(string name, object? value)
        {
            var param = _connection.CreateCommand().CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }
    }
} 