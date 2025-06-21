using System;

namespace DentalSpa.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Campos para Reset de Senha
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        // Campos para Refresh Token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public bool IsAdmin() => Role == "admin";
        public bool IsStaff() => Role == "staff";
        public bool IsValidResetToken(string token) => ResetToken == token && ResetTokenExpiry > DateTime.UtcNow;
    }
}