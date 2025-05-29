using ClinicApi.Models;
using ClinicApi.Repositories;

namespace ClinicApi.Services
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffResponse>> GetAllStaffAsync();
        Task<StaffResponse?> GetStaffByIdAsync(int id);
        Task<StaffResponse> CreateStaffAsync(CreateStaffRequest request);
        Task<StaffResponse?> UpdateStaffAsync(int id, UpdateStaffRequest request);
        Task<bool> DeleteStaffAsync(int id);
        Task<IEnumerable<StaffResponse>> GetStaffByDepartmentAsync(string department);
        Task<IEnumerable<StaffResponse>> GetStaffByPositionAsync(string position);
        Task<IEnumerable<StaffResponse>> SearchStaffAsync(string searchTerm);
        Task<StaffStatsResponse> GetStaffStatsAsync();
        Task<IEnumerable<string>> GetDepartmentsAsync();
        Task<IEnumerable<string>> GetPositionsAsync();
        Task<IEnumerable<StaffResponse>> GetTeamMembersAsync(int managerId);
    }

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
            try
            {
                var staff = await _staffRepository.GetAllWithDetailsAsync();
                return staff.Select(MapToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os funcionários");
                throw;
            }
        }

        public async Task<StaffResponse?> GetStaffByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido fornecido: {Id}", id);
                return null;
            }

            try
            {
                var staff = await _staffRepository.GetByIdWithDetailsAsync(id);
                return staff != null ? MapToResponse(staff) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionário com ID {Id}", id);
                throw;
            }
        }

        public async Task<StaffResponse> CreateStaffAsync(CreateStaffRequest request)
        {
            ValidateStaffRequest(request);

            try
            {
                // Check if email already exists
                var existingStaff = await SearchStaffAsync(request.Email);
                if (existingStaff.Any(s => s.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ArgumentException("Email já está em uso por outro funcionário");
                }

                // Validate manager exists if provided
                if (request.ManagerId.HasValue)
                {
                    var manager = await _staffRepository.GetByIdWithDetailsAsync(request.ManagerId.Value);
                    if (manager == null)
                    {
                        throw new ArgumentException("Gerente especificado não foi encontrado");
                    }
                }

                var staff = await _staffRepository.CreateAsync(request);
                var detailedStaff = await _staffRepository.GetByIdWithDetailsAsync(staff.Id);
                return MapToResponse(detailedStaff!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar funcionário");
                throw;
            }
        }

        public async Task<StaffResponse?> UpdateStaffAsync(int id, UpdateStaffRequest request)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido fornecido para atualização: {Id}", id);
                return null;
            }

            ValidateStaffRequest(request);

            try
            {
                // Check if staff exists
                var existingStaff = await _staffRepository.GetByIdWithDetailsAsync(id);
                if (existingStaff == null)
                {
                    _logger.LogWarning("Funcionário com ID {Id} não encontrado para atualização", id);
                    return null;
                }

                // Check if email is being changed and if it's already in use
                if (!existingStaff.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailInUse = await SearchStaffAsync(request.Email);
                    if (emailInUse.Any(s => s.Id != id && s.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new ArgumentException("Email já está em uso por outro funcionário");
                    }
                }

                // Validate manager exists if provided and is not the same person
                if (request.ManagerId.HasValue)
                {
                    if (request.ManagerId.Value == id)
                    {
                        throw new ArgumentException("Um funcionário não pode ser gerente de si mesmo");
                    }

                    var manager = await _staffRepository.GetByIdWithDetailsAsync(request.ManagerId.Value);
                    if (manager == null)
                    {
                        throw new ArgumentException("Gerente especificado não foi encontrado");
                    }
                }

                var updatedStaff = await _staffRepository.UpdateAsync(id, request);
                if (updatedStaff == null)
                {
                    return null;
                }

                var detailedStaff = await _staffRepository.GetByIdWithDetailsAsync(updatedStaff.Id);
                return MapToResponse(detailedStaff!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar funcionário com ID {Id}", id);
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
                var existingStaff = await _staffRepository.GetByIdWithDetailsAsync(id);
                if (existingStaff == null)
                {
                    _logger.LogWarning("Funcionário com ID {Id} não encontrado para exclusão", id);
                    return false;
                }

                // Check if staff has team members (is a manager)
                var teamMembers = await _staffRepository.GetTeamMembersAsync(id);
                if (teamMembers.Any())
                {
                    throw new InvalidOperationException("Não é possível excluir um funcionário que tem membros de equipe subordinados");
                }

                return await _staffRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir funcionário com ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<StaffResponse>> GetStaffByDepartmentAsync(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
            {
                _logger.LogWarning("Departamento vazio fornecido");
                return Enumerable.Empty<StaffResponse>();
            }

            try
            {
                var staff = await _staffRepository.GetByDepartmentAsync(department);
                return staff.Select(MapToBasicResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionários do departamento {Department}", department);
                throw;
            }
        }

        public async Task<IEnumerable<StaffResponse>> GetStaffByPositionAsync(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
            {
                _logger.LogWarning("Cargo vazio fornecido");
                return Enumerable.Empty<StaffResponse>();
            }

            try
            {
                var staff = await _staffRepository.GetByPositionAsync(position);
                return staff.Select(MapToBasicResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionários do cargo {Position}", position);
                throw;
            }
        }

        public async Task<IEnumerable<StaffResponse>> SearchStaffAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.LogWarning("Termo de busca vazio fornecido");
                return Enumerable.Empty<StaffResponse>();
            }

            try
            {
                var staff = await _staffRepository.SearchAsync(searchTerm);
                return staff.Select(MapToBasicResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionários com termo {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<StaffStatsResponse> GetStaffStatsAsync()
        {
            try
            {
                return await _staffRepository.GetStatsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas de funcionários");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            try
            {
                return await _staffRepository.GetDepartmentsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar departamentos");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetPositionsAsync()
        {
            try
            {
                return await _staffRepository.GetPositionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cargos");
                throw;
            }
        }

        public async Task<IEnumerable<StaffResponse>> GetTeamMembersAsync(int managerId)
        {
            if (managerId <= 0)
            {
                _logger.LogWarning("ID de gerente inválido fornecido: {ManagerId}", managerId);
                return Enumerable.Empty<StaffResponse>();
            }

            try
            {
                var teamMembers = await _staffRepository.GetTeamMembersAsync(managerId);
                return teamMembers.Select(MapToBasicResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar membros da equipe do gerente {ManagerId}", managerId);
                throw;
            }
        }

        // Mapping methods
        private static StaffResponse MapToResponse(StaffDetailedModel staff)
        {
            return new StaffResponse
            {
                Id = staff.Id,
                FullName = staff.FullName,
                Email = staff.Email,
                Phone = staff.Phone,
                Position = staff.Position,
                Specialization = staff.Specialization,
                Department = staff.Department,
                Salary = staff.Salary,
                HireDate = staff.HireDate,
                IsActive = staff.IsActive,
                Bio = staff.Bio,
                ProfileImageUrl = staff.ProfileImageUrl,
                YearsOfExperience = staff.YearsOfExperience,
                License = staff.License,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt,
                ManagerId = staff.ManagerId,
                ManagerName = staff.ManagerName,
                Certifications = staff.Certifications,
                Skills = staff.Skills
            };
        }

        private static StaffResponse MapToBasicResponse(StaffModel staff)
        {
            return new StaffResponse
            {
                Id = staff.Id,
                FullName = staff.FullName,
                Email = staff.Email,
                Phone = staff.Phone,
                Position = staff.Position,
                Specialization = staff.Specialization,
                Department = staff.Department,
                Salary = staff.Salary,
                HireDate = staff.HireDate,
                IsActive = staff.IsActive,
                Bio = staff.Bio,
                ProfileImageUrl = staff.ProfileImageUrl,
                YearsOfExperience = staff.YearsOfExperience,
                License = staff.License,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt,
                ManagerId = staff.ManagerId,
                Certifications = staff.Certifications,
                Skills = staff.Skills
            };
        }

        // Validation methods
        private static void ValidateStaffRequest(CreateStaffRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Nome completo é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email é obrigatório");

            if (!IsValidEmail(request.Email))
                throw new ArgumentException("Email inválido");

            if (string.IsNullOrWhiteSpace(request.Phone))
                throw new ArgumentException("Telefone é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Position))
                throw new ArgumentException("Cargo é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Specialization))
                throw new ArgumentException("Especialização é obrigatória");

            if (string.IsNullOrWhiteSpace(request.Department))
                throw new ArgumentException("Departamento é obrigatório");

            if (request.Salary < 0)
                throw new ArgumentException("Salário deve ser maior ou igual a zero");

            if (request.HireDate > DateTime.Now)
                throw new ArgumentException("Data de contratação não pode ser no futuro");

            if (request.YearsOfExperience < 0 || request.YearsOfExperience > 50)
                throw new ArgumentException("Anos de experiência deve estar entre 0 e 50");
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