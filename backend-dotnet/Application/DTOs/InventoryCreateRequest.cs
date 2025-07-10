namespace DentalSpa.Application.DTOs
{
    public class InventoryCreateRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ClinicId { get; set; }
    }
} 