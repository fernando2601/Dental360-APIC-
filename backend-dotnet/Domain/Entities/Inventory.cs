using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Domain.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}