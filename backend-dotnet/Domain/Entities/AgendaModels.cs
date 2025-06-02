namespace ClinicApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? ServiceId { get; set; }
        public int? StaffId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "pending"; // pending, confirmed, in_progress, completed, cancelled, no_show
        public string? Notes { get; set; }
        public string? Room { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public int? CancelledBy { get; set; }
        public string Priority { get; set; } = "normal"; // low, normal, high, urgent
        public bool IsRecurring { get; set; } = false;
        public string? RecurrencePattern { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        public int? ParentAppointmentId { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties (will be filled by joins)
        public string? PatientName { get; set; }
        public string? PatientPhone { get; set; }
        public string? PatientEmail { get; set; }
        public string? ServiceName { get; set; }
        public string? StaffName { get; set; }
        public string? CreatedByName { get; set; }
    }

    public class CreateAppointmentDto
    {
        public int PatientId { get; set; }
        public int? ServiceId { get; set; }
        public int? StaffId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public string? Room { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string Priority { get; set; } = "normal";
        public bool IsRecurring { get; set; } = false;
        public string? RecurrencePattern { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
    }

    public class UpdateAppointmentDto
    {
        public int? ServiceId { get; set; }
        public int? StaffId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public string? Room { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public string? Priority { get; set; }
    }

    public class AppointmentCalendarView
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string? ServiceName { get; set; }
        public string? StaffName { get; set; }
        public string? Room { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string PriorityColor { get; set; } = string.Empty;
        public bool AllDay { get; set; } = false;
        public string? Notes { get; set; }
        public decimal? EstimatedCost { get; set; }
    }

    public class AvailabilitySlot
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAvailable { get; set; }
        public int? StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? Room { get; set; }
        public string? ConflictReason { get; set; }
    }

    public class AgendaStatistics
    {
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int WeekAppointments { get; set; }
        public int MonthAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal CancellationRate { get; set; }
        public decimal NoShowRate { get; set; }
        public decimal AverageAppointmentValue { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal EstimatedRevenue { get; set; }
        public int MostBookedService { get; set; }
        public string? MostBookedServiceName { get; set; }
        public int MostRequestedStaff { get; set; }
        public string? MostRequestedStaffName { get; set; }
        public List<HourlyStats> HourlyDistribution { get; set; } = new();
        public List<DailyStats> WeeklyDistribution { get; set; } = new();
        public List<StatusStats> StatusDistribution { get; set; } = new();
    }

    public class HourlyStats
    {
        public int Hour { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DailyStats
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public decimal Revenue { get; set; }
    }

    public class StatusStats
    {
        public string Status { get; set; } = string.Empty;
        public string StatusLabel { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class AppointmentConflict
    {
        public int AppointmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ConflictType { get; set; } = string.Empty; // staff_conflict, room_conflict, patient_conflict
        public string ConflictDescription { get; set; } = string.Empty;
        public int? ConflictingAppointmentId { get; set; }
        public string? ConflictingPatientName { get; set; }
        public string? ConflictingStaffName { get; set; }
        public string? ConflictingRoom { get; set; }
    }

    public class RescheduleRequest
    {
        public int AppointmentId { get; set; }
        public DateTime NewStartTime { get; set; }
        public DateTime NewEndTime { get; set; }
        public int? NewStaffId { get; set; }
        public string? NewRoom { get; set; }
        public string? Reason { get; set; }
        public bool NotifyPatient { get; set; } = true;
    }

    public class BulkAppointmentAction
    {
        public int[] AppointmentIds { get; set; } = Array.Empty<int>();
        public string Action { get; set; } = string.Empty; // confirm, cancel, reschedule, complete
        public string? Reason { get; set; }
        public DateTime? NewDateTime { get; set; }
        public bool NotifyPatients { get; set; } = true;
    }

    public class WorkingHours
    {
        public int Id { get; set; }
        public int? StaffId { get; set; } // null para horário geral da clínica
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class BlockedTimeSlot
    {
        public int Id { get; set; }
        public int? StaffId { get; set; }
        public string? Room { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // vacation, meeting, maintenance, holiday
        public bool IsRecurring { get; set; } = false;
        public string? RecurrencePattern { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
    }

    public class AppointmentReminder
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Type { get; set; } = string.Empty; // sms, email, whatsapp
        public DateTime ScheduledFor { get; set; }
        public bool IsSent { get; set; } = false;
        public DateTime? SentAt { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; } = 0;
    }

    public class AgendaFilters
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? PatientId { get; set; }
        public int? StaffId { get; set; }
        public int? ServiceId { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Room { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 50;
        public string SortBy { get; set; } = "start_time";
        public string SortOrder { get; set; } = "asc";
    }

    public class TimeSlotAvailability
    {
        public DateTime DateTime { get; set; }
        public bool IsAvailable { get; set; }
        public List<AvailableStaff> AvailableStaff { get; set; } = new();
        public List<string> AvailableRooms { get; set; } = new();
        public string? UnavailabilityReason { get; set; }
    }

    public class AvailableStaff
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string? UnavailabilityReason { get; set; }
    }
}