using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class ServiceCreateRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Duração deve ser maior que zero")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;
    }

    public class ServiceUpdateRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Duração deve ser maior que zero")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class ServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsExpensive { get; set; }
        public bool IsLongDuration { get; set; }
    }
}