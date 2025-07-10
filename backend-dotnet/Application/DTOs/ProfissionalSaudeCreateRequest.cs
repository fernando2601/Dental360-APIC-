namespace DentalSpa.Application.DTOs
{
    public class ProfissionalSaudeCreateRequest
    {
        public int StaffId { get; set; }
        public string? RegistroProfissional { get; set; }
        public string? TipoRegistro { get; set; }
        public string? Conselho { get; set; }
        public string? UF { get; set; }
        public DateTime? DataRegistro { get; set; }
        public string? Especialidade { get; set; }
    }
} 