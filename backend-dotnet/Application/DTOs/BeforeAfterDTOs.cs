using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class BeforeAfterCreateRequest
    {
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Serviço é obrigatório")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Foto anterior é obrigatória")]
        public string BeforePhotoUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Foto posterior é obrigatória")]
        public string AfterPhotoUrl { get; set; } = string.Empty;

        public string? Notes { get; set; }

        [Required(ErrorMessage = "Data do tratamento é obrigatória")]
        public DateTime TreatmentDate { get; set; }

        public bool IsPublic { get; set; } = false;
    }

    public class BeforeAfterDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public string BeforePhotoUrl { get; set; } = string.Empty;
        public string AfterPhotoUrl { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime TreatmentDate { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public ClientDTO? Client { get; set; }
        public ServiceDTO? Service { get; set; }
        public bool HasBothPhotos { get; set; }
        public bool IsRecent { get; set; }
    }
}

namespace DentalSpa.Application.DTOs
{
    public class LearningAreaCreateRequest
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Conteúdo é obrigatório")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsPublished { get; set; } = true;
    }

    public class LearningAreaDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsPublished { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool HasMedia { get; set; }
        public bool IsPopular { get; set; }
    }

    public class ClinicInfoCreateRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Endereço é obrigatório")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        public string? Website { get; set; }
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }

        [Required(ErrorMessage = "Horário de funcionamento é obrigatório")]
        public string WorkingHours { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }
    }

    public class ClinicInfoDTO
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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool HasSocialMedia { get; set; }
        public bool HasWebsite { get; set; }
    }
}