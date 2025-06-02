using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Models
{
    public class BeforeAfterModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TreatmentType { get; set; } = string.Empty;
        public string BeforeImageUrl { get; set; } = string.Empty;
        public string AfterImageUrl { get; set; } = string.Empty;
        public int? PatientId { get; set; }
        public string PatientAge { get; set; } = string.Empty;
        public string PatientGender { get; set; } = string.Empty;
        public DateTime TreatmentDate { get; set; }
        public string DentistName { get; set; } = string.Empty;
        public string TreatmentDetails { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int ViewCount { get; set; } = 0;
        public double Rating { get; set; } = 0;
        public int RatingCount { get; set; } = 0;
        public List<string> Tags { get; set; } = new();
    }

    public class CreateBeforeAfterRequest
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo de tratamento é obrigatório")]
        [StringLength(100, ErrorMessage = "Tipo de tratamento deve ter no máximo 100 caracteres")]
        public string TreatmentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Imagem do antes é obrigatória")]
        [StringLength(500, ErrorMessage = "URL da imagem deve ter no máximo 500 caracteres")]
        public string BeforeImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Imagem do depois é obrigatória")]
        [StringLength(500, ErrorMessage = "URL da imagem deve ter no máximo 500 caracteres")]
        public string AfterImageUrl { get; set; } = string.Empty;

        public int? PatientId { get; set; }

        [StringLength(20, ErrorMessage = "Idade deve ter no máximo 20 caracteres")]
        public string PatientAge { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Gênero deve ter no máximo 20 caracteres")]
        public string PatientGender { get; set; } = string.Empty;

        public DateTime TreatmentDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Nome do dentista é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome do dentista deve ter no máximo 200 caracteres")]
        public string DentistName { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Detalhes do tratamento devem ter no máximo 2000 caracteres")]
        public string TreatmentDetails { get; set; } = string.Empty;

        public bool IsPublic { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public List<string> Tags { get; set; } = new();
    }

    public class UpdateBeforeAfterRequest : CreateBeforeAfterRequest
    {
    }

    public class BeforeAfterResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TreatmentType { get; set; } = string.Empty;
        public string BeforeImageUrl { get; set; } = string.Empty;
        public string AfterImageUrl { get; set; } = string.Empty;
        public int? PatientId { get; set; }
        public string PatientAge { get; set; } = string.Empty;
        public string PatientGender { get; set; } = string.Empty;
        public DateTime TreatmentDate { get; set; }
        public string DentistName { get; set; } = string.Empty;
        public string TreatmentDetails { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public List<string> Tags { get; set; } = new();
        
        // Computed properties
        public string FormattedTreatmentDate => TreatmentDate.ToString("dd/MM/yyyy");
        public string FormattedRating => Rating > 0 ? $"{Rating:F1} ⭐" : "Sem avaliações";
        public string PatientInfo => !string.IsNullOrEmpty(PatientAge) && !string.IsNullOrEmpty(PatientGender) 
            ? $"{PatientAge} anos, {PatientGender}" 
            : "Não informado";
    }

    public class BeforeAfterStatsResponse
    {
        public int TotalCases { get; set; }
        public int PublicCases { get; set; }
        public int PrivateCases { get; set; }
        public int TotalViews { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public List<TreatmentTypeStats> TreatmentBreakdown { get; set; } = new();
        public List<PopularCase> MostViewed { get; set; } = new();
        public List<MonthlyStats> MonthlyData { get; set; } = new();
    }

    public class TreatmentTypeStats
    {
        public string TreatmentType { get; set; } = string.Empty;
        public int CaseCount { get; set; }
        public double AverageRating { get; set; }
        public int TotalViews { get; set; }
    }

    public class PopularCase
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TreatmentType { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public double Rating { get; set; }
        public string BeforeImageUrl { get; set; } = string.Empty;
        public string AfterImageUrl { get; set; } = string.Empty;
    }

    public class MonthlyStats
    {
        public string Month { get; set; } = string.Empty;
        public int CasesAdded { get; set; }
        public int TotalViews { get; set; }
        public double AverageRating { get; set; }
    }

    public class BeforeAfterRatingModel
    {
        public int Id { get; set; }
        public int BeforeAfterId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateRatingRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Avaliação é obrigatória")]
        [Range(1, 5, ErrorMessage = "Avaliação deve estar entre 1 e 5")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comentário deve ter no máximo 500 caracteres")]
        public string Comment { get; set; } = string.Empty;
    }
}