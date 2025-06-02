using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class CreateUserDto
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome completo é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome completo deve ter no máximo 200 caracteres")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Papel/função é obrigatório")]
        [StringLength(50, ErrorMessage = "Papel deve ter no máximo 50 caracteres")]
        public string Role { get; set; } = "staff";

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome completo é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome completo deve ter no máximo 200 caracteres")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Papel/função é obrigatório")]
        [StringLength(50, ErrorMessage = "Papel deve ter no máximo 50 caracteres")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string? Phone { get; set; }

        public bool IsActive { get; set; }
    }

    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Senha atual é obrigatória")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Nova senha deve ter entre 6 e 100 caracteres")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("NewPassword", ErrorMessage = "Nova senha e confirmação devem coincidir")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}