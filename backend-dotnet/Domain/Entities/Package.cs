using System;
using System.Collections.Generic;

namespace DentalSpa.Domain.Entities
{
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

        // Remover métodos que usam SessionsIncluded
        // public int GetRemainingSessions() => (Product?.SessionsIncluded ?? 0) - SessionsUsed;
        // public bool IsExpired() => DateTime.Now > ExpiryDate;
        // public bool IsCompleted() => SessionsUsed >= (Product?.SessionsIncluded ?? 0);
        // public bool CanUse() => Status == "active" && !IsExpired() && !IsCompleted();
    }
}