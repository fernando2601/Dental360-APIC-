using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _appointmentRepository.GetAllAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _appointmentRepository.GetByIdAsync(id);
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            appointment.CreatedAt = DateTime.UtcNow;
            return await _appointmentRepository.CreateAsync(appointment);
        }

        public async Task<Appointment?> UpdateAppointmentAsync(int id, Appointment appointment)
        {
            return await _appointmentRepository.UpdateAsync(id, appointment);
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            return await _appointmentRepository.DeleteAsync(id);
        }

        public async Task<object> GetAppointmentReportsAsync(
            DateTime? startDate,
            DateTime? endDate,
            string[]? statuses,
            int? professionalId,
            int? clientId,
            string? convenio,
            string? sala,
            int page,
            int limit)
        {
            // Implementação básica - retorna todos os agendamentos
            var appointments = await _appointmentRepository.GetAllAsync();
            
            return new
            {
                appointments = appointments,
                pagination = new
                {
                    currentPage = page,
                    totalPages = 1,
                    totalItems = appointments.Count(),
                    itemsPerPage = limit
                },
                summary = new
                {
                    totalAppointments = appointments.Count(),
                    totalRevenue = 0,
                    completedAppointments = appointments.Count(a => a.Status == "completed"),
                    cancelledAppointments = appointments.Count(a => a.Status == "cancelled")
                }
            };
        }
    }
}