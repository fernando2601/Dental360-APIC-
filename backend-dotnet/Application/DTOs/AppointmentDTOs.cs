using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class AppointmentCreateRequest
    {
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Serviço é obrigatório")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Profissional é obrigatório")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "Data e hora são obrigatórias")]
        public DateTime DateTime { get; set; }

        public string? Notes { get; set; }
        public decimal? Price { get; set; }
    }

    public class AppointmentUpdateRequest
    {
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public int StaffId { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; } = "scheduled";
        public string? Notes { get; set; }
        public decimal? Price { get; set; }
    }

    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public int StaffId { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ClientDTO? Client { get; set; }
        public ServiceDTO? Service { get; set; }
        public StaffDTO? Staff { get; set; }

        public bool IsToday { get; set; }
        public bool IsPast { get; set; }
        public bool CanBeCancelled { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class AppointmentFilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ClientId { get; set; }
        public int? StaffId { get; set; }
        public string? Status { get; set; }
    }
}