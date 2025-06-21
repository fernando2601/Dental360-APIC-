using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment?> UpdateAsync(int id, Appointment appointment);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Appointment>> SearchAsync(string searchTerm);
    }
}