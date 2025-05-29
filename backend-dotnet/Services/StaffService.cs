using ClinicApi.Models;
using ClinicApi.Repositories;

namespace ClinicApi.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;

        public StaffService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<IEnumerable<Staff>> GetAllStaffAsync()
        {
            return await _staffRepository.GetAllAsync();
        }

        public async Task<Staff?> GetStaffByIdAsync(int id)
        {
            return await _staffRepository.GetByIdAsync(id);
        }

        public async Task<Staff> CreateStaffAsync(CreateStaffDto staffDto)
        {
            return await _staffRepository.CreateAsync(staffDto);
        }

        public async Task<Staff?> UpdateStaffAsync(int id, CreateStaffDto staffDto)
        {
            return await _staffRepository.UpdateAsync(id, staffDto);
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            return await _staffRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Staff>> SearchStaffAsync(string searchTerm)
        {
            return await _staffRepository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<Staff>> GetStaffBySpecializationAsync(string specialization)
        {
            return await _staffRepository.GetBySpecializationAsync(specialization);
        }
    }
}