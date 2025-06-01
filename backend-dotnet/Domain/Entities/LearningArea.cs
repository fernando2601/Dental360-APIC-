using System;

namespace DentalSpa.Domain.Entities
{
    public class LearningArea
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsPublished { get; set; } = true;
        public int ViewCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool HasMedia() => !string.IsNullOrEmpty(ImageUrl) || !string.IsNullOrEmpty(VideoUrl);
        public bool IsPopular() => ViewCount > 100;
        public void IncrementView() => ViewCount++;
    }

    public class ClinicInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public string WorkingHours { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool HasSocialMedia() => !string.IsNullOrEmpty(Instagram) || !string.IsNullOrEmpty(Facebook);
        public bool HasWebsite() => !string.IsNullOrEmpty(Website);
    }
}