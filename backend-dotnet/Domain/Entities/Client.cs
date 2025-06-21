namespace DentalSpa.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Birthday { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}