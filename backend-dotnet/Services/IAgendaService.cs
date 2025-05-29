using ClinicApi.Models;

namespace ClinicApi.Services
{
    public interface IAgendaService
    {
        // CRUD Básico
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto appointmentDto);
        Task<Appointment?> UpdateAppointmentAsync(int id, CreateAppointmentDto appointmentDto);
        Task<bool> DeleteAppointmentAsync(int id);

        // Calendar Views
        Task<object> GetCalendarViewAsync(DateTime startDate, DateTime endDate, int? staffId = null, string viewType = "month");
        Task<object> GetDayViewAsync(DateTime date, int? staffId = null);
        Task<object> GetWeekViewAsync(DateTime weekStart, int? staffId = null);
        Task<object> GetMonthViewAsync(DateTime monthStart, int? staffId = null);

        // Agenda Management
        Task<ScheduleOverview> GetScheduleOverviewAsync(DateTime date);
        Task<object> GetStaffAvailabilityAsync(DateTime date, int? staffId = null);
        Task<object> GetAvailableSlotsAsync(DateTime date, int staffId, int serviceDuration);

        // Smart Scheduling
        Task<object> FindBestAvailableSlotAsync(int staffId, int serviceId, DateTime preferredDate, int durationMinutes);
        Task<object> SuggestAlternativeSlotsAsync(CreateAppointmentDto appointment);
        Task<bool> ValidateAppointmentAsync(CreateAppointmentDto appointment);

        // Recurring Appointments
        Task<object> CreateRecurringAppointmentsAsync(CreateAppointmentDto baseAppointment, RecurringAppointmentPattern pattern);
        Task<object> GetRecurringSeriesAsync(int parentId);
        Task<bool> UpdateRecurringSeriesAsync(int parentId, CreateAppointmentDto updates, bool updateFutureOnly = false);

        // Status Management
        Task<bool> UpdateAppointmentStatusAsync(int id, string status, string? notes = null);
        Task<object> GetAppointmentsByStatusAsync(string status, DateTime? date = null);
        Task<bool> BulkUpdateStatusAsync(int[] appointmentIds, string status);

        // Search & Reports
        Task<object> GetAppointmentReportsAsync(DateTime? startDate, DateTime? endDate, string[]? statuses, 
            int? professionalId, int? clientId, string? convenio, string? sala, int page, int limit);

        // Statistics & Analytics
        Task<AppointmentStatistics> GetAppointmentStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetDashboardMetricsAsync(DateTime? date = null);
        Task<object> GetUtilizationReportAsync(DateTime startDate, DateTime endDate);
        Task<object> GetPerformanceAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Reminders & Notifications
        Task<object> GetPendingRemindersAsync();
        Task<bool> CreateReminderAsync(int appointmentId, string type, DateTime? scheduledFor = null);
        Task<bool> SendRemindersAsync();
        Task<object> GetReminderSettingsAsync();

        // Working Hours & Availability
        Task<object> GetStaffWorkingHoursAsync(int staffId);
        Task<bool> UpdateWorkingHoursAsync(int staffId, List<WorkingHours> workingHours);
        Task<object> GetStaffScheduleAsync(int staffId, DateTime startDate, DateTime endDate);

        // Room Management
        Task<object> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime);
        Task<object> GetRoomUtilizationAsync(DateTime date);
        Task<object> GetRoomScheduleAsync(string room, DateTime date);

        // Conflict Detection & Resolution
        Task<object> CheckConflictsAsync(CreateAppointmentDto appointment);
        Task<object> ResolveSchedulingConflictsAsync(int appointmentId);

        // Export & Integration
        Task<object> ExportScheduleAsync(DateTime startDate, DateTime endDate, string format = "pdf");
        Task<object> ImportAppointmentsAsync(object importData);
    }
}