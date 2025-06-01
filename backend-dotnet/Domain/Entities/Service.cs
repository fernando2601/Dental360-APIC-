using System;

namespace DentalSpa.Domain.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; } // em minutos
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsExpensive() => Price > 500;
        public bool IsLongDuration() => Duration > 120;
        public TimeSpan GetDurationTimeSpan() => TimeSpan.FromMinutes(Duration);
    }
}