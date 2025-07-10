namespace DentalSpa.Domain.Entities
{
    public class PatientClinic
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public int ClinicId { get; set; }
        public ClinicInfo Clinic { get; set; } = null!;
    }
} 