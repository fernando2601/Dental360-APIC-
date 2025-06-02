using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Models
{
    public class ServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateServiceRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        [StringLength(100, ErrorMessage = "Categoria deve ter no máximo 100 caracteres")]
        public string Category { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(1, 480, ErrorMessage = "Duração deve estar entre 1 e 480 minutos")]
        public int DurationMinutes { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }

    public class UpdateServiceRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        [StringLength(100, ErrorMessage = "Categoria deve ter no máximo 100 caracteres")]
        public string Category { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(1, 480, ErrorMessage = "Duração deve estar entre 1 e 480 minutos")]
        public int DurationMinutes { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }

    public class ServiceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string FormattedPrice => $"R$ {Price:N2}";
        public string FormattedDuration => $"{DurationMinutes} minutos";
    }

    public class ServiceStatsResponse
    {
        public int TotalServices { get; set; }
        public int ActiveServices { get; set; }
        public int InactiveServices { get; set; }
        public decimal AveragePrice { get; set; }
        public int AverageDuration { get; set; }
        public List<CategoryStats> CategoryBreakdown { get; set; } = new();
    }

    public class CategoryStats
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal AveragePrice { get; set; }
    }
}