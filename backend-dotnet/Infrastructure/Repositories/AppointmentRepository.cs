using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IDbConnection _connection;

        public AppointmentRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            var appointments = new List<Appointment>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM appointments WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appointments.Add(new Appointment
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        });
                    }
                }
            }
            return await Task.FromResult(appointments);
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM appointments WHERE id = @Id AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Appointment
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        };
                    }
                }
            }
            return await Task.FromResult<Appointment?>(null);
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO appointments (is_active) VALUES (1); SELECT CAST(SCOPE_IDENTITY() as int)";
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                appointment.Id = id;
                return await Task.FromResult(appointment);
            }
        }

        public async Task<Appointment?> UpdateAsync(int id, Appointment appointment)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE appointments SET is_active = 1 WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? appointment : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE appointments SET is_active = 0 WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<Appointment>> SearchAsync(string searchTerm)
        {
            var appointments = new List<Appointment>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM appointments WHERE is_active = 1 AND title LIKE @SearchTerm";
                var param = cmd.CreateParameter();
                param.ParameterName = "@SearchTerm";
                param.Value = $"%{searchTerm}%";
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appointments.Add(new Appointment
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        });
                    }
                }
            }
            return await Task.FromResult(appointments);
        }
    }
} 