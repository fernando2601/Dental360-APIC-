using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IAgendaRepository
    {
        // CRUD Básico
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> CreateAsync(CreateAppointmentDto appointment);
        Task<Appointment?> UpdateAsync(int id, CreateAppointmentDto appointment);
        Task<bool> DeleteAsync(int id);

        // Calendar Views
        Task<IEnumerable<AppointmentCalendarView>> GetCalendarViewAsync(DateTime startDate, DateTime endDate, int? staffId = null);
        Task<IEnumerable<AppointmentCalendarView>> GetDayViewAsync(DateTime date, int? staffId = null);
        Task<IEnumerable<AppointmentCalendarView>> GetWeekViewAsync(DateTime weekStart, int? staffId = null);
        Task<IEnumerable<AppointmentCalendarView>> GetMonthViewAsync(DateTime monthStart, int? staffId = null);

        // Agenda Management
        Task<ScheduleOverview> GetScheduleOverviewAsync(DateTime date);
        Task<IEnumerable<StaffAvailability>> GetStaffAvailabilityAsync(DateTime date, int? staffId = null);
        Task<IEnumerable<AppointmentSlot>> GetAvailableSlotsAsync(DateTime date, int staffId, int serviceDuration);

        // Conflicts & Validation
        Task<IEnumerable<AppointmentConflict>> CheckConflictsAsync(CreateAppointmentDto appointment);
        Task<bool> ValidateAppointmentSlotAsync(DateTime startTime, DateTime endTime, int staffId, int? excludeAppointmentId = null);

        // Recurring Appointments
        Task<IEnumerable<Appointment>> CreateRecurringAppointmentsAsync(CreateAppointmentDto baseAppointment, RecurringAppointmentPattern pattern);
        Task<IEnumerable<Appointment>> GetRecurringAppointmentsAsync(int parentId);
        Task<bool> UpdateRecurringSeriesAsync(int parentId, CreateAppointmentDto updates, bool updateFutureOnly = false);

        // Status Management
        Task<bool> UpdateAppointmentStatusAsync(int id, string status, string? notes = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(string status, DateTime? date = null);
        Task<bool> BulkUpdateStatusAsync(int[] appointmentIds, string status);

        // Search & Filters
        Task<IEnumerable<AppointmentWithDetails>> GetAppointmentReportsAsync(
            DateTime? startDate = null, DateTime? endDate = null, string[]? statuses = null,
            int? professionalId = null, int? clientId = null, string? convenio = null,
            string? sala = null, int page = 1, int limit = 25);
        Task<int> GetAppointmentReportsCountAsync(
            DateTime? startDate = null, DateTime? endDate = null, string[]? statuses = null,
            int? professionalId = null, int? clientId = null, string? convenio = null, string? sala = null);

        // Statistics & Analytics
        Task<AppointmentStatistics> GetAppointmentStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetDashboardMetricsAsync(DateTime? date = null);
        Task<object> GetUtilizationReportAsync(DateTime startDate, DateTime endDate);

        // Reminders
        Task<IEnumerable<AppointmentReminder>> GetPendingRemindersAsync();
        Task<AppointmentReminder> CreateReminderAsync(int appointmentId, string type, DateTime scheduledFor);
        Task<bool> MarkReminderSentAsync(int reminderId);

        // Working Hours
        Task<IEnumerable<WorkingHours>> GetStaffWorkingHoursAsync(int staffId);
        Task<bool> UpdateWorkingHoursAsync(int staffId, List<WorkingHours> workingHours);

        // Room Management
        Task<IEnumerable<string>> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime);
        Task<object> GetRoomUtilizationAsync(DateTime date);
    }
}