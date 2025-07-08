namespace DentalSpa.Application.DTOs
{
    public class InventoryCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int MinStock { get; set; } = 10;
        public decimal UnitPrice { get; set; }
        public string? Supplier { get; set; }
        public string? Location { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Status { get; set; } = "Ativo";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int ClinicId { get; set; }
    }
} 