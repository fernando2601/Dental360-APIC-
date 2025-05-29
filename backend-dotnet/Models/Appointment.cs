namespace ClinicApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "scheduled";
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class AppointmentWithDetails
    {
        public int Id { get; set; }
        public string Procedimento { get; set; } = string.Empty;
        public string? Recorrencia { get; set; }
        public ClientInfo Paciente { get; set; } = new();
        public ProfessionalInfo Profissional { get; set; } = new();
        public int Duracao { get; set; }
        public DateTime Data { get; set; }
        public string DataFormatada { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusLabel { get; set; } = string.Empty;
        public string Convenio { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public string Comanda { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string ValorFormatado { get; set; } = string.Empty;
    }

    public class ClientInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ProfessionalInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;
    }

    public class CreateAppointmentDto
    {
        public int ClientId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}