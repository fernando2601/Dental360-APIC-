using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Domain.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(20)]
        public string BillingCycle { get; set; } = string.Empty; // Monthly, Quarterly, Yearly
        
        public int MaxClients { get; set; }
        
        public int MaxAppointments { get; set; }
        
        public bool HasInventoryManagement { get; set; }
        
        public bool HasFinancialReports { get; set; }
        
        public bool HasWhatsAppIntegration { get; set; }
        
        public bool HasBackup { get; set; }
        
        public bool HasSupport { get; set; }
        
        [StringLength(50)]
        public string SupportLevel { get; set; } = string.Empty; // Basic, Premium, Enterprise
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<ClientSubscription> ClientSubscriptions { get; set; } = new List<ClientSubscription>();
    }

    public class ClientSubscription
    {
        public int Id { get; set; }
        
        [Required]
        public int ClientId { get; set; }
        
        [Required]
        public int SubscriptionId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty; // Active, Suspended, Cancelled, Expired
        
        public decimal PaidAmount { get; set; }
        
        public DateTime? LastPaymentDate { get; set; }
        
        public DateTime? NextPaymentDate { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Client Client { get; set; } = null!;
        public virtual Subscription Subscription { get; set; } = null!;
    }
}