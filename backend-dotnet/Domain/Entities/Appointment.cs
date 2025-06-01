using System;

namespace DentalSpa.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public int StaffId { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; } = "scheduled"; // scheduled, confirmed, completed, cancelled
        public string? Notes { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Client? Client { get; set; }
        public Service? Service { get; set; }
        public Staff? Staff { get; set; }

        public bool IsToday() => DateTime.Date == DateTime.Today;
        public bool IsPast() => DateTime < DateTime.Now;
        public bool CanBeCancelled() => DateTime > DateTime.Now.AddHours(24);
        public bool IsCompleted() => Status == "completed";
    }
}