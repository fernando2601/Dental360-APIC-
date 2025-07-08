using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IDbConnection _connection;
        public PermissionRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            var permissions = new List<Permission>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description FROM permissions";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permissions.Add(new Permission
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))
                        });
                    }
                }
            }
            return await Task.FromResult(permissions);
        }
        public async Task<Permission?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description FROM permissions WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Permission
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))
                        };
                    }
                }
            }
            return await Task.FromResult<Permission?>(null);
        }
        public async Task<Permission?> GetByNameAsync(string name)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description FROM permissions WHERE name = @Name";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Name";
                param.Value = name;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Permission
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))
                        };
                    }
                }
            }
            return await Task.FromResult<Permission?>(null);
        }
    }
} 