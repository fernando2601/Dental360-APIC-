using System;

namespace DentalSpa.Domain.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; } = 5;
        public decimal UnitPrice { get; set; }
        public string? Supplier { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsLowStock() => Quantity <= MinQuantity;
        public bool IsExpiringSoon() => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.Now.AddDays(30);
        public bool IsExpired() => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now;
        public decimal GetTotalValue() => Quantity * UnitPrice;
    }
}