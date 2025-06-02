using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreateServiceDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        public TimeSpan Duration { get; set; }
        
        [StringLength(500)]
        public string? Requirements { get; set; }
        
        [StringLength(500)]
        public string? AfterCare { get; set; }
        
        public bool RequiresConsultation { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateServiceDto : CreateServiceDto
    {
        public int Id { get; set; }
    }

    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Requirements { get; set; }
        public string? AfterCare { get; set; }
        public bool RequiresConsultation { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ServiceSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsActive { get; set; }
    }
}