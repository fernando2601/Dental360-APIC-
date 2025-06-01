using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class InventoryCreateRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade deve ser maior ou igual a zero")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade mínima deve ser maior ou igual a zero")]
        public int MinQuantity { get; set; } = 5;

        [Required(ErrorMessage = "Preço unitário é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal UnitPrice { get; set; }

        public string? Supplier { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class InventoryUpdateRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade deve ser maior ou igual a zero")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade mínima deve ser maior ou igual a zero")]
        public int MinQuantity { get; set; }

        [Required(ErrorMessage = "Preço unitário é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal UnitPrice { get; set; }

        public string? Supplier { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class InventoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Supplier { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsExpiringSoon { get; set; }
        public bool IsExpired { get; set; }
        public decimal TotalValue { get; set; }
    }
}