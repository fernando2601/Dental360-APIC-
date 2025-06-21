using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IAuthService
    {
        Task<object> LoginAsync(object request);
        Task<object> RegisterAsync(object request);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ForgotPasswordAsync(object request);
        Task<bool> ResetPasswordAsync(object request);
        Task<bool> ValidateTokenAsync(string token);
        Task<object> RefreshTokenAsync(object request);
        Task<bool> LogoutAsync(int userId);
        Task<object> GetProfileAsync(int userId);
    }
} 