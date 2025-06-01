using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class ClientCreateRequest
    {
        [Required(ErrorMessage = "Nome completo é obrigatório")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string Phone { get; set; } = string.Empty;

        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Notes { get; set; }
    }

    public class ClientUpdateRequest
    {
        [Required(ErrorMessage = "Nome completo é obrigatório")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string Phone { get; set; } = string.Empty;

        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Notes { get; set; }
    }

    public class ClientDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Age { get; set; }
        public bool IsMinor { get; set; }
    }

    public class ClientSearchRequest
    {
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}