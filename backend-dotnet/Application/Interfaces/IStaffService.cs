using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<Staff>> GetAllStaffAsync();
        Task<Staff?> GetStaffByIdAsync(int id);
        Task<Staff> CreateStaffAsync(CreateStaffDto staffDto);
        Task<Staff?> UpdateStaffAsync(int id, CreateStaffDto staffDto);
        Task<bool> DeleteStaffAsync(int id);
        Task<IEnumerable<Staff>> SearchStaffAsync(string searchTerm);
        Task<IEnumerable<Staff>> GetStaffBySpecializationAsync(string specialization);
    }
}