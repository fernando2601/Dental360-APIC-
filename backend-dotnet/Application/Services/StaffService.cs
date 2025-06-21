using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

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

        public async Task<IEnumerable<Staff>> GetAllStaffAsync()
        {
            try
            {
                return await _staffRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os funcionários");
                throw;
            }
        }

        public async Task<Staff?> GetStaffByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido fornecido: {Id}", id);
                return null;
            }

            try
            {
                return await _staffRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionário com ID {Id}", id);
                throw;
            }
        }

        public async Task<Staff> CreateStaffAsync(Staff staff)
        {
            ValidateStaff(staff);

            try
            {
                return await _staffRepository.CreateAsync(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar funcionário");
                throw;
            }
        }

        public async Task<Staff?> UpdateStaffAsync(Staff staff)
        {
            if (staff.Id <= 0)
            {
                _logger.LogWarning("ID inválido fornecido para atualização: {Id}", staff.Id);
                return null;
            }

            ValidateStaff(staff);

            try
            {
                return await _staffRepository.UpdateAsync(staff.Id, staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar funcionário com ID {Id}", staff.Id);
                throw;
            }
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

        public async Task<IEnumerable<Staff>> SearchStaffAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<Staff>();
            }

            try
            {
                return await _staffRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionários com termo: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Staff>> GetStaffBySpecializationAsync(string specialization)
        {
            // Implementação básica - retorna todos os funcionários
            return await _staffRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Staff>> GetStaffByDepartmentAsync(string department)
        {
            // Implementação básica - retorna todos os funcionários
            return await _staffRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Staff>> GetStaffByPositionAsync(string position)
        {
            // Implementação básica - retorna todos os funcionários
            return await _staffRepository.GetAllAsync();
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

        public async Task<IEnumerable<Staff>> GetTeamMembersAsync(int managerId)
        {
            // Implementação básica - retorna todos os funcionários
            return await _staffRepository.GetAllAsync();
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
    }
}