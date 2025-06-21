using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment?> UpdateAppointmentAsync(int id, Appointment appointment);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<object> GetAppointmentReportsAsync(
            DateTime? startDate,
            DateTime? endDate,
            string[]? statuses,
            int? professionalId,
            int? clientId,
            string? convenio,
            string? sala,
            int page,
            int limit);
    }
}