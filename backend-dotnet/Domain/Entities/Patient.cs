using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? MedicalHistory { get; set; }
        public string? Allergies { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string CPF { get; set; } = string.Empty;
        public string? RG { get; set; }
        public string EstadoNascimento { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Telefone { get; set; }
        public string? Endereco { get; set; }
    }
}