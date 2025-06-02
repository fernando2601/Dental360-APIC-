using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;

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

        public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto appointmentDto)
        {
            return await _appointmentRepository.CreateAsync(appointmentDto);
        }

        public async Task<Appointment?> UpdateAppointmentAsync(int id, CreateAppointmentDto appointmentDto)
        {
            return await _appointmentRepository.UpdateAsync(id, appointmentDto);
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
            var appointments = await _appointmentRepository.GetAppointmentReportsAsync(
                startDate, endDate, statuses, professionalId, clientId, convenio, sala, page, limit);
            
            var totalCount = await _appointmentRepository.GetAppointmentReportsCountAsync(
                startDate, endDate, statuses, professionalId, clientId, convenio, sala);

            // Calcular estatísticas
            var allAppointments = await _appointmentRepository.GetAppointmentReportsAsync(
                startDate, endDate, statuses, professionalId, clientId, convenio, sala, 1, int.MaxValue);

            var statusBreakdown = allAppointments
                .GroupBy(a => a.Status)
                .Select(g => new
                {
                    status = g.Key,
                    count = g.Count(),
                    percentage = totalCount > 0 ? (int)Math.Round((double)g.Count() / totalCount * 100) : 0
                })
                .ToList();

            return new
            {
                appointments = appointments,
                pagination = new
                {
                    currentPage = page,
                    totalPages = (int)Math.Ceiling((double)totalCount / limit),
                    totalItems = totalCount,
                    itemsPerPage = limit
                },
                summary = new
                {
                    totalAppointments = totalCount,
                    totalRevenue = allAppointments.Sum(a => a.Valor),
                    completedAppointments = allAppointments.Count(a => a.Status == "completed"),
                    cancelledAppointments = allAppointments.Count(a => a.Status == "cancelled" || a.Status == "no_show"),
                    statusBreakdown = statusBreakdown
                }
            };
        }
    }
}