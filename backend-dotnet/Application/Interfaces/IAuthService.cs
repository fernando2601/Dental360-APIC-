using System.Threading.Tasks;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Interfaces
{
    public interface IAuthService
    {
        Task<object?> LoginAsync(LoginRequest request);
        Task<object> RegisterAsync(RegisterRequest request);
        Task<bool> ForgotPasswordAsync(object request);
        Task<bool> ResetPasswordAsync(object request);
        Task<object> GetProfileAsync(int userId);
        Task<object> RefreshTokenAsync(object request);
    }
} 