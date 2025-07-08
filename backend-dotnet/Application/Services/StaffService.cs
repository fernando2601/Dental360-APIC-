using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ILogger<StaffService> _logger;

        public StaffService(IStaffRepository staffRepository, ILogger<StaffService> logger)
        {
            _staffRepository = staffRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<StaffResponse>> GetAllStaffAsync()
        {
            var staff = await _staffRepository.GetAllStaffAsync();
            return staff.Select(MapToResponse);
        }

        public async Task<StaffResponse?> GetStaffByIdAsync(int id)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(id);
            return staff == null ? null : MapToResponse(staff);
        }

        public async Task<StaffResponse> CreateAsync(StaffCreateRequest request)
        {
            var staff = new Staff
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                ClinicId = request.ClinicId,
                PositionId = request.PositionId,
                Department = request.Department,
                Salary = request.Salary,
                HireDate = request.HireDate,
                IsActive = request.IsActive,
                Bio = request.Bio,
                ProfileImageUrl = request.ProfileImageUrl,
                YearsOfExperience = request.YearsOfExperience,
                License = request.License
            };
            var created = await _staffRepository.CreateAsync(staff);
            // Salvar relação N:N com serviços
            await _staffRepository.SetStaffServicesAsync(created.Id, request.ServiceIds);
            return MapToResponse(created, request.ServiceIds);
        }

        public async Task<StaffResponse?> UpdateAsync(int id, StaffCreateRequest request)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null) return null;
            staff.FullName = request.FullName;
            staff.Email = request.Email;
            staff.Phone = request.Phone;
            staff.ClinicId = request.ClinicId;
            staff.PositionId = request.PositionId;
            staff.Department = request.Department;
            staff.Salary = request.Salary;
            staff.HireDate = request.HireDate;
            staff.IsActive = request.IsActive;
            staff.Bio = request.Bio;
            staff.ProfileImageUrl = request.ProfileImageUrl;
            staff.YearsOfExperience = request.YearsOfExperience;
            staff.License = request.License;
            var updated = await _staffRepository.UpdateAsync(id, staff);
            await _staffRepository.SetStaffServicesAsync(id, request.ServiceIds);
            return MapToResponse(updated, request.ServiceIds);
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido fornecido para exclusão: {Id}", id);
                return false;
            }

            try
            {
                return await _staffRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir funcionário com ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<StaffResponse>> SearchStaffAsync(string searchTerm)
        {
            var staff = await _staffRepository.SearchStaffAsync(searchTerm);
            return staff.Select(MapToResponse);
        }

        public async Task<IEnumerable<StaffResponse>> GetStaffBySpecializationAsync(string specialization)
        {
            // Implementação básica - retorna todos os funcionários
            return Enumerable.Empty<StaffResponse>();
        }

        public async Task<IEnumerable<StaffResponse>> GetStaffByDepartmentAsync(string department)
        {
            var staff = await _staffRepository.GetStaffByDepartmentAsync(department);
            return staff.Select(MapToResponse);
        }

        public async Task<IEnumerable<StaffResponse>> GetStaffByPositionAsync(string position)
        {
            var staff = await _staffRepository.GetStaffByPositionAsync(position);
            return staff.Select(MapToResponse);
        }

        public async Task<object> GetStaffStatsAsync()
        {
            // Implementação básica
            var staff = await _staffRepository.GetAllAsync();
            return new
            {
                totalStaff = staff.Count(),
                activeStaff = staff.Count(s => s.IsActive),
                departments = staff.Select(s => s.Department).Distinct().Count()
            };
        }

        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            // Implementação básica
            var staff = await _staffRepository.GetAllAsync();
            return staff.Select(s => s.Department).Distinct();
        }

        public async Task<IEnumerable<string>> GetPositionsAsync()
        {
            // Implementação básica
            var staff = await _staffRepository.GetAllAsync();
            return staff.Select(s => s.Position).Distinct();
        }

        public async Task<IEnumerable<StaffResponse>> GetTeamMembersAsync(int managerId)
        {
            var staff = await _staffRepository.GetTeamMembersAsync(managerId);
            return staff.Select(MapToResponse);
        }

        private static void ValidateStaff(Staff staff)
        {
            if (string.IsNullOrWhiteSpace(staff.Name))
                throw new ArgumentException("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(staff.Email))
                throw new ArgumentException("Email é obrigatório");

            if (!IsValidEmail(staff.Email))
                throw new ArgumentException("Email inválido");

            if (string.IsNullOrWhiteSpace(staff.Position))
                throw new ArgumentException("Cargo é obrigatório");
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private StaffResponse MapToResponse(Staff s, List<int>? serviceIds = null)
        {
            return new StaffResponse
            {
                FullName = s.FullName,
                Email = s.Email,
                Phone = s.Phone,
                PositionId = s.PositionId,
                ClinicId = s.ClinicId,
                Department = s.Department,
                Salary = s.Salary,
                HireDate = s.HireDate,
                IsActive = s.IsActive,
                Bio = s.Bio,
                ProfileImageUrl = s.ProfileImageUrl,
                YearsOfExperience = s.YearsOfExperience,
                License = s.License,
                Name = s.Name,
                ServiceIds = serviceIds ?? s.StaffServices.Select(ss => ss.ServiceId).ToList()
            };
        }
    }
}