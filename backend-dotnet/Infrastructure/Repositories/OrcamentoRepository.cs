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
                cmd.CommandText = "SELECT id FROM orcamentos WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orcamentos.Add(new Orcamento
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
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
                cmd.CommandText = "SELECT id FROM orcamentos WHERE id = @Id AND is_active = 1";
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
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
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
                cmd.CommandText = "INSERT INTO orcamentos (is_active) VALUES (1); SELECT CAST(SCOPE_IDENTITY() as int)";
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                orcamento.Id = id;
                return await Task.FromResult(orcamento);
            }
        }

        public async Task<Orcamento?> UpdateAsync(int id, Orcamento orcamento)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE orcamentos SET is_active = 1 WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
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

        public async Task<IEnumerable<Orcamento>> SearchAsync(string searchTerm)
        {
            var orcamentos = new List<Orcamento>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM orcamentos WHERE is_active = 1 AND description LIKE @SearchTerm";
                var param = cmd.CreateParameter();
                param.ParameterName = "@SearchTerm";
                param.Value = $"%{searchTerm}%";
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orcamentos.Add(new Orcamento
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        });
                    }
                }
            }
            return await Task.FromResult(orcamentos);
        }
    }
} 