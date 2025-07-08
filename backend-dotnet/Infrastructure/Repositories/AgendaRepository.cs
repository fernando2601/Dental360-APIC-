using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class AgendaRepository : IAgendaRepository
    {
        private readonly IDbConnection _connection;

        public AgendaRepository(IDbConnection connection)
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

        public async Task<IEnumerable<Appointment>> GetByStaffAndTimeRangeAsync(int staffId, DateTime start, DateTime end)
        {
            var appointments = new List<Appointment>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, staffid, starttime, endtime FROM appointments WHERE staffid = @StaffId AND is_active = 1 AND ((starttime < @End AND endtime > @Start))";
                var paramStaff = cmd.CreateParameter();
                paramStaff.ParameterName = "@StaffId";
                paramStaff.Value = staffId;
                cmd.Parameters.Add(paramStaff);
                var paramStart = cmd.CreateParameter();
                paramStart.ParameterName = "@Start";
                paramStart.Value = start;
                cmd.Parameters.Add(paramStart);
                var paramEnd = cmd.CreateParameter();
                paramEnd.ParameterName = "@End";
                paramEnd.Value = end;
                cmd.Parameters.Add(paramEnd);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appointments.Add(new Appointment
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            StaffId = reader.GetInt32(reader.GetOrdinal("staffid")),
                            StartTime = reader.GetDateTime(reader.GetOrdinal("starttime")),
                            EndTime = reader.GetDateTime(reader.GetOrdinal("endtime"))
                        });
                    }
                }
            }
            return await Task.FromResult(appointments);
        }
    }
} 