using DentalSpa.Domain.Entities;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffResponse>> GetAllStaffAsync();
        Task<StaffResponse?> GetStaffByIdAsync(int id);
        Task<StaffResponse> CreateAsync(StaffCreateRequest request);
        Task<StaffResponse?> UpdateAsync(int id, StaffCreateRequest request);
        Task<bool> DeleteStaffAsync(int id);
        Task<IEnumerable<StaffResponse>> GetStaffByDepartmentAsync(string department);
        Task<IEnumerable<StaffResponse>> GetStaffByPositionAsync(string position);
        Task<object> GetStaffStatsAsync();
        Task<IEnumerable<string>> GetDepartmentsAsync();
        Task<IEnumerable<string>> GetPositionsAsync();
        Task<IEnumerable<StaffResponse>> GetTeamMembersAsync(int managerId);
    }
}