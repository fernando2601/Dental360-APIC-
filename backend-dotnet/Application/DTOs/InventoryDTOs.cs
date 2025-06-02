using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreateInventoryDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;
        
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
        [StringLength(200)]
        public string? Supplier { get; set; }
        
        [StringLength(100)]
        public string? Location { get; set; }
        
        [Range(0, int.MaxValue)]
        public int MinStock { get; set; } = 10;
        
        public DateTime? ExpirationDate { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Ativo";
    }

    public class UpdateInventoryDto : CreateInventoryDto
    {
        public int Id { get; set; }
    }

    public class InventoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public string? Supplier { get; set; }
        public string? Location { get; set; }
        public int MinStock { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal TotalValue { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsExpiringSoon { get; set; }
    }

    public class StockAdjustmentDto
    {
        [Required]
        public int ItemId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Reference { get; set; }
    }
}