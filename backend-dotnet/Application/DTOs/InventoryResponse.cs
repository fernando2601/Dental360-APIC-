namespace DentalSpa.Application.DTOs
{
    public class InventoryResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ProductCategory { get; set; } = string.Empty;
    }
} 