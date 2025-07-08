using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbConnection _connection;

        public ClientRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            var list = new List<Client>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, full_name, email, phone FROM clients WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Client
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetString(reader.GetOrdinal("phone"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, full_name, email, phone FROM clients WHERE id = @Id AND is_active = 1";
                var param = cmd.CreateParameter(); param.ParameterName = "@Id"; param.Value = id; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Client
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetString(reader.GetOrdinal("phone"))
                        };
                    }
                }
            }
            return await Task.FromResult<Client?>(null);
        }

        public async Task<Client> CreateAsync(Client client)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO clients (full_name, email, phone, is_active) VALUES (@FullName, @Email, @Phone, 1); SELECT LASTVAL();";
                var p1 = cmd.CreateParameter(); p1.ParameterName = "@FullName"; p1.Value = client.FullName; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@Email"; p2.Value = client.Email; cmd.Parameters.Add(p2);
                var p3 = cmd.CreateParameter(); p3.ParameterName = "@Phone"; p3.Value = client.Phone; cmd.Parameters.Add(p3);
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                client.Id = id;
                return await Task.FromResult(client);
            }
        }

        public async Task<Client?> UpdateAsync(int id, Client client)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE clients SET full_name = @FullName, email = @Email, phone = @Phone WHERE id = @Id AND is_active = 1";
                var p0 = cmd.CreateParameter(); p0.ParameterName = "@Id"; p0.Value = id; cmd.Parameters.Add(p0);
                var p1 = cmd.CreateParameter(); p1.ParameterName = "@FullName"; p1.Value = client.FullName; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@Email"; p2.Value = client.Email; cmd.Parameters.Add(p2);
                var p3 = cmd.CreateParameter(); p3.ParameterName = "@Phone"; p3.Value = client.Phone; cmd.Parameters.Add(p3);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? client : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE clients SET is_active = 0 WHERE id = @Id";
                var param = cmd.CreateParameter(); param.ParameterName = "@Id"; param.Value = id; cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<Client>> SearchAsync(string searchTerm)
        {
            var clients = new List<Client>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM clients WHERE name LIKE @SearchTerm";
                var param = cmd.CreateParameter(); param.ParameterName = "@SearchTerm"; param.Value = $"%{searchTerm}%"; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Mapear para entidade Client
                    }
                }
            }
            return clients;
        }
    }
} 