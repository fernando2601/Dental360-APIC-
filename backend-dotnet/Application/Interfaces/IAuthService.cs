using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IAuthService
    {
        Task<object> LoginAsync(string email, string password);
        Task<object> RegisterAsync(User user, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<bool> ValidateTokenAsync(string token);
        Task<object> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(int userId);
    }
} 