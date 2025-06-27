using System;
using System.Collections.Generic;

namespace DentalSpa.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int SessionsIncluded { get; set; }
        public int ValidityDays { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal GetDiscountPercentage() => ((OriginalPrice - Price) / OriginalPrice) * 100;
        public bool HasDiscount() => Price < OriginalPrice;
        public decimal GetPricePerSession() => Price / SessionsIncluded;
    }

    public class ClientProduct
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; }
        public int SessionsUsed { get; set; } = 0;
        public string Status { get; set; } = "active"; // active, expired, completed

        // Navigation properties
        public Client? Client { get; set; }
        public Product? Product { get; set; }

        public int GetRemainingSessions() => (Product?.SessionsIncluded ?? 0) - SessionsUsed;
        public bool IsExpired() => DateTime.Now > ExpiryDate;
        public bool IsCompleted() => SessionsUsed >= (Product?.SessionsIncluded ?? 0);
        public bool CanUse() => Status == "active" && !IsExpired() && !IsCompleted();
    }
}