using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IAgendaService
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment?> UpdateAsync(int id, Appointment appointment);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Appointment>> SearchAsync(string searchTerm);
        
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsByStaffAsync(int staffId);
        Task<bool> ConfirmAppointmentAsync(int id);
        Task<bool> CancelAppointmentAsync(int id);
        Task<bool> CompleteAppointmentAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByStaffAndDateAsync(int staffId, DateTime date);
    }
} 