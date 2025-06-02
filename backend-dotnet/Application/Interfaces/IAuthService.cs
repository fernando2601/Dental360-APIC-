using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IAuthService
    {
        // Authentication
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<User> RegisterAsync(RegisterRequest request);
        Task<bool> LogoutAsync(string token);
        Task<LoginResponse> RefreshTokenAsync(string refreshToken);

        // User Management
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> UpdateUserAsync(int id, User user);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();

        // Password Reset
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
        Task<bool> ValidateResetTokenAsync(string token, string email);

        // Dashboard & Analytics
        Task<DashboardMetrics> GetDashboardMetricsAsync(int? userId = null);
        Task<IEnumerable<RecentActivity>> GetRecentActivitiesAsync(int limit = 10);
        Task<SystemInfo> GetSystemInfoAsync();

        // User Profile
        Task<UserProfile?> GetUserProfileAsync(int userId);
        Task<bool> UpdateUserProfileAsync(int userId, UserProfile profile);
        Task<UserStatistics> GetUserStatisticsAsync(int userId);

        // Validation
        Task<bool> ValidateTokenAsync(string token);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);

        // Session Management
        Task<bool> RevokeAllSessionsAsync(int userId);
        Task<IEnumerable<object>> GetActiveSessionsAsync(int userId);

        // Security & Audit
        Task<IEnumerable<object>> GetUserActivityLogAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> LogSecurityEventAsync(int userId, string eventType, string details);
        Task<IEnumerable<SystemAlert>> GetSecurityAlertsAsync();
    }
}