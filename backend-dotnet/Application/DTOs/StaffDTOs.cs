using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class StaffCreateRequest
    {
        [Required(ErrorMessage = "Usuário é obrigatório")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Especialização é obrigatória")]
        public string Specialization { get; set; } = string.Empty;

        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class StaffUpdateRequest
    {
        [Required(ErrorMessage = "Especialização é obrigatória")]
        public string Specialization { get; set; } = string.Empty;

        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class StaffDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Specialization { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDTO? User { get; set; }
        public bool IsSpecialist { get; set; }
    }
}