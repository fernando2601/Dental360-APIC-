using System;

namespace DentalSpa.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int GetAge()
        {
            if (!Birthday.HasValue) return 0;
            var today = DateTime.Today;
            var age = today.Year - Birthday.Value.Year;
            if (Birthday.Value.Date > today.AddYears(-age)) age--;
            return age;
        }

        public bool IsMinor() => GetAge() < 18;
    }
}