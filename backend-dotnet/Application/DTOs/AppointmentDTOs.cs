using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreateAppointmentDto
    {
        [Required]
        public int ClientId { get; set; }
        
        [Required]
        public int ServiceId { get; set; }
        
        [Required]
        public int StaffId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        public TimeSpan Duration { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Agendado";
        
        public decimal? Price { get; set; }
    }

    public class UpdateAppointmentDto : CreateAppointmentDto
    {
        public int Id { get; set; }
    }

    public class AppointmentDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AppointmentSummaryDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? Price { get; set; }
    }

    public class RescheduleAppointmentDto
    {
        [Required]
        public DateTime NewDate { get; set; }
        
        [StringLength(500)]
        public string? Reason { get; set; }
    }

    public class CancelAppointmentDto
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}