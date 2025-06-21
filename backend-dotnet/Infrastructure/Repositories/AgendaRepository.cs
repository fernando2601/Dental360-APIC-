using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;
using System.Data.SqlClient;

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
            const string sql = "SELECT * FROM appointments WHERE is_active = 1";
            return await Task.FromResult(_connection.Query<Appointment>(sql));
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM appointments WHERE id = @Id AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirstOrDefault<Appointment>(sql, new { Id = id }));
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            const string sql = @"
                INSERT INTO appointments (patient_id, service_id, staff_id, start_time, end_time, status, notes, room, estimated_cost, priority, is_active, created_at, updated_at)
                VALUES (@PatientId, @ServiceId, @StaffId, @StartTime, @EndTime, @Status, @Notes, @Room, @EstimatedCost, @Priority, 1, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;
            
            var id = await Task.FromResult(_connection.QuerySingle<int>(sql, appointment));
            appointment.Id = id;
            return appointment;
        }

        public async Task<Appointment?> UpdateAsync(int id, Appointment appointment)
        {
            const string sql = @"
                UPDATE appointments 
                SET patient_id = @PatientId, service_id = @ServiceId, staff_id = @StaffId, 
                    start_time = @StartTime, end_time = @EndTime, status = @Status, 
                    notes = @Notes, room = @Room, estimated_cost = @EstimatedCost, 
                    priority = @Priority, updated_at = @UpdatedAt
                WHERE id = @Id AND is_active = 1";
            
            appointment.Id = id;
            appointment.UpdatedAt = DateTime.UtcNow;
            
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, appointment));
            return rowsAffected > 0 ? appointment : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE appointments SET is_active = 0 WHERE id = @Id";
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, new { Id = id }));
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Appointment>> SearchAsync(string searchTerm)
        {
            const string sql = @"
                SELECT a.* FROM appointments a
                INNER JOIN patients p ON a.patient_id = p.id
                WHERE a.is_active = 1 AND p.name LIKE @SearchTerm";
            
            return await Task.FromResult(_connection.Query<Appointment>(sql, new { SearchTerm = $"%{searchTerm}%" }));
        }
    }
} 