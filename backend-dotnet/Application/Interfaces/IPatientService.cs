using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient> CreateAsync(Patient patient);
        Task<Patient?> UpdateAsync(int id, Patient patient);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
    }
}