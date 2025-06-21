namespace DentalSpa.Domain.Entities
{
    public class Staff
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public int YearsOfExperience { get; set; }
        public string? License { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ManagerId { get; set; }
    }
}