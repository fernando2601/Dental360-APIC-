using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<Staff>> GetAllStaffAsync();
        Task<Staff?> GetStaffByIdAsync(int id);
        Task<Staff> CreateStaffAsync(Staff staff);
        Task<Staff?> UpdateStaffAsync(Staff staff);
        Task<bool> DeleteStaffAsync(int id);
        Task<IEnumerable<Staff>> SearchStaffAsync(string searchTerm);
        Task<IEnumerable<Staff>> GetStaffBySpecializationAsync(string specialization);
        Task<IEnumerable<Staff>> GetStaffByDepartmentAsync(string department);
        Task<IEnumerable<Staff>> GetStaffByPositionAsync(string position);
        Task<object> GetStaffStatsAsync();
        Task<IEnumerable<string>> GetDepartmentsAsync();
        Task<IEnumerable<string>> GetPositionsAsync();
        Task<IEnumerable<Staff>> GetTeamMembersAsync(int managerId);
    }
}