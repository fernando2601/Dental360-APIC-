namespace DentalSpa.Domain.Entities
{
    public class StaffClinic
    {
        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;
        public int ClinicId { get; set; }
        public ClinicInfo Clinic { get; set; } = null!;
    }
} 