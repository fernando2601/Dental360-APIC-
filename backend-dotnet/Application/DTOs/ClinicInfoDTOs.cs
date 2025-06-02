using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class ClinicInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? SocialMedia { get; set; }
        public string OpeningHours { get; set; } = string.Empty;
        public string Services { get; set; } = string.Empty;
        public string? About { get; set; }
        public string? Mission { get; set; }
        public string? Vision { get; set; }
        public string? Values { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateClinicInfoDto
    {
        [Required(ErrorMessage = "Nome da clínica é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Endereço é obrigatório")]
        [StringLength(500, ErrorMessage = "Endereço deve ter no máximo 500 caracteres")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Url(ErrorMessage = "Formato de URL inválido")]
        public string? Website { get; set; }

        [StringLength(500, ErrorMessage = "Redes sociais deve ter no máximo 500 caracteres")]
        public string? SocialMedia { get; set; }

        [Required(ErrorMessage = "Horário de funcionamento é obrigatório")]
        [StringLength(200, ErrorMessage = "Horário deve ter no máximo 200 caracteres")]
        public string OpeningHours { get; set; } = string.Empty;

        [Required(ErrorMessage = "Serviços são obrigatórios")]
        [StringLength(1000, ErrorMessage = "Serviços deve ter no máximo 1000 caracteres")]
        public string Services { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Sobre deve ter no máximo 2000 caracteres")]
        public string? About { get; set; }

        [StringLength(500, ErrorMessage = "Missão deve ter no máximo 500 caracteres")]
        public string? Mission { get; set; }

        [StringLength(500, ErrorMessage = "Visão deve ter no máximo 500 caracteres")]
        public string? Vision { get; set; }

        [StringLength(500, ErrorMessage = "Valores deve ter no máximo 500 caracteres")]
        public string? Values { get; set; }
    }

    public class UpdateClinicInfoDto
    {
        [Required(ErrorMessage = "Nome da clínica é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Endereço é obrigatório")]
        [StringLength(500, ErrorMessage = "Endereço deve ter no máximo 500 caracteres")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Url(ErrorMessage = "Formato de URL inválido")]
        public string? Website { get; set; }

        [StringLength(500, ErrorMessage = "Redes sociais deve ter no máximo 500 caracteres")]
        public string? SocialMedia { get; set; }

        [Required(ErrorMessage = "Horário de funcionamento é obrigatório")]
        [StringLength(200, ErrorMessage = "Horário deve ter no máximo 200 caracteres")]
        public string OpeningHours { get; set; } = string.Empty;

        [Required(ErrorMessage = "Serviços são obrigatórios")]
        [StringLength(1000, ErrorMessage = "Serviços deve ter no máximo 1000 caracteres")]
        public string Services { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Sobre deve ter no máximo 2000 caracteres")]
        public string? About { get; set; }

        [StringLength(500, ErrorMessage = "Missão deve ter no máximo 500 caracteres")]
        public string? Mission { get; set; }

        [StringLength(500, ErrorMessage = "Visão deve ter no máximo 500 caracteres")]
        public string? Vision { get; set; }

        [StringLength(500, ErrorMessage = "Valores deve ter no máximo 500 caracteres")]
        public string? Values { get; set; }
    }
}