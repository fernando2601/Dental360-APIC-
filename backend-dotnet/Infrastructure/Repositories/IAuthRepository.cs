using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IAuthRepository
    {
        // User Management
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();

        // Authentication
        Task<User?> ValidateUserCredentialsAsync(string username, string password);
        Task<bool> UpdateLastLoginAsync(int userId);
        Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);

        // Dashboard Analytics
        Task<DashboardMetrics> GetDashboardMetricsAsync(int? userId = null);
        Task<IEnumerable<RecentActivity>> GetRecentActivitiesAsync(int limit = 10);
        Task<IEnumerable<UpcomingAppointment>> GetTodayAppointmentsAsync();
        Task<SystemInfo> GetSystemInfoAsync();

        // User Profile & Preferences
        Task<UserProfile?> GetUserProfileAsync(int userId);
        Task<bool> UpdateUserProfileAsync(int userId, UserProfile profile);
        Task<bool> UpdateUserPreferencesAsync(int userId, UserPreferences preferences);
        Task<UserStatistics> GetUserStatisticsAsync(int userId);

        // Password Reset
        Task<bool> CreatePasswordResetTokenAsync(string email, string token);
        Task<bool> ValidatePasswordResetTokenAsync(string email, string token);
        Task<bool> ConsumePasswordResetTokenAsync(string email, string token);

        // Session Management
        Task<bool> CreateSessionAsync(int userId, string sessionToken);
        Task<bool> ValidateSessionAsync(string sessionToken);
        Task<bool> RevokeSessionAsync(string sessionToken);
        Task<bool> RevokeAllUserSessionsAsync(int userId);

        // Audit & Logging
        Task<bool> LogUserActivityAsync(int userId, string activity, string details = "");
        Task<IEnumerable<object>> GetUserActivityLogAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<SystemAlert>> GetSystemAlertsAsync(string? severity = null);
    }
}