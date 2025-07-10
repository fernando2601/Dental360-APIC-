namespace DentalSpa.Application.DTOs
{
    public class PatientCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<int> ClinicIds { get; set; } = new();
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? MedicalHistory { get; set; }
        public string? Allergies { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public bool IsActive { get; set; } = true;
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string CPF { get; set; } = string.Empty;
        public string? RG { get; set; }
        public string EstadoNascimento { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Telefone { get; set; }
        public string? Endereco { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Segment { get; set; } = string.Empty;
    }
} 