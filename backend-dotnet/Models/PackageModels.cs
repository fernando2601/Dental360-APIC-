using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Models
{
    public class PackageModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public int DurationDays { get; set; }
        public int ValidityDays { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public string ImageUrl { get; set; } = string.Empty;
        public string Terms { get; set; } = string.Empty;
        public int MaxUsages { get; set; } = 1;
        public bool IsPopular { get; set; } = false;
    }

    public class PackageDetailedModel : PackageModel
    {
        public List<ServiceModel> Services { get; set; } = new();
        public decimal TotalSavings => OriginalPrice - FinalPrice;
        public int TotalPurchases { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class CreatePackageRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        [StringLength(100, ErrorMessage = "Categoria deve ter no máximo 100 caracteres")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço original é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal OriginalPrice { get; set; }

        [Range(0, 100, ErrorMessage = "Desconto deve estar entre 0 e 100")]
        public decimal DiscountPercentage { get; set; }

        [Range(1, 3650, ErrorMessage = "Duração deve estar entre 1 e 3650 dias")]
        public int DurationDays { get; set; }

        [Range(1, 365, ErrorMessage = "Validade deve estar entre 1 e 365 dias")]
        public int ValidityDays { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Pelo menos um serviço deve ser incluído")]
        public List<int> ServiceIds { get; set; } = new();

        [StringLength(500, ErrorMessage = "URL da imagem deve ter no máximo 500 caracteres")]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Termos devem ter no máximo 2000 caracteres")]
        public string Terms { get; set; } = string.Empty;

        [Range(1, 999, ErrorMessage = "Número de usos deve estar entre 1 e 999")]
        public int MaxUsages { get; set; } = 1;

        public bool IsPopular { get; set; } = false;
    }

    public class UpdatePackageRequest : CreatePackageRequest
    {
    }

    public class PackageResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public int DurationDays { get; set; }
        public int ValidityDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ServiceModel> Services { get; set; } = new();
        public string ImageUrl { get; set; } = string.Empty;
        public string Terms { get; set; } = string.Empty;
        public int MaxUsages { get; set; }
        public bool IsPopular { get; set; }
        public decimal TotalSavings { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        
        // Computed properties
        public string FormattedOriginalPrice => $"R$ {OriginalPrice:N2}";
        public string FormattedFinalPrice => $"R$ {FinalPrice:N2}";
        public string FormattedSavings => $"R$ {TotalSavings:N2}";
        public string FormattedDiscount => $"{DiscountPercentage:F0}% OFF";
    }

    public class PackageStatsResponse
    {
        public int TotalPackages { get; set; }
        public int ActivePackages { get; set; }
        public int InactivePackages { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePackagePrice { get; set; }
        public decimal AverageDiscount { get; set; }
        public List<CategoryPackageStats> CategoryBreakdown { get; set; } = new();
        public List<PopularPackage> MostPopular { get; set; } = new();
        public List<RevenueByMonth> MonthlyRevenue { get; set; } = new();
    }

    public class CategoryPackageStats
    {
        public string Category { get; set; } = string.Empty;
        public int PackageCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class PopularPackage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PurchaseCount { get; set; }
        public decimal Revenue { get; set; }
        public double Rating { get; set; }
    }

    public class RevenueByMonth
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int PackagesSold { get; set; }
    }

    // Clinic Information Models
    public class ClinicInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string OpeningHours { get; set; } = string.Empty;
        public string Specialties { get; set; } = string.Empty;
        public string SocialMedia { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateClinicInfoRequest
    {
        [Required(ErrorMessage = "Nome da clínica é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Endereço é obrigatório")]
        [StringLength(500, ErrorMessage = "Endereço deve ter no máximo 500 caracteres")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Website deve ter no máximo 200 caracteres")]
        public string Website { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "URL do logo deve ter no máximo 500 caracteres")]
        public string LogoUrl { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Horário de funcionamento deve ter no máximo 500 caracteres")]
        public string OpeningHours { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Especialidades devem ter no máximo 1000 caracteres")]
        public string Specialties { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Redes sociais devem ter no máximo 1000 caracteres")]
        public string SocialMedia { get; set; } = string.Empty;
    }

    public class ClinicInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string OpeningHours { get; set; } = string.Empty;
        public string Specialties { get; set; } = string.Empty;
        public string SocialMedia { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ClinicStatsResponse
    {
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int ActiveStaff { get; set; }
        public int TotalServices { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime EstablishedDate { get; set; }
        public int YearsInOperation { get; set; }
    }
}