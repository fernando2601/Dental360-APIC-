namespace ClinicApi.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Supplier { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? BatchNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateInventoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Supplier { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? BatchNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }
}