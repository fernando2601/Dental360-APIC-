using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IStaffRepository
    {
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<Staff?> GetByIdAsync(int id);
        Task<Staff> CreateAsync(Staff staff);
        Task<Staff?> UpdateAsync(int id, Staff staff);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Staff>> GetBySpecializationAsync(string specialization);
        Task SetStaffServicesAsync(int staffId, List<int> serviceIds);
        Task<IEnumerable<Staff>> GetStaffByDepartmentAsync(string department);
        Task<IEnumerable<Staff>> GetStaffByPositionAsync(string position);
        Task<IEnumerable<Staff>> GetTeamMembersAsync(int managerId);
        Task<IEnumerable<Staff>> GetAllStaffAsync();
        Task<Staff?> GetStaffByIdAsync(int id);
    }
}