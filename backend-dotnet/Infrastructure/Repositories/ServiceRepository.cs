using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

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
            var services = new List<Service>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM services WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new Service
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        });
                    }
                }
            }
            return await Task.FromResult(services);
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM services WHERE id = @Id AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Service
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        };
                    }
                }
            }
            return await Task.FromResult<Service?>(null);
        }

        public async Task<Service> CreateAsync(Service service)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO services (is_active) VALUES (1); SELECT CAST(SCOPE_IDENTITY() as int)";
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                service.Id = id;
                return await Task.FromResult(service);
            }
        }

        public async Task<Service?> UpdateAsync(int id, Service service)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE services SET is_active = 1 WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? service : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE services SET is_active = 0 WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<Service>> SearchAsync(string searchTerm)
        {
            var services = new List<Service>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM services WHERE is_active = 1 AND name ILIKE @SearchTerm";
                var param = cmd.CreateParameter();
                param.ParameterName = "@SearchTerm";
                param.Value = $"%{searchTerm}%";
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new Service
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        });
                    }
                }
            }
            return await Task.FromResult(services);
        }

        public async Task<IEnumerable<Service>> GetByCategoryAsync(string category)
        {
            var services = new List<Service>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM services WHERE category = @Category AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Category";
                param.Value = category;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new Service
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        });
                    }
                }
            }
            return await Task.FromResult(services);
        }

        public async Task SetServiceStaffAsync(int serviceId, List<int> staffIds)
        {
            // Remove relações antigas
            using (var deleteCmd = _connection.CreateCommand())
            {
                deleteCmd.CommandText = "DELETE FROM staff_service WHERE service_id = @ServiceId";
                var param = deleteCmd.CreateParameter(); param.ParameterName = "@ServiceId"; param.Value = serviceId; deleteCmd.Parameters.Add(param);
                deleteCmd.ExecuteNonQuery();
            }
            // Adiciona novas relações
            foreach (var staffId in staffIds)
            {
                using (var insertCmd = _connection.CreateCommand())
                {
                    insertCmd.CommandText = "INSERT INTO staff_service (service_id, staff_id) VALUES (@ServiceId, @StaffId)";
                    var param1 = insertCmd.CreateParameter(); param1.ParameterName = "@ServiceId"; param1.Value = serviceId; insertCmd.Parameters.Add(param1);
                    var param2 = insertCmd.CreateParameter(); param2.ParameterName = "@StaffId"; param2.Value = staffId; insertCmd.Parameters.Add(param2);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category)
        {
            var services = new List<Service>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM services WHERE category = @Category";
                var param = cmd.CreateParameter(); param.ParameterName = "@Category"; param.Value = category; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Mapear para entidade Service
                    }
                }
            }
            return services;
        }
    }
} 