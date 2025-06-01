using System;

namespace DentalSpa.Domain.Entities
{
    public class BeforeAfter
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public string BeforePhotoUrl { get; set; } = string.Empty;
        public string AfterPhotoUrl { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime TreatmentDate { get; set; }
        public bool IsPublic { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Client? Client { get; set; }
        public Service? Service { get; set; }

        public bool HasBothPhotos() => !string.IsNullOrEmpty(BeforePhotoUrl) && !string.IsNullOrEmpty(AfterPhotoUrl);
        public bool IsRecent() => TreatmentDate > DateTime.Now.AddDays(-30);
    }
}