using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreateSubscriptionDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(20)]
        public string BillingCycle { get; set; } = "Monthly";
        
        public int MaxClients { get; set; }
        
        public int MaxAppointments { get; set; }
        
        public bool HasInventoryManagement { get; set; }
        
        public bool HasFinancialReports { get; set; }
        
        public bool HasWhatsAppIntegration { get; set; }
        
        public bool HasBackup { get; set; }
        
        public bool HasSupport { get; set; }
        
        [StringLength(50)]
        public string SupportLevel { get; set; } = "Basic";
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSubscriptionDto : CreateSubscriptionDto
    {
        public int Id { get; set; }
    }

    public class SubscriptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public int MaxClients { get; set; }
        public int MaxAppointments { get; set; }
        public bool HasInventoryManagement { get; set; }
        public bool HasFinancialReports { get; set; }
        public bool HasWhatsAppIntegration { get; set; }
        public bool HasBackup { get; set; }
        public bool HasSupport { get; set; }
        public string SupportLevel { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ActiveSubscriptions { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }

    public class CreateClientSubscriptionDto
    {
        [Required]
        public int ClientId { get; set; }
        
        [Required]
        public int SubscriptionId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PaidAmount { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateClientSubscriptionDto : CreateClientSubscriptionDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";
    }

    public class ClientSubscriptionDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PaidAmount { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; }
    }
}