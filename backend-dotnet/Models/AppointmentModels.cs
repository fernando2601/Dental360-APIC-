namespace ClinicApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "scheduled";
        public string? Notes { get; set; }
        public string? Room { get; set; }
        public string? RecurrencePattern { get; set; }
        public int? RecurrenceParentId { get; set; }
        public string Priority { get; set; } = "normal"; // low, normal, high, urgent
        public string? ReminderType { get; set; } // sms, email, whatsapp
        public DateTime? ReminderSentAt { get; set; }
        public bool IsBlocked { get; set; } = false;
        public string? BlockReason { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? PreAuthCode { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AppointmentCalendarView
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool AllDay { get; set; } = false;
        public string? Notes { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string? InsuranceProvider { get; set; }
    }

    public class AppointmentWithDetails
    {
        public int Id { get; set; }
        public string Procedimento { get; set; } = string.Empty;
        public string? Recorrencia { get; set; }
        public ClientInfo Paciente { get; set; } = new();
        public ProfessionalInfo Profissional { get; set; } = new();
        public int Duracao { get; set; }
        public DateTime Data { get; set; }
        public string DataFormatada { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusLabel { get; set; } = string.Empty;
        public string Convenio { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public string Comanda { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string ValorFormatado { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? ReminderType { get; set; }
        public bool HasReminder { get; set; }
    }

    public class ClientInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public string? Address { get; set; }
    }

    public class ProfessionalInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    public class CreateAppointmentDto
    {
        public int ClientId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public string? Room { get; set; }
        public string? RecurrencePattern { get; set; }
        public string Priority { get; set; } = "normal";
        public string? ReminderType { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? PreAuthCode { get; set; }
    }

    public class AppointmentSlot
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAvailable { get; set; }
        public string? Reason { get; set; }
        public int? ExistingAppointmentId { get; set; }
    }

    public class StaffAvailability
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? UnavailabilityReason { get; set; }
        public List<AppointmentSlot> AvailableSlots { get; set; } = new();
    }

    public class AppointmentConflict
    {
        public DateTime ConflictTime { get; set; }
        public string ConflictType { get; set; } = string.Empty; // staff_busy, room_occupied, outside_hours
        public string Description { get; set; } = string.Empty;
        public int? ConflictingAppointmentId { get; set; }
        public string? Suggestion { get; set; }
    }

    public class RecurringAppointmentPattern
    {
        public string Type { get; set; } = string.Empty; // daily, weekly, monthly
        public int Interval { get; set; } = 1;
        public List<DayOfWeek>? DaysOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxOccurrences { get; set; }
    }

    public class AppointmentStatistics
    {
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int WeekAppointments { get; set; }
        public int MonthAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal CancellationRate { get; set; }
        public decimal NoShowRate { get; set; }
        public List<StatusCount> StatusBreakdown { get; set; } = new();
        public List<HourlyCount> BusiestHours { get; set; } = new();
        public List<DailyCount> WeeklyTrend { get; set; } = new();
    }

    public class StatusCount
    {
        public string Status { get; set; } = string.Empty;
        public string StatusLabel { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class HourlyCount
    {
        public int Hour { get; set; }
        public int Count { get; set; }
        public string TimeRange { get; set; } = string.Empty;
    }

    public class DailyCount
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string DayName { get; set; } = string.Empty;
    }

    public class AppointmentReminder
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Type { get; set; } = string.Empty; // sms, email, whatsapp
        public string Status { get; set; } = string.Empty; // pending, sent, failed
        public DateTime ScheduledFor { get; set; }
        public DateTime? SentAt { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class WorkingHours
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsWorkingDay { get; set; } = true;
    }

    public class TimeSlotRequest
    {
        public int StaffId { get; set; }
        public DateTime Date { get; set; }
        public int ServiceDuration { get; set; }
    }

    public class ScheduleOverview
    {
        public DateTime Date { get; set; }
        public List<StaffSchedule> StaffSchedules { get; set; } = new();
        public List<RoomSchedule> RoomSchedules { get; set; } = new();
        public int TotalAppointments { get; set; }
        public int AvailableSlots { get; set; }
    }

    public class StaffSchedule
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public List<AppointmentCalendarView> Appointments { get; set; } = new();
        public List<AppointmentSlot> AvailableSlots { get; set; } = new();
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class RoomSchedule
    {
        public string Room { get; set; } = string.Empty;
        public List<AppointmentCalendarView> Appointments { get; set; } = new();
        public List<TimeSpan> AvailableSlots { get; set; } = new();
    }
}