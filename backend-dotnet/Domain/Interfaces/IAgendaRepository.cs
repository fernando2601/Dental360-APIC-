using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IAgendaRepository
    {
        // CRUD Básico
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto appointmentDto, int createdBy);
        Task<Appointment?> UpdateAppointmentAsync(int id, UpdateAppointmentDto appointmentDto);
        Task<bool> DeleteAppointmentAsync(int id);

        // Calendar Views
        Task<IEnumerable<AppointmentCalendarView>> GetCalendarViewAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int days = 7);

        // Availability & Scheduling
        Task<IEnumerable<AvailabilitySlot>> GetAvailabilitySlotsAsync(DateTime date, int? staffId = null);
        Task<IEnumerable<TimeSlotAvailability>> GetTimeSlotAvailabilityAsync(DateTime startDate, DateTime endDate, int durationMinutes);
        Task<bool> IsTimeSlotAvailableAsync(DateTime startTime, DateTime endTime, int? staffId = null, string? room = null, int? excludeAppointmentId = null);
        Task<IEnumerable<AppointmentConflict>> CheckConflictsAsync(DateTime startTime, DateTime endTime, int? staffId, string? room, int? excludeAppointmentId = null);

        // Filtering & Search
        Task<IEnumerable<Appointment>> GetAppointmentsWithFiltersAsync(AgendaFilters filters);
        Task<int> GetAppointmentsCountAsync(AgendaFilters filters);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsByStaffAsync(int staffId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(string status);

        // Status Management
        Task<bool> UpdateAppointmentStatusAsync(int id, string status, string? reason = null, int? updatedBy = null);
        Task<bool> ConfirmAppointmentAsync(int id, int? confirmedBy = null);
        Task<bool> CancelAppointmentAsync(int id, string reason, int? cancelledBy = null);
        Task<bool> CompleteAppointmentAsync(int id, decimal? actualCost = null, string? notes = null);
        Task<bool> MarkNoShowAsync(int id, string? notes = null);

        // Rescheduling
        Task<bool> RescheduleAppointmentAsync(RescheduleRequest request, int? rescheduledBy = null);
        Task<bool> BulkUpdateAppointmentsAsync(BulkAppointmentAction action, int? updatedBy = null);

        // Statistics & Analytics
        Task<AgendaStatistics> GetAgendaStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<HourlyStats>> GetHourlyDistributionAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<DailyStats>> GetWeeklyDistributionAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<StatusStats>> GetStatusDistributionAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Working Hours & Blocked Time
        Task<IEnumerable<WorkingHours>> GetWorkingHoursAsync(int? staffId = null);
        Task<WorkingHours> CreateWorkingHoursAsync(WorkingHours workingHours);
        Task<bool> UpdateWorkingHoursAsync(int id, WorkingHours workingHours);
        Task<bool> DeleteWorkingHoursAsync(int id);

        Task<IEnumerable<BlockedTimeSlot>> GetBlockedTimeSlotsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<BlockedTimeSlot> CreateBlockedTimeSlotAsync(BlockedTimeSlot blockedSlot);
        Task<bool> UpdateBlockedTimeSlotAsync(int id, BlockedTimeSlot blockedSlot);
        Task<bool> DeleteBlockedTimeSlotAsync(int id);

        // Reminders
        Task<IEnumerable<AppointmentReminder>> GetPendingRemindersAsync();
        Task<AppointmentReminder> CreateReminderAsync(AppointmentReminder reminder);
        Task<bool> MarkReminderAsSentAsync(int reminderId);
        Task<bool> UpdateReminderErrorAsync(int reminderId, string errorMessage);

        // Recurring Appointments
        Task<IEnumerable<Appointment>> CreateRecurringAppointmentsAsync(CreateAppointmentDto appointmentDto, int createdBy);
        Task<IEnumerable<Appointment>> GetRecurringAppointmentsAsync(int parentAppointmentId);
        Task<bool> UpdateRecurringSeriesAsync(int parentAppointmentId, UpdateAppointmentDto appointmentDto);
        Task<bool> CancelRecurringSeriesAsync(int parentAppointmentId, string reason, int? cancelledBy = null);

        // Reports & Export
        Task<object> GetAppointmentReportAsync(DateTime startDate, DateTime endDate, string reportType);
        Task<IEnumerable<object>> GetStaffProductivityReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<object>> GetServiceUtilizationReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<object>> GetPatientAttendanceReportAsync(DateTime startDate, DateTime endDate);

        // Dashboard Metrics
        Task<object> GetDashboardMetricsAsync();
        Task<IEnumerable<object>> GetRecentAppointmentChangesAsync(int limit = 10);
        Task<IEnumerable<object>> GetUpcomingDeadlinesAsync();

        // Room Management
        Task<IEnumerable<string>> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime);
        Task<IEnumerable<object>> GetRoomUtilizationAsync(DateTime startDate, DateTime endDate);

        // Patient Communication
        Task<bool> SendAppointmentConfirmationAsync(int appointmentId);
        Task<bool> SendAppointmentReminderAsync(int appointmentId, string type);
        Task<bool> SendReschedulingNotificationAsync(int appointmentId);
        Task<bool> SendCancellationNotificationAsync(int appointmentId);
    }
}