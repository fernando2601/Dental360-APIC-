using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IAppointmentRepository
    {
        // CRUD Básico
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> CreateAsync(CreateAppointmentDto appointment);
        Task<Appointment?> UpdateAsync(int id, CreateAppointmentDto appointment);
        Task<bool> DeleteAsync(int id);

        // Funcionalidades de Calendário
        Task<IEnumerable<AppointmentCalendar>> GetCalendarAppointmentsAsync(DateTime startDate, DateTime endDate, int? staffId = null);
        Task<AppointmentScheduling> GetAvailableSlotsAsync(DateTime date, int? staffId = null, int? serviceId = null);
        Task<IEnumerable<TimeSlot>> GetTimeSlotsByDateAsync(DateTime date, int? staffId = null);

        // Validações e Conflitos
        Task<IEnumerable<AppointmentConflict>> CheckConflictsAsync(DateTime startTime, DateTime endTime, int staffId, int? excludeAppointmentId = null);
        Task<bool> IsSlotAvailableAsync(DateTime startTime, DateTime endTime, int staffId, string? room = null);

        // Agendamentos Recorrentes
        Task<IEnumerable<Appointment>> CreateRecurringAppointmentsAsync(CreateAppointmentDto baseAppointment, string recurrencePattern, DateTime endDate, int maxOccurrences);
        Task<IEnumerable<Appointment>> GetRecurringAppointmentsAsync(int parentAppointmentId);

        // Lista de Espera
        Task<IEnumerable<WaitingList>> GetWaitingListAsync(int? staffId = null, int? serviceId = null);
        Task<WaitingList> AddToWaitingListAsync(WaitingList waitingItem);
        Task<bool> NotifyWaitingListAsync(int waitingListId, int appointmentId);

        // Lembretes
        Task<IEnumerable<AppointmentReminder>> GetPendingRemindersAsync();
        Task<AppointmentReminder> CreateReminderAsync(AppointmentReminder reminder);
        Task<bool> MarkReminderAsSentAsync(int reminderId);

        // Estatísticas e Relatórios
        Task<AppointmentStatistics> GetAppointmentStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<AppointmentWithDetails>> GetAppointmentReportsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string[]? statuses = null,
            int? professionalId = null,
            int? clientId = null,
            string? convenio = null,
            string? sala = null,
            int page = 1,
            int limit = 25);
        Task<int> GetAppointmentReportsCountAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string[]? statuses = null,
            int? professionalId = null,
            int? clientId = null,
            string? convenio = null,
            string? sala = null);

        // Gestão de Status
        Task<bool> ConfirmAppointmentAsync(int id);
        Task<bool> CancelAppointmentAsync(int id, string reason);
        Task<bool> CompleteAppointmentAsync(int id, string? feedback = null, int? rating = null);
        Task<bool> MarkAsNoShowAsync(int id);

        // Disponibilidade de Profissionais
        Task<StaffAvailability> GetStaffAvailabilityAsync(int staffId, DateTime date);
        Task<IEnumerable<StaffAvailability>> GetAllStaffAvailabilityAsync(DateTime date);

        // Busca e Filtros Avançados
        Task<IEnumerable<Appointment>> SearchAppointmentsAsync(string searchTerm, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByClientAsync(int clientId, int? limit = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByStaffAsync(int staffId, DateTime? date = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByServiceAsync(int serviceId, DateTime? startDate = null, DateTime? endDate = null);

        // Dashboard de Agenda
        Task<object> GetAppointmentDashboardAsync(DateTime? date = null);
        Task<object> GetAppointmentMetricsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}