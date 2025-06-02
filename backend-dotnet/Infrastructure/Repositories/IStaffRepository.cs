using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IStaffRepository
    {
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<Staff?> GetByIdAsync(int id);
        Task<Staff> CreateAsync(CreateStaffDto staff);
        Task<Staff?> UpdateAsync(int id, CreateStaffDto staff);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Staff>> SearchAsync(string searchTerm);
        Task<IEnumerable<Staff>> GetBySpecializationAsync(string specialization);
    }
}