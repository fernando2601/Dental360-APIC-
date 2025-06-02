using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Domain.Entities
{
    public class ClinicInfo
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(300)]
        public string Address { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Website { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string OpeningTime { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string ClosingTime { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string WorkingDays { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string City { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string State { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string ZipCode { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string WhatsApp { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Instagram { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Facebook { get; set; } = string.Empty;
        
        public string? Logo { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}