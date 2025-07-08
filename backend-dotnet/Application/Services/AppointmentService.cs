using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IEnumerable<AppointmentResponse>> GetAllAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            return appointments.Select(MapToResponse);
        }

        public async Task<AppointmentResponse?> GetByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            return appointment == null ? null : MapToResponse(appointment);
        }

        public async Task<IEnumerable<AppointmentResponse>> SearchAsync(string searchTerm)
        {
            var appointments = await _appointmentRepository.SearchAsync(searchTerm);
            return appointments.Select(MapToResponse);
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            appointment.CreatedAt = DateTime.UtcNow;
            return await _appointmentRepository.CreateAsync(appointment);
        }

        public async Task<Appointment?> UpdateAsync(int id, Appointment appointment)
        {
            return await _appointmentRepository.UpdateAsync(id, appointment);
        }

        public async Task<bool> DeleteAsync(int id)
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

        public async Task<IEnumerable<AppointmentResponse>> GetBusyTimesAsync(int staffId, DateTime date)
        {
            var all = await _appointmentRepository.GetAllAsync();
            return all.Where(a => a.StaffId == staffId && a.StartTime.Date == date.Date).Select(MapToResponse);
        }

        private AppointmentResponse MapToResponse(Appointment a)
        {
            return new AppointmentResponse
            {
                ClientId = a.ClientId,
                StaffId = a.StaffId,
                ServiceId = a.ServiceId,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Notes = a.Notes,
                Room = a.Room,
                Price = a.Price,
                PaymentStatus = a.PaymentStatus,
                PaymentMethod = a.PaymentMethod,
                IsRecurring = a.IsRecurring,
                RecurrencePattern = a.RecurrencePattern,
                RecurrenceEndDate = a.RecurrenceEndDate,
                ParentAppointmentId = a.ParentAppointmentId,
                CancellationReason = a.CancellationReason,
                CancelledAt = a.CancelledAt,
                ConfirmedAt = a.ConfirmedAt,
                CompletedAt = a.CompletedAt,
                ClientFeedback = a.ClientFeedback,
                Rating = a.Rating,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            };
        }
    }
}