using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreatePackageDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal? OriginalPrice { get; set; }
        
        [Range(0, 100)]
        public decimal? DiscountPercentage { get; set; }
        
        public List<int> ServiceIds { get; set; } = new();
        
        [Range(1, int.MaxValue)]
        public int ValidityDays { get; set; } = 365;
        
        [Range(1, int.MaxValue)]
        public int MaxUsage { get; set; } = 1;
        
        [StringLength(500)]
        public string? Terms { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool RequiresConsultation { get; set; }
        
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
    }

    public class UpdatePackageDto : CreatePackageDto
    {
        public int Id { get; set; }
    }

    public class PackageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public List<ServiceSummaryDto> Services { get; set; } = new();
        public int ValidityDays { get; set; }
        public int MaxUsage { get; set; }
        public string? Terms { get; set; }
        public bool IsActive { get; set; }
        public bool RequiresConsultation { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal Savings { get; set; }
    }

    public class PackageSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int ServiceCount { get; set; }
    }

    public class ClientPackageDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int UsageCount { get; set; }
        public int MaxUsage { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PaidAmount { get; set; }
        public string? Notes { get; set; }
    }
}