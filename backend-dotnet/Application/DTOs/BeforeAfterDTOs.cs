using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreateBeforeAfterDto
    {
        [Required]
        public int ClientId { get; set; }
        
        [Required]
        public int ServiceId { get; set; }
        
        [Required]
        public string BeforePhotoUrl { get; set; } = string.Empty;
        
        [Required]
        public string AfterPhotoUrl { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? TreatmentDetails { get; set; }
        
        public DateTime? TreatmentDate { get; set; }
        
        public bool IsPublic { get; set; } = false;
        
        public bool ClientConsent { get; set; } = false;
        
        [StringLength(200)]
        public string? Tags { get; set; }
        
        [Range(1, 5)]
        public int? Rating { get; set; }
        
        [StringLength(1000)]
        public string? ClientTestimonial { get; set; }
    }

    public class UpdateBeforeAfterDto : CreateBeforeAfterDto
    {
        public int Id { get; set; }
    }

    public class BeforeAfterDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string BeforePhotoUrl { get; set; } = string.Empty;
        public string AfterPhotoUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? TreatmentDetails { get; set; }
        public DateTime? TreatmentDate { get; set; }
        public bool IsPublic { get; set; }
        public bool ClientConsent { get; set; }
        public string? Tags { get; set; }
        public int? Rating { get; set; }
        public string? ClientTestimonial { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
    }

    public class BeforeAfterSummaryDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string BeforePhotoUrl { get; set; } = string.Empty;
        public string AfterPhotoUrl { get; set; } = string.Empty;
        public DateTime? TreatmentDate { get; set; }
        public bool IsPublic { get; set; }
        public int? Rating { get; set; }
    }

    public class GalleryItemDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string BeforePhotoUrl { get; set; } = string.Empty;
        public string AfterPhotoUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? TreatmentDate { get; set; }
        public string? Tags { get; set; }
        public int? Rating { get; set; }
        public string? ClientTestimonial { get; set; }
    }
}