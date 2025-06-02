using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreateStaffDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Specialization { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? LicenseNumber { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal? HourlyRate { get; set; }
        
        [Range(0, 100)]
        public decimal? CommissionRate { get; set; }
        
        [StringLength(500)]
        public string? Qualifications { get; set; }
        
        [StringLength(1000)]
        public string? Bio { get; set; }
        
        public DateTime? HireDate { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Ativo";
        
        public bool IsActive { get; set; } = true;
        
        [StringLength(200)]
        public string? WorkingHours { get; set; }
    }

    public class UpdateStaffDto : CreateStaffDto
    {
        public int Id { get; set; }
    }

    public class StaffDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? CommissionRate { get; set; }
        public string? Qualifications { get; set; }
        public string? Bio { get; set; }
        public DateTime? HireDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? WorkingHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class StaffSummaryDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int TotalAppointments { get; set; }
    }
}