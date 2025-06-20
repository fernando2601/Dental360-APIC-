namespace ClinicApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "user"; // admin, manager, user
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public User User { get; set; } = new();
        public DateTime ExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
    }

    public class DashboardMetrics
    {
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int LowStockAlerts { get; set; }
        public int ExpirationAlerts { get; set; }
        public int NewPatientsThisMonth { get; set; }
        public decimal AppointmentCompletionRate { get; set; }
        public List<RecentActivity> RecentActivities { get; set; } = new();
        public List<UpcomingAppointment> TodayAppointments { get; set; } = new();
        public List<QuickStat> QuickStats { get; set; } = new();
    }

    public class RecentActivity
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // appointment, payment, patient, inventory
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string FormattedTime { get; set; } = string.Empty;
        public string? Link { get; set; }
    }

    public class UpcomingAppointment
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public string FormattedTime { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
    }

    public class QuickStat
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Change { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty; // increase, decrease, neutral
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserPreferences Preferences { get; set; } = new();
        public UserStatistics Statistics { get; set; } = new();
    }

    public class UserPreferences
    {
        public string Theme { get; set; } = "light"; // light, dark, auto
        public string Language { get; set; } = "pt-BR";
        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public string TimeZone { get; set; } = "America/Sao_Paulo";
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public string TimeFormat { get; set; } = "HH:mm";
    }

    public class UserStatistics
    {
        public int TotalLogins { get; set; }
        public int AppointmentsCreated { get; set; }
        public int PatientsManaged { get; set; }
        public DateTime? LastActivity { get; set; }
        public int DaysActive { get; set; }
    }

    public class SystemInfo
    {
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveSessions { get; set; }
        public string DatabaseStatus { get; set; } = string.Empty;
        public List<SystemAlert> Alerts { get; set; } = new();
    }

    public class SystemAlert
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}