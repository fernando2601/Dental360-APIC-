namespace DentalSpa.Domain.Entities
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
        public decimal Price { get; set; }
        public string? PaymentStatus { get; set; } = "pending";
        public string? PaymentMethod { get; set; }
        public bool IsRecurring { get; set; } = false;
        public string? RecurrencePattern { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        public int? ParentAppointmentId { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ClientFeedback { get; set; }
        public int? Rating { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ClinicId { get; set; }
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
    }

    public class ClientInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ProfessionalInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;
    }

    // Models específicos para funcionalidades da Agenda
    public class AppointmentCalendar
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
        public decimal Price { get; set; }
        public string? Notes { get; set; }
        public bool IsRecurring { get; set; }
    }

    public class AppointmentScheduling
    {
        public DateTime Date { get; set; }
        public List<TimeSlot> AvailableSlots { get; set; } = new();
        public List<AppointmentCalendar> ExistingAppointments { get; set; } = new();
        public StaffAvailability StaffAvailability { get; set; } = new();
    }

    public class TimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? UnavailableReason { get; set; }
        public int? StaffId { get; set; }
        public string? Room { get; set; }
    }

    public class StaffAvailability
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public List<AvailabilitySlot> Slots { get; set; } = new();
        public WorkingHours WorkingHours { get; set; } = new();
    }

    public class AvailabilitySlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? Reason { get; set; }
    }

    public class WorkingHours
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<DayOfWeek> WorkingDays { get; set; } = new();
        public List<TimeSpan> BreakTimes { get; set; } = new();
    }

    public class AppointmentConflict
    {
        public int AppointmentId { get; set; }
        public string ConflictType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ConflictDateTime { get; set; }
        public List<string> Suggestions { get; set; } = new();
    }

    public class AppointmentReminder
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Type { get; set; } = string.Empty; // sms, email, whatsapp
        public DateTime ScheduledTime { get; set; }
        public string Status { get; set; } = "pending";
        public string? Message { get; set; }
        public DateTime? SentAt { get; set; }
    }

    public class AppointmentStatistics
    {
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal CancellationRate { get; set; }
        public decimal NoShowRate { get; set; }
        public decimal AverageRating { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<AppointmentsByDay> AppointmentsByDay { get; set; } = new();
        public List<AppointmentsByStaff> AppointmentsByStaff { get; set; } = new();
        public List<AppointmentsByService> AppointmentsByService { get; set; } = new();
    }

    public class AppointmentsByDay
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }

    public class AppointmentsByStaff
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class AppointmentsByService
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class RecurringAppointmentTemplate
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string RecurrencePattern { get; set; } = string.Empty; // daily, weekly, monthly
        public int RecurrenceInterval { get; set; } = 1;
        public List<DayOfWeek> RecurrenceDays { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxOccurrences { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }

    public class WaitingList
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }
        public DateTime PreferredDate { get; set; }
        public TimeSpan? PreferredTime { get; set; }
        public string Priority { get; set; } = "normal"; // low, normal, high, urgent
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsNotified { get; set; } = false;
    }
}