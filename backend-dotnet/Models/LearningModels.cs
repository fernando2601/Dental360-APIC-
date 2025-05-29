using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Models
{
    public class LearningContentModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty; // video, article, course, quiz
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty; // beginner, intermediate, advanced
        public string ContentUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public List<string> Tags { get; set; } = new();
        public decimal Rating { get; set; }
        public int TotalRatings { get; set; }
        public int ViewCount { get; set; }
        public int CompletionCount { get; set; }
    }

    public class LearningContentDetailedModel : LearningContentModel
    {
        public string CreatedByName { get; set; } = string.Empty;
        public List<LearningProgressModel> UserProgress { get; set; } = new();
        public List<LearningCommentModel> Comments { get; set; } = new();
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int UserRating { get; set; }
    }

    public class LearningProgressModel
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public int UserId { get; set; }
        public decimal ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TimeSpentMinutes { get; set; }
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; } = string.Empty;
    }

    public class LearningCommentModel
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; } = string.Empty;
    }

    public class CreateLearningContentRequest
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(2000, ErrorMessage = "Descrição deve ter no máximo 2000 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo de conteúdo é obrigatório")]
        public string ContentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        [StringLength(100, ErrorMessage = "Categoria deve ter no máximo 100 caracteres")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dificuldade é obrigatória")]
        public string Difficulty { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL do conteúdo é obrigatória")]
        [StringLength(500, ErrorMessage = "URL deve ter no máximo 500 caracteres")]
        public string ContentUrl { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "URL da thumbnail deve ter no máximo 500 caracteres")]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [Range(1, 600, ErrorMessage = "Duração deve estar entre 1 e 600 minutos")]
        public int DurationMinutes { get; set; }

        public bool IsActive { get; set; } = true;
        public List<string> Tags { get; set; } = new();
    }

    public class UpdateLearningContentRequest : CreateLearningContentRequest
    {
    }

    public class LearningContentResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Tags { get; set; } = new();
        public decimal Rating { get; set; }
        public int TotalRatings { get; set; }
        public int ViewCount { get; set; }
        public int CompletionCount { get; set; }
        public string FormattedDuration => $"{DurationMinutes} min";
        public string FormattedRating => $"{Rating:F1}/5";
    }

    public class LearningStatsResponse
    {
        public int TotalContent { get; set; }
        public int ActiveContent { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public decimal AverageCompletionRate { get; set; }
        public int TotalViewTime { get; set; }
        public List<CategoryStats> CategoryBreakdown { get; set; } = new();
        public List<PopularContent> MostPopular { get; set; } = new();
        public List<RecentActivity> RecentActivities { get; set; } = new();
    }

    public class CategoryStats
    {
        public string Category { get; set; } = string.Empty;
        public int ContentCount { get; set; }
        public int ViewCount { get; set; }
        public decimal CompletionRate { get; set; }
    }

    public class PopularContent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public decimal Rating { get; set; }
    }

    public class RecentActivity
    {
        public string UserName { get; set; } = string.Empty;
        public string ContentTitle { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty; // completed, started, rated
        public DateTime Timestamp { get; set; }
    }

    public class UpdateProgressRequest
    {
        [Range(0, 100, ErrorMessage = "Progresso deve estar entre 0 e 100")]
        public decimal ProgressPercentage { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Tempo gasto deve ser maior ou igual a zero")]
        public int TimeSpentMinutes { get; set; }

        public bool IsCompleted { get; set; }
    }

    public class AddCommentRequest
    {
        [Required(ErrorMessage = "Comentário é obrigatório")]
        [StringLength(1000, ErrorMessage = "Comentário deve ter no máximo 1000 caracteres")]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Avaliação deve estar entre 1 e 5")]
        public int Rating { get; set; }
    }
}