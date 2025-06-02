using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Domain.Entities
{
    public class LearningArea
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(300)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public string? VideoUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsPublished { get; set; } = true;
        
        public int ViewCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool HasMedia() => !string.IsNullOrEmpty(ImageUrl) || !string.IsNullOrEmpty(VideoUrl);
        public bool IsPopular() => ViewCount > 100;
        public void IncrementView() => ViewCount++;
    }
}