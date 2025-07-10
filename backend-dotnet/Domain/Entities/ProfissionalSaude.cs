namespace DentalSpa.Domain.Entities
{
    public class ProfissionalSaude
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;
        public string? RegistroProfissional { get; set; } // CRM, CRO, CRP, etc.
        public string? TipoRegistro { get; set; } // CRM, CRO, CRP, etc.
        public string? Conselho { get; set; } // Conselho Regional
        public string? UF { get; set; } // Estado do registro
        public DateTime? DataRegistro { get; set; }
        public string? Especialidade { get; set; }
    }
} 