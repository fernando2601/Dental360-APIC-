using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto appointmentDto);
        Task<Appointment?> UpdateAppointmentAsync(int id, CreateAppointmentDto appointmentDto);
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