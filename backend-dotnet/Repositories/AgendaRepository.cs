using Dapper;
using Npgsql;
using ClinicApi.Models;
using System.Text;

namespace ClinicApi.Repositories
{
    public class AgendaRepository : IAgendaRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AgendaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("DATABASE_URL") 
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    client_id as ClientId,
                    staff_id as StaffId,
                    service_id as ServiceId,
                    start_time as StartTime,
                    end_time as EndTime,
                    status as Status,
                    notes as Notes,
                    room as Room,
                    recurrence_pattern as RecurrencePattern,
                    recurrence_parent_id as RecurrenceParentId,
                    priority as Priority,
                    reminder_type as ReminderType,
                    reminder_sent_at as ReminderSentAt,
                    is_blocked as IsBlocked,
                    block_reason as BlockReason,
                    estimated_cost as EstimatedCost,
                    insurance_provider as InsuranceProvider,
                    pre_auth_code as PreAuthCode,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM appointments 
                ORDER BY start_time DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Appointment>(sql);
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    client_id as ClientId,
                    staff_id as StaffId,
                    service_id as ServiceId,
                    start_time as StartTime,
                    end_time as EndTime,
                    status as Status,
                    notes as Notes,
                    room as Room,
                    recurrence_pattern as RecurrencePattern,
                    recurrence_parent_id as RecurrenceParentId,
                    priority as Priority,
                    reminder_type as ReminderType,
                    reminder_sent_at as ReminderSentAt,
                    is_blocked as IsBlocked,
                    block_reason as BlockReason,
                    estimated_cost as EstimatedCost,
                    insurance_provider as InsuranceProvider,
                    pre_auth_code as PreAuthCode,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM appointments 
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Appointment>(sql, new { Id = id });
        }

        public async Task<Appointment> CreateAsync(CreateAppointmentDto appointmentDto)
        {
            const string sql = @"
                INSERT INTO appointments 
                (client_id, staff_id, service_id, start_time, end_time, notes, room, 
                 recurrence_pattern, priority, reminder_type, estimated_cost, 
                 insurance_provider, pre_auth_code, created_at, updated_at)
                VALUES 
                (@ClientId, @StaffId, @ServiceId, @StartTime, @EndTime, @Notes, @Room,
                 @RecurrencePattern, @Priority, @ReminderType, @EstimatedCost,
                 @InsuranceProvider, @PreAuthCode, @CreatedAt, @UpdatedAt)
                RETURNING 
                    id as Id,
                    client_id as ClientId,
                    staff_id as StaffId,
                    service_id as ServiceId,
                    start_time as StartTime,
                    end_time as EndTime,
                    status as Status,
                    notes as Notes,
                    room as Room,
                    recurrence_pattern as RecurrencePattern,
                    recurrence_parent_id as RecurrenceParentId,
                    priority as Priority,
                    reminder_type as ReminderType,
                    reminder_sent_at as ReminderSentAt,
                    is_blocked as IsBlocked,
                    block_reason as BlockReason,
                    estimated_cost as EstimatedCost,
                    insurance_provider as InsuranceProvider,
                    pre_auth_code as PreAuthCode,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            var now = DateTime.UtcNow;
            return await connection.QuerySingleAsync<Appointment>(sql, new
            {
                appointmentDto.ClientId,
                appointmentDto.StaffId,
                appointmentDto.ServiceId,
                appointmentDto.StartTime,
                appointmentDto.EndTime,
                appointmentDto.Notes,
                appointmentDto.Room,
                appointmentDto.RecurrencePattern,
                appointmentDto.Priority,
                appointmentDto.ReminderType,
                appointmentDto.EstimatedCost,
                appointmentDto.InsuranceProvider,
                appointmentDto.PreAuthCode,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        public async Task<Appointment?> UpdateAsync(int id, CreateAppointmentDto appointmentDto)
        {
            const string sql = @"
                UPDATE appointments 
                SET 
                    client_id = @ClientId,
                    staff_id = @StaffId,
                    service_id = @ServiceId,
                    start_time = @StartTime,
                    end_time = @EndTime,
                    notes = @Notes,
                    room = @Room,
                    recurrence_pattern = @RecurrencePattern,
                    priority = @Priority,
                    reminder_type = @ReminderType,
                    estimated_cost = @EstimatedCost,
                    insurance_provider = @InsuranceProvider,
                    pre_auth_code = @PreAuthCode,
                    updated_at = @UpdatedAt
                WHERE id = @Id
                RETURNING 
                    id as Id,
                    client_id as ClientId,
                    staff_id as StaffId,
                    service_id as ServiceId,
                    start_time as StartTime,
                    end_time as EndTime,
                    status as Status,
                    notes as Notes,
                    room as Room,
                    recurrence_pattern as RecurrencePattern,
                    recurrence_parent_id as RecurrenceParentId,
                    priority as Priority,
                    reminder_type as ReminderType,
                    reminder_sent_at as ReminderSentAt,
                    is_blocked as IsBlocked,
                    block_reason as BlockReason,
                    estimated_cost as EstimatedCost,
                    insurance_provider as InsuranceProvider,
                    pre_auth_code as PreAuthCode,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Appointment>(sql, new
            {
                Id = id,
                appointmentDto.ClientId,
                appointmentDto.StaffId,
                appointmentDto.ServiceId,
                appointmentDto.StartTime,
                appointmentDto.EndTime,
                appointmentDto.Notes,
                appointmentDto.Room,
                appointmentDto.RecurrencePattern,
                appointmentDto.Priority,
                appointmentDto.ReminderType,
                appointmentDto.EstimatedCost,
                appointmentDto.InsuranceProvider,
                appointmentDto.PreAuthCode,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM appointments WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<AppointmentCalendarView>> GetCalendarViewAsync(DateTime startDate, DateTime endDate, int? staffId = null)
        {
            var whereClause = staffId.HasValue ? "AND a.staff_id = @StaffId" : "";
            
            var sql = $@"
                SELECT 
                    a.id as Id,
                    c.full_name || ' - ' || s.name as Title,
                    a.start_time as Start,
                    a.end_time as End,
                    a.status as Status,
                    CASE a.status
                        WHEN 'scheduled' THEN '#3B82F6'
                        WHEN 'confirmed' THEN '#8B5CF6'
                        WHEN 'completed' THEN '#10B981'
                        WHEN 'cancelled' THEN '#EF4444'
                        WHEN 'no_show' THEN '#F59E0B'
                        ELSE '#6B7280'
                    END as StatusColor,
                    c.full_name as ClientName,
                    st.name as StaffName,
                    s.name as ServiceName,
                    COALESCE(a.room, 'Sala 1') as Room,
                    a.priority as Priority,
                    false as AllDay,
                    a.notes as Notes,
                    a.estimated_cost as EstimatedCost,
                    a.insurance_provider as InsuranceProvider
                FROM appointments a
                INNER JOIN clients c ON a.client_id = c.id
                INNER JOIN staff st ON a.staff_id = st.id
                INNER JOIN services s ON a.service_id = s.id
                WHERE a.start_time >= @StartDate 
                    AND a.start_time <= @EndDate
                    {whereClause}
                ORDER BY a.start_time";

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = new { StartDate = startDate, EndDate = endDate, StaffId = staffId };
            return await connection.QueryAsync<AppointmentCalendarView>(sql, parameters);
        }

        public async Task<IEnumerable<AppointmentCalendarView>> GetDayViewAsync(DateTime date, int? staffId = null)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1).AddTicks(-1);
            return await GetCalendarViewAsync(startDate, endDate, staffId);
        }

        public async Task<IEnumerable<AppointmentCalendarView>> GetWeekViewAsync(DateTime weekStart, int? staffId = null)
        {
            var endDate = weekStart.AddDays(7).AddTicks(-1);
            return await GetCalendarViewAsync(weekStart, endDate, staffId);
        }

        public async Task<IEnumerable<AppointmentCalendarView>> GetMonthViewAsync(DateTime monthStart, int? staffId = null)
        {
            var endDate = monthStart.AddMonths(1).AddTicks(-1);
            return await GetCalendarViewAsync(monthStart, endDate, staffId);
        }

        public async Task<ScheduleOverview> GetScheduleOverviewAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1).AddTicks(-1);

            // Buscar agendamentos do dia
            var appointments = await GetDayViewAsync(date);
            
            // Buscar disponibilidade da equipe
            var staffAvailability = await GetStaffAvailabilityAsync(date);

            var sql = @"
                SELECT COUNT(*) FROM appointments 
                WHERE start_time >= @StartDate AND start_time <= @EndDate";

            using var connection = new NpgsqlConnection(_connectionString);
            var totalAppointments = await connection.QuerySingleAsync<int>(sql, new { StartDate = startDate, EndDate = endDate });

            // Organizar por equipe
            var staffSchedules = staffAvailability.Select(staff => new StaffSchedule
            {
                StaffId = staff.StaffId,
                StaffName = staff.StaffName,
                Appointments = appointments.Where(a => a.StaffName == staff.StaffName).ToList(),
                AvailableSlots = staff.AvailableSlots,
                IsAvailable = staff.IsAvailable,
                StartTime = staff.StartTime,
                EndTime = staff.EndTime
            }).ToList();

            return new ScheduleOverview
            {
                Date = date,
                StaffSchedules = staffSchedules,
                TotalAppointments = totalAppointments,
                AvailableSlots = staffSchedules.Sum(s => s.AvailableSlots.Count)
            };
        }

        public async Task<IEnumerable<StaffAvailability>> GetStaffAvailabilityAsync(DateTime date, int? staffId = null)
        {
            var whereClause = staffId.HasValue ? "AND st.id = @StaffId" : "";
            
            var sql = $@"
                SELECT 
                    st.id as StaffId,
                    st.name as StaffName,
                    @Date as Date,
                    COALESCE(wh.start_time, '08:00:00') as StartTime,
                    COALESCE(wh.end_time, '18:00:00') as EndTime,
                    CASE WHEN wh.is_working_day IS NULL THEN true ELSE wh.is_working_day END as IsAvailable,
                    '' as UnavailabilityReason
                FROM staff st
                LEFT JOIN working_hours wh ON st.id = wh.staff_id 
                    AND wh.day_of_week = EXTRACT(DOW FROM @Date)
                WHERE st.is_active = true {whereClause}";

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = new { Date = date, StaffId = staffId };
            var availability = await connection.QueryAsync<StaffAvailability>(sql, parameters);

            // Para cada staff, calcular slots disponíveis
            var result = new List<StaffAvailability>();
            foreach (var staff in availability)
            {
                staff.AvailableSlots = await CalculateAvailableSlots(staff.StaffId, date, staff.StartTime, staff.EndTime);
                result.Add(staff);
            }

            return result;
        }

        public async Task<IEnumerable<AppointmentSlot>> GetAvailableSlotsAsync(DateTime date, int staffId, int serviceDuration)
        {
            // Buscar horários de trabalho
            var workingHours = await GetStaffWorkingHoursAsync(staffId);
            var dayWorkingHours = workingHours.FirstOrDefault(w => w.DayOfWeek == date.DayOfWeek);
            
            if (dayWorkingHours == null || !dayWorkingHours.IsWorkingDay)
                return new List<AppointmentSlot>();

            return await CalculateAvailableSlots(staffId, date, dayWorkingHours.StartTime, dayWorkingHours.EndTime, serviceDuration);
        }

        private async Task<List<AppointmentSlot>> CalculateAvailableSlots(int staffId, DateTime date, TimeSpan startTime, TimeSpan endTime, int serviceDuration = 30)
        {
            var slots = new List<AppointmentSlot>();
            var startDateTime = date.Date.Add(startTime);
            var endDateTime = date.Date.Add(endTime);

            // Buscar agendamentos existentes
            var existingAppointments = await GetStaffAppointmentsForDay(staffId, date);

            var currentTime = startDateTime;
            while (currentTime.AddMinutes(serviceDuration) <= endDateTime)
            {
                var slotEnd = currentTime.AddMinutes(serviceDuration);
                
                // Verificar se há conflito
                var hasConflict = existingAppointments.Any(a => 
                    (currentTime >= a.StartTime && currentTime < a.EndTime) ||
                    (slotEnd > a.StartTime && slotEnd <= a.EndTime) ||
                    (currentTime <= a.StartTime && slotEnd >= a.EndTime));

                slots.Add(new AppointmentSlot
                {
                    Start = currentTime,
                    End = slotEnd,
                    IsAvailable = !hasConflict,
                    Reason = hasConflict ? "Horário ocupado" : null,
                    ExistingAppointmentId = hasConflict ? 
                        existingAppointments.FirstOrDefault(a => currentTime >= a.StartTime && currentTime < a.EndTime)?.Id : null
                });

                currentTime = currentTime.AddMinutes(15); // Intervalos de 15 minutos
            }

            return slots;
        }

        private async Task<List<Appointment>> GetStaffAppointmentsForDay(int staffId, DateTime date)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    start_time as StartTime,
                    end_time as EndTime,
                    status as Status
                FROM appointments 
                WHERE staff_id = @StaffId 
                    AND DATE(start_time) = DATE(@Date)
                    AND status NOT IN ('cancelled', 'no_show')";

            using var connection = new NpgsqlConnection(_connectionString);
            return (await connection.QueryAsync<Appointment>(sql, new { StaffId = staffId, Date = date })).ToList();
        }

        public async Task<IEnumerable<AppointmentConflict>> CheckConflictsAsync(CreateAppointmentDto appointment)
        {
            var conflicts = new List<AppointmentConflict>();

            // Verificar conflito de horário da equipe
            var staffConflictSql = @"
                SELECT id, start_time, end_time 
                FROM appointments 
                WHERE staff_id = @StaffId 
                    AND status NOT IN ('cancelled', 'no_show')
                    AND (
                        (@StartTime >= start_time AND @StartTime < end_time) OR
                        (@EndTime > start_time AND @EndTime <= end_time) OR
                        (@StartTime <= start_time AND @EndTime >= end_time)
                    )";

            using var connection = new NpgsqlConnection(_connectionString);
            var staffConflicts = await connection.QueryAsync(staffConflictSql, new 
            { 
                appointment.StaffId, 
                appointment.StartTime, 
                appointment.EndTime 
            });

            foreach (var conflict in staffConflicts)
            {
                conflicts.Add(new AppointmentConflict
                {
                    ConflictTime = appointment.StartTime,
                    ConflictType = "staff_busy",
                    Description = "Profissional já possui agendamento neste horário",
                    ConflictingAppointmentId = conflict.id,
                    Suggestion = "Escolha outro horário ou profissional"
                });
            }

            // Verificar conflito de sala
            if (!string.IsNullOrEmpty(appointment.Room))
            {
                var roomConflictSql = @"
                    SELECT id FROM appointments 
                    WHERE room = @Room 
                        AND status NOT IN ('cancelled', 'no_show')
                        AND (
                            (@StartTime >= start_time AND @StartTime < end_time) OR
                            (@EndTime > start_time AND @EndTime <= end_time) OR
                            (@StartTime <= start_time AND @EndTime >= end_time)
                        )";

                var roomConflicts = await connection.QueryAsync(roomConflictSql, new 
                { 
                    appointment.Room, 
                    appointment.StartTime, 
                    appointment.EndTime 
                });

                foreach (var conflict in roomConflicts)
                {
                    conflicts.Add(new AppointmentConflict
                    {
                        ConflictTime = appointment.StartTime,
                        ConflictType = "room_occupied",
                        Description = $"Sala {appointment.Room} já está ocupada neste horário",
                        ConflictingAppointmentId = conflict.id,
                        Suggestion = "Escolha outra sala ou horário"
                    });
                }
            }

            return conflicts;
        }

        public async Task<bool> ValidateAppointmentSlotAsync(DateTime startTime, DateTime endTime, int staffId, int? excludeAppointmentId = null)
        {
            var sql = @"
                SELECT COUNT(*) FROM appointments 
                WHERE staff_id = @StaffId 
                    AND status NOT IN ('cancelled', 'no_show')
                    AND (@ExcludeId IS NULL OR id != @ExcludeId)
                    AND (
                        (@StartTime >= start_time AND @StartTime < end_time) OR
                        (@EndTime > start_time AND @EndTime <= end_time) OR
                        (@StartTime <= start_time AND @EndTime >= end_time)
                    )";

            using var connection = new NpgsqlConnection(_connectionString);
            var conflictCount = await connection.QuerySingleAsync<int>(sql, new 
            { 
                StaffId = staffId,
                StartTime = startTime,
                EndTime = endTime,
                ExcludeId = excludeAppointmentId
            });

            return conflictCount == 0;
        }

        public async Task<IEnumerable<Appointment>> CreateRecurringAppointmentsAsync(CreateAppointmentDto baseAppointment, RecurringAppointmentPattern pattern)
        {
            var appointments = new List<Appointment>();
            var currentDate = baseAppointment.StartTime.Date;
            var endDate = pattern.EndDate.Date;
            var occurrenceCount = 0;

            // Criar primeiro agendamento como pai
            var parentAppointment = await CreateAsync(baseAppointment);
            appointments.Add(parentAppointment);

            // Gerar agendamentos recorrentes
            while (currentDate <= endDate && (pattern.MaxOccurrences == null || occurrenceCount < pattern.MaxOccurrences))
            {
                currentDate = CalculateNextOccurrence(currentDate, pattern);
                if (currentDate > endDate) break;

                var duration = baseAppointment.EndTime - baseAppointment.StartTime;
                var newStartTime = currentDate.Add(baseAppointment.StartTime.TimeOfDay);
                var newEndTime = newStartTime.Add(duration);

                var recurringAppointment = new CreateAppointmentDto
                {
                    ClientId = baseAppointment.ClientId,
                    StaffId = baseAppointment.StaffId,
                    ServiceId = baseAppointment.ServiceId,
                    StartTime = newStartTime,
                    EndTime = newEndTime,
                    Notes = baseAppointment.Notes,
                    Room = baseAppointment.Room,
                    Priority = baseAppointment.Priority,
                    ReminderType = baseAppointment.ReminderType,
                    EstimatedCost = baseAppointment.EstimatedCost,
                    InsuranceProvider = baseAppointment.InsuranceProvider,
                    RecurrencePattern = $"PARENT:{parentAppointment.Id}"
                };

                var appointment = await CreateAsync(recurringAppointment);
                appointments.Add(appointment);
                occurrenceCount++;
            }

            return appointments;
        }

        private DateTime CalculateNextOccurrence(DateTime currentDate, RecurringAppointmentPattern pattern)
        {
            return pattern.Type.ToLower() switch
            {
                "daily" => currentDate.AddDays(pattern.Interval),
                "weekly" => currentDate.AddDays(7 * pattern.Interval),
                "monthly" => currentDate.AddMonths(pattern.Interval),
                _ => currentDate.AddDays(pattern.Interval)
            };
        }

        public async Task<IEnumerable<Appointment>> GetRecurringAppointmentsAsync(int parentId)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    client_id as ClientId,
                    staff_id as StaffId,
                    service_id as ServiceId,
                    start_time as StartTime,
                    end_time as EndTime,
                    status as Status,
                    notes as Notes,
                    room as Room,
                    recurrence_pattern as RecurrencePattern,
                    recurrence_parent_id as RecurrenceParentId,
                    priority as Priority,
                    reminder_type as ReminderType,
                    reminder_sent_at as ReminderSentAt,
                    is_blocked as IsBlocked,
                    block_reason as BlockReason,
                    estimated_cost as EstimatedCost,
                    insurance_provider as InsuranceProvider,
                    pre_auth_code as PreAuthCode,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM appointments 
                WHERE id = @ParentId OR recurrence_pattern = @Pattern
                ORDER BY start_time";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Appointment>(sql, new 
            { 
                ParentId = parentId, 
                Pattern = $"PARENT:{parentId}" 
            });
        }

        public async Task<bool> UpdateRecurringSeriesAsync(int parentId, CreateAppointmentDto updates, bool updateFutureOnly = false)
        {
            var whereClause = updateFutureOnly 
                ? "AND start_time >= @CurrentTime" 
                : "";

            var sql = $@"
                UPDATE appointments 
                SET 
                    notes = @Notes,
                    room = @Room,
                    priority = @Priority,
                    reminder_type = @ReminderType,
                    estimated_cost = @EstimatedCost,
                    insurance_provider = @InsuranceProvider,
                    updated_at = @UpdatedAt
                WHERE (id = @ParentId OR recurrence_pattern = @Pattern)
                    {whereClause}";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                ParentId = parentId,
                Pattern = $"PARENT:{parentId}",
                updates.Notes,
                updates.Room,
                updates.Priority,
                updates.ReminderType,
                updates.EstimatedCost,
                updates.InsuranceProvider,
                UpdatedAt = DateTime.UtcNow,
                CurrentTime = DateTime.Now
            });

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status, string? notes = null)
        {
            const string sql = @"
                UPDATE appointments 
                SET 
                    status = @Status,
                    notes = CASE WHEN @Notes IS NOT NULL THEN @Notes ELSE notes END,
                    updated_at = @UpdatedAt
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                Status = status,
                Notes = notes,
                UpdatedAt = DateTime.UtcNow
            });

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(string status, DateTime? date = null)
        {
            var whereClause = date.HasValue ? "AND DATE(start_time) = DATE(@Date)" : "";
            
            var sql = $@"
                SELECT 
                    id as Id,
                    client_id as ClientId,
                    staff_id as StaffId,
                    service_id as ServiceId,
                    start_time as StartTime,
                    end_time as EndTime,
                    status as Status,
                    notes as Notes,
                    room as Room,
                    recurrence_pattern as RecurrencePattern,
                    recurrence_parent_id as RecurrenceParentId,
                    priority as Priority,
                    reminder_type as ReminderType,
                    reminder_sent_at as ReminderSentAt,
                    is_blocked as IsBlocked,
                    block_reason as BlockReason,
                    estimated_cost as EstimatedCost,
                    insurance_provider as InsuranceProvider,
                    pre_auth_code as PreAuthCode,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM appointments 
                WHERE status = @Status {whereClause}
                ORDER BY start_time";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Appointment>(sql, new { Status = status, Date = date });
        }

        public async Task<bool> BulkUpdateStatusAsync(int[] appointmentIds, string status)
        {
            const string sql = @"
                UPDATE appointments 
                SET 
                    status = @Status,
                    updated_at = @UpdatedAt
                WHERE id = ANY(@Ids)";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Ids = appointmentIds,
                Status = status,
                UpdatedAt = DateTime.UtcNow
            });

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<AppointmentWithDetails>> GetAppointmentReportsAsync(
            DateTime? startDate = null, DateTime? endDate = null, string[]? statuses = null,
            int? professionalId = null, int? clientId = null, string? convenio = null,
            string? sala = null, int page = 1, int limit = 25)
        {
            var whereConditions = new List<string>();
            var parameters = new DynamicParameters();

            var sql = new StringBuilder(@"
                SELECT 
                    a.id as Id,
                    s.name as Procedimento,
                    CASE WHEN a.recurrence_pattern IS NOT NULL THEN 'Sim' ELSE 'Não' END as Recorrencia,
                    c.id as PacienteId,
                    c.full_name as PacienteNome,
                    CASE 
                        WHEN LENGTH(c.full_name) >= 3 THEN 
                            SUBSTRING(SPLIT_PART(c.full_name, ' ', 1), 1, 3) || '.772.070-84'
                        ELSE '315.772.070-84'
                    END as PacienteCpf,
                    c.phone as PacienteTelefone,
                    c.email as PacienteEmail,
                    st.id as ProfissionalId,
                    st.name as ProfissionalNome,
                    st.specialization as ProfissionalEspecialidade,
                    COALESCE(s.duration, 60) as Duracao,
                    a.start_time as Data,
                    TO_CHAR(a.start_time, 'DD/MM/YYYY HH24:MI:SS') as DataFormatada,
                    a.status as Status,
                    CASE a.status
                        WHEN 'scheduled' THEN 'Agendado'
                        WHEN 'confirmed' THEN 'Confirmado'
                        WHEN 'completed' THEN 'Concluído'
                        WHEN 'cancelled' THEN 'Cancelado'
                        WHEN 'no_show' THEN 'Não compareceu'
                        ELSE 'Agendado'
                    END as StatusLabel,
                    COALESCE(a.insurance_provider, 'Particular') as Convenio,
                    COALESCE(a.room, 'Sala 1') as Sala,
                    'CMD-' || LPAD(a.id::text, 4, '0') as Comanda,
                    COALESCE(s.price, 250.00) as Valor,
                    'R$ ' || TO_CHAR(COALESCE(s.price, 250.00), 'FM999,999.00') as ValorFormatado,
                    a.priority as Priority,
                    a.reminder_type as ReminderType,
                    CASE WHEN a.reminder_sent_at IS NOT NULL THEN true ELSE false END as HasReminder
                FROM appointments a
                INNER JOIN clients c ON a.client_id = c.id
                INNER JOIN staff st ON a.staff_id = st.id
                INNER JOIN services s ON a.service_id = s.id");

            // Aplicar filtros dinamicamente
            if (startDate.HasValue)
            {
                whereConditions.Add("a.start_time >= @StartDate");
                parameters.Add("StartDate", startDate.Value);
            }

            if (endDate.HasValue)
            {
                whereConditions.Add("a.start_time <= @EndDate");
                parameters.Add("EndDate", endDate.Value);
            }

            if (statuses != null && statuses.Length > 0)
            {
                whereConditions.Add("a.status = ANY(@Statuses)");
                parameters.Add("Statuses", statuses);
            }

            if (professionalId.HasValue)
            {
                whereConditions.Add("a.staff_id = @ProfessionalId");
                parameters.Add("ProfessionalId", professionalId.Value);
            }

            if (clientId.HasValue)
            {
                whereConditions.Add("a.client_id = @ClientId");
                parameters.Add("ClientId", clientId.Value);
            }

            if (!string.IsNullOrEmpty(convenio))
            {
                whereConditions.Add("a.insurance_provider = @Convenio");
                parameters.Add("Convenio", convenio);
            }

            if (!string.IsNullOrEmpty(sala))
            {
                whereConditions.Add("a.room = @Sala");
                parameters.Add("Sala", sala);
            }

            if (whereConditions.Any())
            {
                sql.Append(" WHERE " + string.Join(" AND ", whereConditions));
            }

            sql.Append(" ORDER BY a.start_time DESC");

            // Paginação
            var offset = (page - 1) * limit;
            sql.Append($" LIMIT {limit} OFFSET {offset}");

            using var connection = new NpgsqlConnection(_connectionString);
            var results = await connection.QueryAsync(sql.ToString(), parameters);

            return results.Select(r => new AppointmentWithDetails
            {
                Id = r.Id,
                Procedimento = r.Procedimento,
                Recorrencia = r.Recorrencia,
                Paciente = new ClientInfo
                {
                    Id = r.PacienteId,
                    Nome = r.PacienteNome,
                    Cpf = r.PacienteCpf,
                    Telefone = r.PacienteTelefone,
                    Email = r.PacienteEmail
                },
                Profissional = new ProfessionalInfo
                {
                    Id = r.ProfissionalId,
                    Nome = r.ProfissionalNome,
                    Especialidade = r.ProfissionalEspecialidade
                },
                Duracao = r.Duracao,
                Data = r.Data,
                DataFormatada = r.DataFormatada,
                Status = r.Status,
                StatusLabel = r.StatusLabel,
                Convenio = r.Convenio,
                Sala = r.Sala,
                Comanda = r.Comanda,
                Valor = r.Valor,
                ValorFormatado = r.ValorFormatado,
                Priority = r.Priority,
                ReminderType = r.ReminderType,
                HasReminder = r.HasReminder
            });
        }

        public async Task<int> GetAppointmentReportsCountAsync(
            DateTime? startDate = null, DateTime? endDate = null, string[]? statuses = null,
            int? professionalId = null, int? clientId = null, string? convenio = null, string? sala = null)
        {
            var whereConditions = new List<string>();
            var parameters = new DynamicParameters();

            var sql = new StringBuilder(@"
                SELECT COUNT(*)
                FROM appointments a
                INNER JOIN clients c ON a.client_id = c.id
                INNER JOIN staff st ON a.staff_id = st.id
                INNER JOIN services s ON a.service_id = s.id");

            // Aplicar os mesmos filtros
            if (startDate.HasValue)
            {
                whereConditions.Add("a.start_time >= @StartDate");
                parameters.Add("StartDate", startDate.Value);
            }

            if (endDate.HasValue)
            {
                whereConditions.Add("a.start_time <= @EndDate");
                parameters.Add("EndDate", endDate.Value);
            }

            if (statuses != null && statuses.Length > 0)
            {
                whereConditions.Add("a.status = ANY(@Statuses)");
                parameters.Add("Statuses", statuses);
            }

            if (professionalId.HasValue)
            {
                whereConditions.Add("a.staff_id = @ProfessionalId");
                parameters.Add("ProfessionalId", professionalId.Value);
            }

            if (clientId.HasValue)
            {
                whereConditions.Add("a.client_id = @ClientId");
                parameters.Add("ClientId", clientId.Value);
            }

            if (!string.IsNullOrEmpty(convenio))
            {
                whereConditions.Add("a.insurance_provider = @Convenio");
                parameters.Add("Convenio", convenio);
            }

            if (!string.IsNullOrEmpty(sala))
            {
                whereConditions.Add("a.room = @Sala");
                parameters.Add("Sala", sala);
            }

            if (whereConditions.Any())
            {
                sql.Append(" WHERE " + string.Join(" AND ", whereConditions));
            }

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleAsync<int>(sql.ToString(), parameters);
        }

        public async Task<AppointmentStatistics> GetAppointmentStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var dates = NormalizeDateRange(startDate, endDate);
            
            var sql = @"
                SELECT 
                    COUNT(*) as TotalAppointments,
                    COUNT(CASE WHEN DATE(start_time) = CURRENT_DATE THEN 1 END) as TodayAppointments,
                    COUNT(CASE WHEN start_time >= DATE_TRUNC('week', CURRENT_DATE) 
                        AND start_time < DATE_TRUNC('week', CURRENT_DATE) + INTERVAL '1 week' THEN 1 END) as WeekAppointments,
                    COUNT(CASE WHEN start_time >= DATE_TRUNC('month', CURRENT_DATE) 
                        AND start_time < DATE_TRUNC('month', CURRENT_DATE) + INTERVAL '1 month' THEN 1 END) as MonthAppointments,
                    COUNT(CASE WHEN status = 'completed' THEN 1 END) as CompletedAppointments,
                    COUNT(CASE WHEN status = 'cancelled' THEN 1 END) as CancelledAppointments,
                    COUNT(CASE WHEN status = 'no_show' THEN 1 END) as NoShowAppointments
                FROM appointments 
                WHERE start_time >= @StartDate AND start_time <= @EndDate";

            using var connection = new NpgsqlConnection(_connectionString);
            var result = await connection.QuerySingleAsync(sql, new { StartDate = dates.start, EndDate = dates.end });

            var statistics = new AppointmentStatistics
            {
                TotalAppointments = result.TotalAppointments,
                TodayAppointments = result.TodayAppointments,
                WeekAppointments = result.WeekAppointments,
                MonthAppointments = result.MonthAppointments,
                CompletedAppointments = result.CompletedAppointments,
                CancelledAppointments = result.CancelledAppointments,
                NoShowAppointments = result.NoShowAppointments
            };

            // Calcular percentuais
            if (statistics.TotalAppointments > 0)
            {
                statistics.CompletionRate = (decimal)statistics.CompletedAppointments / statistics.TotalAppointments * 100;
                statistics.CancellationRate = (decimal)statistics.CancelledAppointments / statistics.TotalAppointments * 100;
                statistics.NoShowRate = (decimal)statistics.NoShowAppointments / statistics.TotalAppointments * 100;
            }

            // Buscar breakdown por status
            statistics.StatusBreakdown = await GetStatusBreakdown(dates.start, dates.end);
            statistics.BusiestHours = await GetBusiestHours(dates.start, dates.end);
            statistics.WeeklyTrend = await GetWeeklyTrend(dates.start, dates.end);

            return statistics;
        }

        private async Task<List<StatusCount>> GetStatusBreakdown(DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT 
                    status as Status,
                    CASE status
                        WHEN 'scheduled' THEN 'Agendado'
                        WHEN 'confirmed' THEN 'Confirmado'
                        WHEN 'completed' THEN 'Concluído'
                        WHEN 'cancelled' THEN 'Cancelado'
                        WHEN 'no_show' THEN 'Não compareceu'
                        ELSE 'Outros'
                    END as StatusLabel,
                    COUNT(*) as Count
                FROM appointments 
                WHERE start_time >= @StartDate AND start_time <= @EndDate
                GROUP BY status
                ORDER BY Count DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            var breakdown = await connection.QueryAsync(sql, new { StartDate = startDate, EndDate = endDate });
            
            var total = breakdown.Sum(b => b.Count);
            
            return breakdown.Select(b => new StatusCount
            {
                Status = b.Status,
                StatusLabel = b.StatusLabel,
                Count = b.Count,
                Percentage = total > 0 ? (decimal)b.Count / total * 100 : 0
            }).ToList();
        }

        private async Task<List<HourlyCount>> GetBusiestHours(DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT 
                    EXTRACT(HOUR FROM start_time) as Hour,
                    COUNT(*) as Count
                FROM appointments 
                WHERE start_time >= @StartDate AND start_time <= @EndDate
                GROUP BY EXTRACT(HOUR FROM start_time)
                ORDER BY Count DESC
                LIMIT 10";

            using var connection = new NpgsqlConnection(_connectionString);
            var hours = await connection.QueryAsync(sql, new { StartDate = startDate, EndDate = endDate });
            
            return hours.Select(h => new HourlyCount
            {
                Hour = (int)h.Hour,
                Count = h.Count,
                TimeRange = $"{h.Hour:00}:00 - {h.Hour:00}:59"
            }).ToList();
        }

        private async Task<List<DailyCount>> GetWeeklyTrend(DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT 
                    DATE(start_time) as Date,
                    COUNT(*) as Count
                FROM appointments 
                WHERE start_time >= @StartDate AND start_time <= @EndDate
                GROUP BY DATE(start_time)
                ORDER BY Date";

            using var connection = new NpgsqlConnection(_connectionString);
            var daily = await connection.QueryAsync(sql, new { StartDate = startDate, EndDate = endDate });
            
            return daily.Select(d => new DailyCount
            {
                Date = d.Date,
                Count = d.Count,
                DayName = ((DateTime)d.Date).ToString("dddd", new System.Globalization.CultureInfo("pt-BR"))
            }).ToList();
        }

        public async Task<object> GetDashboardMetricsAsync(DateTime? date = null)
        {
            var targetDate = date ?? DateTime.Today;
            var startOfWeek = targetDate.AddDays(-(int)targetDate.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            var sql = @"
                SELECT 
                    COUNT(CASE WHEN DATE(start_time) = @TargetDate THEN 1 END) as TodayTotal,
                    COUNT(CASE WHEN DATE(start_time) = @TargetDate AND status = 'completed' THEN 1 END) as TodayCompleted,
                    COUNT(CASE WHEN start_time >= @StartOfWeek AND start_time < @EndOfWeek THEN 1 END) as WeekTotal,
                    COUNT(CASE WHEN start_time >= @StartOfWeek AND start_time < @EndOfWeek AND status = 'completed' THEN 1 END) as WeekCompleted,
                    COUNT(CASE WHEN start_time >= @TargetDate AND start_time < @TargetDate + INTERVAL '1 day' 
                        AND status IN ('scheduled', 'confirmed') THEN 1 END) as TodayPending
                FROM appointments";

            using var connection = new NpgsqlConnection(_connectionString);
            var metrics = await connection.QuerySingleAsync(sql, new 
            { 
                TargetDate = targetDate, 
                StartOfWeek = startOfWeek, 
                EndOfWeek = endOfWeek 
            });

            return new
            {
                today = new
                {
                    total = metrics.TodayTotal,
                    completed = metrics.TodayCompleted,
                    pending = metrics.TodayPending,
                    completionRate = metrics.TodayTotal > 0 ? (decimal)metrics.TodayCompleted / metrics.TodayTotal * 100 : 0
                },
                week = new
                {
                    total = metrics.WeekTotal,
                    completed = metrics.WeekCompleted,
                    completionRate = metrics.WeekTotal > 0 ? (decimal)metrics.WeekCompleted / metrics.WeekTotal * 100 : 0
                },
                generatedAt = DateTime.UtcNow
            };
        }

        public async Task<object> GetUtilizationReportAsync(DateTime startDate, DateTime endDate)
        {
            var sql = @"
                SELECT 
                    st.name as StaffName,
                    COUNT(a.id) as TotalAppointments,
                    COUNT(CASE WHEN a.status = 'completed' THEN 1 END) as CompletedAppointments,
                    SUM(EXTRACT(EPOCH FROM (a.end_time - a.start_time)) / 3600) as TotalHours,
                    AVG(EXTRACT(EPOCH FROM (a.end_time - a.start_time)) / 60) as AvgDurationMinutes
                FROM staff st
                LEFT JOIN appointments a ON st.id = a.staff_id 
                    AND a.start_time >= @StartDate 
                    AND a.start_time <= @EndDate
                WHERE st.is_active = true
                GROUP BY st.id, st.name
                ORDER BY TotalAppointments DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            var utilization = await connection.QueryAsync(sql, new { StartDate = startDate, EndDate = endDate });

            return new
            {
                period = new { startDate, endDate },
                staffUtilization = utilization.Select(u => new
                {
                    staffName = u.StaffName,
                    totalAppointments = u.TotalAppointments,
                    completedAppointments = u.CompletedAppointments,
                    totalHours = Math.Round(u.TotalHours ?? 0, 2),
                    avgDurationMinutes = Math.Round(u.AvgDurationMinutes ?? 0, 2),
                    utilizationRate = u.TotalAppointments > 0 ? (decimal)u.CompletedAppointments / u.TotalAppointments * 100 : 0
                }),
                summary = new
                {
                    totalStaff = utilization.Count(),
                    totalAppointments = utilization.Sum(u => u.TotalAppointments),
                    avgUtilization = utilization.Any() ? utilization.Average(u => u.TotalAppointments > 0 ? (decimal)u.CompletedAppointments / u.TotalAppointments * 100 : 0) : 0
                }
            };
        }

        public async Task<IEnumerable<AppointmentReminder>> GetPendingRemindersAsync()
        {
            const string sql = @"
                SELECT 
                    r.id as Id,
                    r.appointment_id as AppointmentId,
                    r.type as Type,
                    r.status as Status,
                    r.scheduled_for as ScheduledFor,
                    r.sent_at as SentAt,
                    r.message as Message,
                    r.error_message as ErrorMessage
                FROM appointment_reminders r
                WHERE r.status = 'pending' 
                    AND r.scheduled_for <= @Now
                ORDER BY r.scheduled_for";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<AppointmentReminder>(sql, new { Now = DateTime.UtcNow });
        }

        public async Task<AppointmentReminder> CreateReminderAsync(int appointmentId, string type, DateTime scheduledFor)
        {
            const string sql = @"
                INSERT INTO appointment_reminders 
                (appointment_id, type, status, scheduled_for, message)
                VALUES 
                (@AppointmentId, @Type, 'pending', @ScheduledFor, @Message)
                RETURNING 
                    id as Id,
                    appointment_id as AppointmentId,
                    type as Type,
                    status as Status,
                    scheduled_for as ScheduledFor,
                    sent_at as SentAt,
                    message as Message,
                    error_message as ErrorMessage";

            var message = GenerateReminderMessage(type);

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleAsync<AppointmentReminder>(sql, new
            {
                AppointmentId = appointmentId,
                Type = type,
                ScheduledFor = scheduledFor,
                Message = message
            });
        }

        private string GenerateReminderMessage(string type)
        {
            return type.ToLower() switch
            {
                "sms" => "Lembrete: Você tem um agendamento amanhã na clínica. Confirme sua presença.",
                "email" => "Prezado(a) paciente, este é um lembrete do seu agendamento na nossa clínica.",
                "whatsapp" => "🦷 Lembrete: Seu agendamento na clínica está marcado para amanhã!",
                _ => "Lembrete de agendamento"
            };
        }

        public async Task<bool> MarkReminderSentAsync(int reminderId)
        {
            const string sql = @"
                UPDATE appointment_reminders 
                SET 
                    status = 'sent',
                    sent_at = @SentAt
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = reminderId,
                SentAt = DateTime.UtcNow
            });

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<WorkingHours>> GetStaffWorkingHoursAsync(int staffId)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    staff_id as StaffId,
                    day_of_week as DayOfWeek,
                    start_time as StartTime,
                    end_time as EndTime,
                    is_working_day as IsWorkingDay
                FROM working_hours 
                WHERE staff_id = @StaffId
                ORDER BY day_of_week";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<WorkingHours>(sql, new { StaffId = staffId });
        }

        public async Task<bool> UpdateWorkingHoursAsync(int staffId, List<WorkingHours> workingHours)
        {
            // Deletar horários existentes
            const string deleteSql = "DELETE FROM working_hours WHERE staff_id = @StaffId";
            
            // Inserir novos horários
            const string insertSql = @"
                INSERT INTO working_hours 
                (staff_id, day_of_week, start_time, end_time, is_working_day)
                VALUES 
                (@StaffId, @DayOfWeek, @StartTime, @EndTime, @IsWorkingDay)";

            using var connection = new NpgsqlConnection(_connectionString);
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await connection.ExecuteAsync(deleteSql, new { StaffId = staffId }, transaction);
                
                foreach (var wh in workingHours)
                {
                    await connection.ExecuteAsync(insertSql, new
                    {
                        StaffId = staffId,
                        wh.DayOfWeek,
                        wh.StartTime,
                        wh.EndTime,
                        wh.IsWorkingDay
                    }, transaction);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime)
        {
            const string sql = @"
                SELECT DISTINCT room 
                FROM appointments 
                WHERE room IS NOT NULL
                    AND NOT (
                        (@StartTime >= start_time AND @StartTime < end_time) OR
                        (@EndTime > start_time AND @EndTime <= end_time) OR
                        (@StartTime <= start_time AND @EndTime >= end_time)
                    )
                    AND status NOT IN ('cancelled', 'no_show')
                UNION
                SELECT 'Sala 1' WHERE NOT EXISTS (
                    SELECT 1 FROM appointments 
                    WHERE room = 'Sala 1'
                        AND (
                            (@StartTime >= start_time AND @StartTime < end_time) OR
                            (@EndTime > start_time AND @EndTime <= end_time) OR
                            (@StartTime <= start_time AND @EndTime >= end_time)
                        )
                        AND status NOT IN ('cancelled', 'no_show')
                )
                UNION
                SELECT 'Sala 2' WHERE NOT EXISTS (
                    SELECT 1 FROM appointments 
                    WHERE room = 'Sala 2'
                        AND (
                            (@StartTime >= start_time AND @StartTime < end_time) OR
                            (@EndTime > start_time AND @EndTime <= end_time) OR
                            (@StartTime <= start_time AND @EndTime >= end_time)
                        )
                        AND status NOT IN ('cancelled', 'no_show')
                )
                ORDER BY room";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<string>(sql, new { StartTime = startTime, EndTime = endTime });
        }

        public async Task<object> GetRoomUtilizationAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1).AddTicks(-1);

            const string sql = @"
                SELECT 
                    COALESCE(room, 'Sala 1') as Room,
                    COUNT(*) as TotalAppointments,
                    SUM(EXTRACT(EPOCH FROM (end_time - start_time)) / 3600) as TotalHours,
                    MIN(start_time) as FirstAppointment,
                    MAX(end_time) as LastAppointment
                FROM appointments 
                WHERE start_time >= @StartDate 
                    AND start_time <= @EndDate
                    AND status NOT IN ('cancelled', 'no_show')
                GROUP BY COALESCE(room, 'Sala 1')
                ORDER BY Room";

            using var connection = new NpgsqlConnection(_connectionString);
            var utilization = await connection.QueryAsync(sql, new { StartDate = startDate, EndDate = endDate });

            return new
            {
                date,
                roomUtilization = utilization.Select(r => new
                {
                    room = r.Room,
                    totalAppointments = r.TotalAppointments,
                    totalHours = Math.Round(r.TotalHours ?? 0, 2),
                    utilizationRate = Math.Round((r.TotalHours ?? 0) / 10 * 100, 2), // Assumindo 10h de operação
                    firstAppointment = r.FirstAppointment,
                    lastAppointment = r.LastAppointment
                }),
                summary = new
                {
                    totalRooms = utilization.Count(),
                    avgUtilization = utilization.Any() ? utilization.Average(r => (r.TotalHours ?? 0) / 10 * 100) : 0
                }
            };
        }

        private (DateTime start, DateTime end) NormalizeDateRange(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.Now.Date.AddDays(1).AddTicks(-1);
            var start = startDate ?? end.AddDays(-30);
            return (start, end);
        }
    }
}