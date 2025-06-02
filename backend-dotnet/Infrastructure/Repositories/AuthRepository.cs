using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace DentalSpa.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("DATABASE_URL") 
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    username as Username,
                    email as Email,
                    full_name as FullName,
                    password_hash as PasswordHash,
                    role as Role,
                    is_active as IsActive,
                    last_login as LastLogin,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM users 
                WHERE username = @Username AND is_active = true";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    username as Username,
                    email as Email,
                    full_name as FullName,
                    password_hash as PasswordHash,
                    role as Role,
                    is_active as IsActive,
                    last_login as LastLogin,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM users 
                WHERE email = @Email AND is_active = true";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    username as Username,
                    email as Email,
                    full_name as FullName,
                    password_hash as PasswordHash,
                    role as Role,
                    is_active as IsActive,
                    last_login as LastLogin,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM users 
                WHERE id = @Id AND is_active = true";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<User> CreateUserAsync(User user)
        {
            const string sql = @"
                INSERT INTO users (username, email, full_name, password_hash, role, is_active, created_at, updated_at)
                VALUES (@Username, @Email, @FullName, @PasswordHash, @Role, @IsActive, @CreatedAt, @UpdatedAt)
                RETURNING 
                    id as Id,
                    username as Username,
                    email as Email,
                    full_name as FullName,
                    password_hash as PasswordHash,
                    role as Role,
                    is_active as IsActive,
                    last_login as LastLogin,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            var now = DateTime.UtcNow;
            return await connection.QuerySingleAsync<User>(sql, new
            {
                user.Username,
                user.Email,
                user.FullName,
                user.PasswordHash,
                user.Role,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            const string sql = @"
                UPDATE users 
                SET 
                    username = @Username,
                    email = @Email,
                    full_name = @FullName,
                    role = @Role,
                    is_active = @IsActive,
                    updated_at = @UpdatedAt
                WHERE id = @Id
                RETURNING 
                    id as Id,
                    username as Username,
                    email as Email,
                    full_name as FullName,
                    password_hash as PasswordHash,
                    role as Role,
                    is_active as IsActive,
                    last_login as LastLogin,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new
            {
                Id = id,
                user.Username,
                user.Email,
                user.FullName,
                user.Role,
                user.IsActive,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            const string sql = @"
                UPDATE users 
                SET 
                    is_active = false,
                    updated_at = @UpdatedAt
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    username as Username,
                    email as Email,
                    full_name as FullName,
                    role as Role,
                    is_active as IsActive,
                    last_login as LastLogin,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM users 
                WHERE is_active = true
                ORDER BY created_at DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<User>(sql);
        }

        public async Task<User?> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null) return null;

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValidPassword ? user : null;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            const string sql = @"
                UPDATE users 
                SET 
                    last_login = @LastLogin,
                    updated_at = @UpdatedAt
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                Id = userId, 
                LastLogin = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow 
            });
            return rowsAffected > 0;
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            const string sql = @"
                UPDATE users 
                SET 
                    password_hash = @PasswordHash,
                    updated_at = @UpdatedAt
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                Id = userId, 
                PasswordHash = newPasswordHash,
                UpdatedAt = DateTime.UtcNow 
            });
            return rowsAffected > 0;
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            const string sql = "SELECT COUNT(*) FROM users WHERE username = @Username";
            
            using var connection = new NpgsqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(sql, new { Username = username });
            return count == 0;
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            const string sql = "SELECT COUNT(*) FROM users WHERE email = @Email";
            
            using var connection = new NpgsqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(sql, new { Email = email });
            return count == 0;
        }

        public async Task<DashboardMetrics> GetDashboardMetricsAsync(int? userId = null)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            // Métricas básicas
            const string metricsQuery = @"
                SELECT 
                    (SELECT COUNT(*) FROM patients WHERE is_active = true) as TotalPatients,
                    (SELECT COUNT(*) FROM appointments WHERE DATE(start_time) = CURRENT_DATE) as TodayAppointments,
                    (SELECT COUNT(*) FROM appointments WHERE status = 'pending' AND start_time >= CURRENT_TIMESTAMP) as PendingAppointments,
                    (SELECT COALESCE(SUM(amount), 0) FROM financial_transactions WHERE DATE(created_at) = CURRENT_DATE AND type = 'income') as TodayRevenue,
                    (SELECT COALESCE(SUM(amount), 0) FROM financial_transactions WHERE DATE_TRUNC('month', created_at) = DATE_TRUNC('month', CURRENT_DATE) AND type = 'income') as MonthlyRevenue,
                    (SELECT COUNT(*) FROM inventory_items WHERE quantity <= threshold AND is_active = true) as LowStockAlerts,
                    (SELECT COUNT(*) FROM inventory_items WHERE expiration_date <= CURRENT_DATE + INTERVAL '30 days' AND is_active = true) as ExpirationAlerts,
                    (SELECT COUNT(*) FROM patients WHERE DATE_TRUNC('month', created_at) = DATE_TRUNC('month', CURRENT_DATE)) as NewPatientsThisMonth";

            var metrics = await connection.QuerySingleAsync(metricsQuery);

            // Taxa de conclusão de consultas
            const string completionRateQuery = @"
                SELECT 
                    CASE 
                        WHEN COUNT(*) > 0 THEN 
                            ROUND((COUNT(CASE WHEN status = 'completed' THEN 1 END)::decimal / COUNT(*) * 100)::numeric, 2)
                        ELSE 0 
                    END as CompletionRate
                FROM appointments 
                WHERE DATE_TRUNC('month', start_time) = DATE_TRUNC('month', CURRENT_DATE)";

            var completionRate = await connection.QuerySingleAsync<decimal>(completionRateQuery);

            return new DashboardMetrics
            {
                TotalPatients = metrics.TotalPatients,
                TodayAppointments = metrics.TodayAppointments,
                PendingAppointments = metrics.PendingAppointments,
                TodayRevenue = metrics.TodayRevenue,
                MonthlyRevenue = metrics.MonthlyRevenue,
                LowStockAlerts = metrics.LowStockAlerts,
                ExpirationAlerts = metrics.ExpirationAlerts,
                NewPatientsThisMonth = metrics.NewPatientsThisMonth,
                AppointmentCompletionRate = completionRate,
                RecentActivities = (await GetRecentActivitiesAsync(5)).ToList(),
                TodayAppointments = (await GetTodayAppointmentsAsync()).ToList(),
                QuickStats = await GenerateQuickStatsAsync()
            };
        }

        public async Task<IEnumerable<RecentActivity>> GetRecentActivitiesAsync(int limit = 10)
        {
            const string sql = @"
                (SELECT 
                    id,
                    'appointment' as Type,
                    'Nova consulta agendada: ' || p.name as Description,
                    'calendar' as Icon,
                    '#10B981' as Color,
                    created_at as Timestamp
                FROM appointments a
                INNER JOIN patients p ON a.patient_id = p.id
                WHERE a.created_at >= CURRENT_DATE - INTERVAL '7 days')
                
                UNION ALL
                
                (SELECT 
                    id,
                    'payment' as Type,
                    'Pagamento recebido: R$ ' || amount::text as Description,
                    'dollar-sign' as Icon,
                    '#3B82F6' as Color,
                    created_at as Timestamp
                FROM financial_transactions 
                WHERE type = 'income' AND created_at >= CURRENT_DATE - INTERVAL '7 days')
                
                UNION ALL
                
                (SELECT 
                    id,
                    'patient' as Type,
                    'Novo paciente cadastrado: ' || name as Description,
                    'user-plus' as Icon,
                    '#8B5CF6' as Color,
                    created_at as Timestamp
                FROM patients 
                WHERE created_at >= CURRENT_DATE - INTERVAL '7 days')
                
                ORDER BY Timestamp DESC
                LIMIT @Limit";

            using var connection = new NpgsqlConnection(_connectionString);
            var activities = await connection.QueryAsync<RecentActivity>(sql, new { Limit = limit });

            return activities.Select(a => new RecentActivity
            {
                Id = a.Id,
                Type = a.Type,
                Description = a.Description,
                Icon = a.Icon,
                Color = a.Color,
                Timestamp = a.Timestamp,
                FormattedTime = FormatTimeAgo(a.Timestamp)
            });
        }

        public async Task<IEnumerable<UpcomingAppointment>> GetTodayAppointmentsAsync()
        {
            const string sql = @"
                SELECT 
                    a.id as Id,
                    p.name as PatientName,
                    s.name as ServiceName,
                    st.name as StaffName,
                    a.start_time as StartTime,
                    a.status as Status,
                    a.room as Room,
                    EXTRACT(EPOCH FROM (a.end_time - a.start_time))/60 as DurationMinutes
                FROM appointments a
                INNER JOIN patients p ON a.patient_id = p.id
                LEFT JOIN services s ON a.service_id = s.id
                LEFT JOIN staff st ON a.staff_id = st.id
                WHERE DATE(a.start_time) = CURRENT_DATE
                ORDER BY a.start_time";

            using var connection = new NpgsqlConnection(_connectionString);
            var appointments = await connection.QueryAsync(sql);

            return appointments.Select(a => new UpcomingAppointment
            {
                Id = a.Id,
                PatientName = a.PatientName,
                ServiceName = a.ServiceName ?? "Consulta",
                StaffName = a.StaffName ?? "N/A",
                StartTime = a.StartTime,
                FormattedTime = a.StartTime.ToString("HH:mm"),
                Status = a.Status,
                StatusColor = GetStatusColor(a.Status),
                Room = a.Room ?? "N/A",
                Duration = $"{a.DurationMinutes}min"
            });
        }

        public async Task<SystemInfo> GetSystemInfoAsync()
        {
            const string sql = @"
                SELECT 
                    COUNT(*) as TotalUsers,
                    COUNT(CASE WHEN last_login >= CURRENT_DATE THEN 1 END) as ActiveToday
                FROM users 
                WHERE is_active = true";

            using var connection = new NpgsqlConnection(_connectionString);
            var systemData = await connection.QuerySingleAsync(sql);

            return new SystemInfo
            {
                Version = "1.0.0",
                Environment = "Production",
                LastUpdate = DateTime.UtcNow.AddDays(-5),
                TotalUsers = systemData.TotalUsers,
                ActiveSessions = systemData.ActiveToday,
                DatabaseStatus = "Online",
                Alerts = new List<SystemAlert>()
            };
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    username as Username,
                    email as Email,
                    full_name as FullName,
                    role as Role,
                    last_login as LastLogin,
                    created_at as CreatedAt
                FROM users 
                WHERE id = @UserId AND is_active = true";

            using var connection = new NpgsqlConnection(_connectionString);
            var user = await connection.QuerySingleOrDefaultAsync(sql, new { UserId = userId });

            if (user == null) return null;

            var statistics = await GetUserStatisticsAsync(userId);

            return new UserProfile
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                Preferences = new UserPreferences(),
                Statistics = statistics
            };
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UserProfile profile)
        {
            const string sql = @"
                UPDATE users 
                SET 
                    full_name = @FullName,
                    email = @Email,
                    updated_at = @UpdatedAt
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = userId,
                profile.FullName,
                profile.Email,
                UpdatedAt = DateTime.UtcNow
            });
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateUserPreferencesAsync(int userId, UserPreferences preferences)
        {
            // Em uma implementação real, isso seria salvo em uma tabela separada
            return true;
        }

        public async Task<UserStatistics> GetUserStatisticsAsync(int userId)
        {
            const string sql = @"
                SELECT 
                    COALESCE(
                        (SELECT COUNT(*) FROM appointments WHERE created_by = @UserId), 0
                    ) as AppointmentsCreated,
                    COALESCE(
                        (SELECT COUNT(DISTINCT patient_id) FROM appointments WHERE created_by = @UserId), 0
                    ) as PatientsManaged";

            using var connection = new NpgsqlConnection(_connectionString);
            var stats = await connection.QuerySingleOrDefaultAsync(sql, new { UserId = userId });

            return new UserStatistics
            {
                TotalLogins = 0,
                AppointmentsCreated = stats?.AppointmentsCreated ?? 0,
                PatientsManaged = stats?.PatientsManaged ?? 0,
                LastActivity = DateTime.UtcNow,
                DaysActive = 30
            };
        }

        // Métodos auxiliares privados
        private async Task<List<QuickStat>> GenerateQuickStatsAsync()
        {
            return new List<QuickStat>
            {
                new QuickStat
                {
                    Label = "Consultas Hoje",
                    Value = "12",
                    Change = "+2",
                    ChangeType = "increase",
                    Icon = "calendar",
                    Color = "#10B981"
                },
                new QuickStat
                {
                    Label = "Receita Mensal",
                    Value = "R$ 45.200",
                    Change = "+15%",
                    ChangeType = "increase",
                    Icon = "trending-up",
                    Color = "#3B82F6"
                },
                new QuickStat
                {
                    Label = "Pacientes Ativos",
                    Value = "328",
                    Change = "+8",
                    ChangeType = "increase",
                    Icon = "users",
                    Color = "#8B5CF6"
                }
            };
        }

        private string FormatTimeAgo(DateTime timestamp)
        {
            var timeSpan = DateTime.UtcNow - timestamp;
            
            if (timeSpan.TotalMinutes < 1) return "Agora";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}min atrás";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h atrás";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}d atrás";
            
            return timestamp.ToString("dd/MM");
        }

        private string GetStatusColor(string status)
        {
            return status switch
            {
                "confirmed" => "#10B981",
                "pending" => "#F59E0B",
                "cancelled" => "#EF4444",
                "completed" => "#3B82F6",
                _ => "#6B7280"
            };
        }

        // Implementações simplificadas para métodos restantes
        public async Task<bool> CreatePasswordResetTokenAsync(string email, string token)
        {
            return true;
        }

        public async Task<bool> ValidatePasswordResetTokenAsync(string email, string token)
        {
            return true;
        }

        public async Task<bool> ConsumePasswordResetTokenAsync(string email, string token)
        {
            return true;
        }

        public async Task<bool> CreateSessionAsync(int userId, string sessionToken)
        {
            return true;
        }

        public async Task<bool> ValidateSessionAsync(string sessionToken)
        {
            return true;
        }

        public async Task<bool> RevokeSessionAsync(string sessionToken)
        {
            return true;
        }

        public async Task<bool> RevokeAllUserSessionsAsync(int userId)
        {
            return true;
        }

        public async Task<bool> LogUserActivityAsync(int userId, string activity, string details = "")
        {
            return true;
        }

        public async Task<IEnumerable<object>> GetUserActivityLogAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return new List<object>();
        }

        public async Task<IEnumerable<SystemAlert>> GetSystemAlertsAsync(string? severity = null)
        {
            return new List<SystemAlert>();
        }
    }
}