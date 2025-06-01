using System;

namespace DentalSpa.Domain.Entities
{
    public class Staff
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Specialization { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User? User { get; set; }

        public bool IsSpecialist() => !string.IsNullOrEmpty(Specialization);
    }
}