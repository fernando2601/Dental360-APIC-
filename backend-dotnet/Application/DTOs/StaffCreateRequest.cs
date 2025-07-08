namespace DentalSpa.Application.DTOs
{
    public class StaffCreateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public int PositionId { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public int YearsOfExperience { get; set; }
        public string? License { get; set; }
    }
} 