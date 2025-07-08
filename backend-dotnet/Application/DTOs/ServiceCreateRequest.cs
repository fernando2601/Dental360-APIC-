namespace DentalSpa.Application.DTOs
{
    public class ServiceCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public bool IsActive { get; set; } = true;
        public int ClinicId { get; set; }
        public List<int> StaffIds { get; set; } = new();
    }
} 