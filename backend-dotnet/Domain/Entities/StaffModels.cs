using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Models
{
    public class StaffModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string Bio { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string License { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int? ManagerId { get; set; }
        
        // Navigation properties
        public List<string> Certifications { get; set; } = new();
        public List<string> Skills { get; set; } = new();
    }

    public class StaffDetailedModel : StaffModel
    {
        public string? ManagerName { get; set; }
        public int TotalPatients { get; set; }
        public int AppointmentsThisMonth { get; set; }
        public decimal RevenuThisMonth { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<StaffModel> TeamMembers { get; set; } = new();
    }

    public class CreateStaffRequest
    {
        [Required(ErrorMessage = "Nome completo é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150, ErrorMessage = "Email deve ter no máximo 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cargo é obrigatório")]
        [StringLength(100, ErrorMessage = "Cargo deve ter no máximo 100 caracteres")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Especialização é obrigatória")]
        [StringLength(150, ErrorMessage = "Especialização deve ter no máximo 150 caracteres")]
        public string Specialization { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departamento é obrigatório")]
        [StringLength(100, ErrorMessage = "Departamento deve ter no máximo 100 caracteres")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salário é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "Salário deve ser maior ou igual a zero")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Data de contratação é obrigatória")]
        public DateTime HireDate { get; set; }

        [StringLength(1000, ErrorMessage = "Bio deve ter no máximo 1000 caracteres")]
        public string Bio { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "URL da imagem deve ter no máximo 500 caracteres")]
        public string ProfileImageUrl { get; set; } = string.Empty;

        [Range(0, 50, ErrorMessage = "Anos de experiência deve estar entre 0 e 50")]
        public int YearsOfExperience { get; set; }

        [StringLength(50, ErrorMessage = "Licença deve ter no máximo 50 caracteres")]
        public string License { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public int? ManagerId { get; set; }
        public List<string> Certifications { get; set; } = new();
        public List<string> Skills { get; set; } = new();
    }

    public class UpdateStaffRequest : CreateStaffRequest
    {
    }

    public class StaffResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string License { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public List<string> Certifications { get; set; } = new();
        public List<string> Skills { get; set; } = new();
        
        // Computed properties
        public string FormattedSalary => $"R$ {Salary:N2}";
        public string YearsAtCompany => $"{(DateTime.Now - HireDate).Days / 365} anos";
        public string Status => IsActive ? "Disponível" : "Inativo";
    }

    public class StaffStatsResponse
    {
        public int TotalStaff { get; set; }
        public int ActiveStaff { get; set; }
        public int InactiveStaff { get; set; }
        public decimal AverageSalary { get; set; }
        public double AverageExperience { get; set; }
        public List<DepartmentStats> DepartmentBreakdown { get; set; } = new();
        public List<PositionStats> PositionBreakdown { get; set; } = new();
    }

    public class DepartmentStats
    {
        public string Department { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal AverageSalary { get; set; }
    }

    public class PositionStats
    {
        public string Position { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal AverageSalary { get; set; }
    }
}