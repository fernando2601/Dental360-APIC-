using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreateClientDto
    {
        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        [StringLength(20)]
        public string? Gender { get; set; }
        
        [StringLength(20)]
        public string? CPF { get; set; }
        
        [StringLength(20)]
        public string? RG { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [StringLength(1000)]
        public string? MedicalHistory { get; set; }
        
        [StringLength(500)]
        public string? Allergies { get; set; }
        
        [StringLength(500)]
        public string? Medications { get; set; }
        
        [StringLength(100)]
        public string? Occupation { get; set; }
        
        [StringLength(200)]
        public string? EmergencyContact { get; set; }
        
        [StringLength(20)]
        public string? EmergencyPhone { get; set; }
    }

    public class UpdateClientDto : CreateClientDto
    {
        public int Id { get; set; }
    }

    public class ClientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? CPF { get; set; }
        public string? RG { get; set; }
        public string? Notes { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Allergies { get; set; }
        public string? Medications { get; set; }
        public string? Occupation { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalAppointments { get; set; }
        public DateTime? LastAppointment { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class ClientSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? LastAppointment { get; set; }
        public int TotalAppointments { get; set; }
        public decimal TotalSpent { get; set; }
        public bool IsActive { get; set; }
    }

    public class PatientDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string CPF { get; set; } = string.Empty;
        public string? RG { get; set; }
        public string EstadoNascimento { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Endereco { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Notes { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Allergies { get; set; }
        public string? Medications { get; set; }
        public string? Occupation { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalAppointments { get; set; }
        public DateTime? LastAppointment { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class CreatePatientDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required]
        public int Idade { get; set; }
        [Required]
        public string CPF { get; set; } = string.Empty;
        public string? RG { get; set; }
        [Required]
        public string EstadoNascimento { get; set; } = string.Empty;
        [Required]
        public DateTime DataNascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Endereco { get; set; }
    }

    public class UpdatePatientDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required]
        public int Idade { get; set; }
        [Required]
        public string CPF { get; set; } = string.Empty;
        public string? RG { get; set; }
        [Required]
        public string EstadoNascimento { get; set; } = string.Empty;
        [Required]
        public DateTime DataNascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Endereco { get; set; }
    }
}