using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Domain.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;
        
        public int MinStock { get; set; } = 10;
        
        public decimal UnitPrice { get; set; }
        
        [StringLength(200)]
        public string? Supplier { get; set; }
        
        [StringLength(100)]
        public string? Location { get; set; }
        
        [StringLength(50)]
        public string? BatchNumber { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Ativo";
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}