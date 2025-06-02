using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IAgendaService
    {
        // CRUD Básico
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto appointmentDto, int createdBy);
        Task<Appointment?> UpdateAppointmentAsync(int id, UpdateAppointmentDto appointmentDto);
        Task<bool> DeleteAppointmentAsync(int id);

        // Calendar Views
        Task<object> GetCalendarViewAsync(DateTime startDate, DateTime endDate);
        Task<object> GetTodayAppointmentsAsync();
        Task<object> GetUpcomingAppointmentsAsync(int days = 7);

        // Availability & Scheduling
        Task<bool> IsTimeSlotAvailableAsync(DateTime startTime, DateTime endTime, int? staffId = null, string? room = null, int? excludeAppointmentId = null);
        Task<object> CheckAvailabilityAsync(DateTime startTime, DateTime endTime, int? staffId = null);
        Task<object> GetAvailableTimeSlotsAsync(DateTime date, int durationMinutes = 60, int? staffId = null);

        // Status Management
        Task<bool> ConfirmAppointmentAsync(int id, int? confirmedBy = null);
        Task<bool> CancelAppointmentAsync(int id, string reason, int? cancelledBy = null);
        Task<bool> CompleteAppointmentAsync(int id, decimal? actualCost = null, string? notes = null);
        Task<bool> MarkNoShowAsync(int id, string? notes = null);
        Task<bool> RescheduleAppointmentAsync(RescheduleRequest request, int? rescheduledBy = null);

        // Filtering & Search
        Task<object> GetAppointmentsWithFiltersAsync(AgendaFilters filters);
        Task<object> GetAppointmentsByPatientAsync(int patientId);
        Task<object> GetAppointmentsByStaffAsync(int staffId, DateTime? startDate = null, DateTime? endDate = null);

        // Statistics & Analytics
        Task<AgendaStatistics> GetAgendaStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetDashboardMetricsAsync();
        Task<object> GetHourlyDistributionAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetWeeklyDistributionAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetStatusDistributionAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Bulk Operations
        Task<bool> BulkUpdateAppointmentsAsync(BulkAppointmentAction action, int? updatedBy = null);

        // Reports
        Task<object> GetAppointmentReportAsync(DateTime startDate, DateTime endDate, string reportType);
        Task<object> GetStaffProductivityReportAsync(DateTime startDate, DateTime endDate);
        Task<object> GetServiceUtilizationReportAsync(DateTime startDate, DateTime endDate);

        // Room Management
        Task<object> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime);
        Task<object> GetRoomUtilizationAsync(DateTime startDate, DateTime endDate);

        // Communication
        Task<bool> SendAppointmentNotificationAsync(int appointmentId, string type);
    }
}