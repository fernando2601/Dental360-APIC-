using System.Threading.Tasks;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Interfaces
{
    public interface IAuthService
    {
        Task<object?> LoginAsync(LoginRequest request);
        Task<object> RegisterAsync(RegisterRequest request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
        Task<object> GetProfileAsync(int userId);
        Task<object?> RefreshTokenAsync(RefreshTokenRequest request);
    }
} 