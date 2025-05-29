using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> CreateAsync(CreateAppointmentDto appointment);
        Task<Appointment?> UpdateAsync(int id, CreateAppointmentDto appointment);
        Task<bool> DeleteAsync(int id);
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
    }
}