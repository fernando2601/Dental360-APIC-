using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentalSpa.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, EmailService emailService)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Validar credenciais
            var user = await _authRepository.ValidateUserCredentialsAsync(request.Username, request.Password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Credenciais inválidas");
            }

            // Atualizar último login
            await _authRepository.UpdateLastLoginAsync(user.Id);

            // Gerar tokens
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpires = DateTime.UtcNow.AddDays(7);

            // Criar sessão (salvar refresh token)
            await _authRepository.CreateSessionAsync(user.Id, refreshToken, refreshTokenExpires);

            return new LoginResponse
            {
                Token = token,
                User = new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role
                },
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                RefreshToken = refreshToken
            };
        }

        public async Task<User> RegisterAsync(RegisterRequest request)
        {
            // Validações
            if (request.Password != request.ConfirmPassword)
            {
                throw new ArgumentException("Senhas não coincidem");
            }

            if (!await _authRepository.IsUsernameAvailableAsync(request.Username))
            {
                throw new ArgumentException("Nome de usuário já existe");
            }

            if (!await _authRepository.IsEmailAvailableAsync(request.Email))
            {
                throw new ArgumentException("Email já está em uso");
            }

            // Validar força da senha
            if (!IsValidPassword(request.Password))
            {
                throw new ArgumentException("Senha deve ter pelo menos 8 caracteres, incluindo maiúscula, minúscula e número");
            }

            // Criar usuário
            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLower(),
                FullName = request.FullName.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = string.IsNullOrEmpty(request.Role) ? "user" : request.Role,
                IsActive = true
            };

            var createdUser = await _authRepository.CreateUserAsync(user);
            
            // Log de auditoria
            await _authRepository.LogUserActivityAsync(createdUser.Id, "user_registered", "Usuário registrado no sistema");

            return createdUser;
        }

        public async Task<bool> LogoutAsync(string token)
        {
            return await _authRepository.RevokeSessionAsync(token);
        }

        public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            var session = await _authRepository.GetSessionByRefreshTokenAsync(refreshToken);
            if (session == null || session.IsRevoked || session.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token inválido ou expirado");
            }

            var user = await _authRepository.GetUserByIdAsync(session.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado");
            }

            // Revogar o refresh token antigo
            await _authRepository.RevokeSessionByRefreshTokenAsync(refreshToken);

            // Gerar novos tokens
            var newJwt = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var newRefreshTokenExpires = DateTime.UtcNow.AddDays(7);
            await _authRepository.CreateSessionAsync(user.Id, newRefreshToken, newRefreshTokenExpires);

            return new LoginResponse
            {
                Token = newJwt,
                User = user,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                RefreshToken = newRefreshToken
            };
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _authRepository.GetUserByIdAsync(id);
        }

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _authRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            // Validar se email está disponível (exceto para o próprio usuário)
            if (user.Email != existingUser.Email)
            {
                if (!await _authRepository.IsEmailAvailableAsync(user.Email))
                {
                    throw new ArgumentException("Email já está em uso");
                }
            }

            return await _authRepository.UpdateUserAsync(id, user);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Verificar senha atual
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                throw new ArgumentException("Senha atual incorreta");
            }

            // Validar nova senha
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new ArgumentException("Senhas não coincidem");
            }

            if (!IsValidPassword(request.NewPassword))
            {
                throw new ArgumentException("Nova senha não atende aos critérios de segurança");
            }

            // Atualizar senha
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            var result = await _authRepository.UpdatePasswordAsync(userId, newPasswordHash);

            if (result)
            {
                // Log de auditoria
                await _authRepository.LogUserActivityAsync(userId, "password_changed", "Senha alterada pelo usuário");
            }

            return result;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _authRepository.DeleteUserAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _authRepository.GetAllUsersAsync();
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                // Por segurança, retornamos true mesmo se o email não existir
                return true;
            }

            // Gerar token de reset
            var resetToken = GeneratePasswordResetToken();
            await _authRepository.CreatePasswordResetTokenAsync(request.Email, resetToken);

            // Enviar e-mail de reset
            var resetUrl = _configuration["App:ResetPasswordUrl"] ?? "https://seusite.com/reset-password";
            var link = $"{resetUrl}?email={Uri.EscapeDataString(user.Email)}&token={resetToken}";
            var subject = "Recuperação de senha - Dental360";
            var body = $@"<p>Olá, {user.FullName}!</p><p>Recebemos uma solicitação para redefinir sua senha.</p><p>Use o código abaixo ou clique no link para redefinir:</p><h2>{resetToken}</h2><p><a href='{link}'>Redefinir senha</a></p><p>Se não foi você, ignore este e-mail.</p>";
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            // Validar token
            if (!await _authRepository.ValidatePasswordResetTokenAsync(request.Email, request.Token))
            {
                throw new ArgumentException("Token inválido ou expirado");
            }

            // Validar senhas
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new ArgumentException("Senhas não coincidem");
            }

            if (!IsValidPassword(request.NewPassword))
            {
                throw new ArgumentException("Senha não atende aos critérios de segurança");
            }

            // Buscar usuário
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return false;
            }

            // Atualizar senha
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            var result = await _authRepository.UpdatePasswordAsync(user.Id, newPasswordHash);

            if (result)
            {
                // Consumir token
                await _authRepository.ConsumePasswordResetTokenAsync(request.Email, request.Token);
                
                // Log de auditoria
                await _authRepository.LogUserActivityAsync(user.Id, "password_reset", "Senha redefinida via reset");
            }

            return result;
        }

        public async Task<bool> ValidateResetTokenAsync(string token, string email)
        {
            return await _authRepository.ValidatePasswordResetTokenAsync(email, token);
        }

        public async Task<DashboardMetrics> GetDashboardMetricsAsync(int? userId = null)
        {
            return await _authRepository.GetDashboardMetricsAsync(userId);
        }

        public async Task<IEnumerable<RecentActivity>> GetRecentActivitiesAsync(int limit = 10)
        {
            return await _authRepository.GetRecentActivitiesAsync(limit);
        }

        public async Task<SystemInfo> GetSystemInfoAsync()
        {
            return await _authRepository.GetSystemInfoAsync();
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            return await _authRepository.GetUserProfileAsync(userId);
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UserProfile profile)
        {
            return await _authRepository.UpdateUserProfileAsync(userId, profile);
        }

        public async Task<UserStatistics> GetUserStatisticsAsync(int userId)
        {
            return await _authRepository.GetUserStatisticsAsync(userId);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            return await _authRepository.ValidateSessionAsync(token);
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            return await _authRepository.IsUsernameAvailableAsync(username);
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return await _authRepository.IsEmailAvailableAsync(email);
        }

        public async Task<bool> RevokeAllSessionsAsync(int userId)
        {
            return await _authRepository.RevokeAllUserSessionsAsync(userId);
        }

        public async Task<IEnumerable<object>> GetActiveSessionsAsync(int userId)
        {
            // Implementação simplificada
            return new List<object>();
        }

        public async Task<IEnumerable<object>> GetUserActivityLogAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _authRepository.GetUserActivityLogAsync(userId, startDate, endDate);
        }

        public async Task<bool> LogSecurityEventAsync(int userId, string eventType, string details)
        {
            return await _authRepository.LogUserActivityAsync(userId, eventType, details);
        }

        public async Task<IEnumerable<SystemAlert>> GetSecurityAlertsAsync()
        {
            return await _authRepository.GetSystemAlertsAsync("security");
        }

        // Métodos auxiliares privados
        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "your-secret-key-here-make-it-very-long-and-secure";
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("fullName", user.FullName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "ClinicApi",
                Audience = _configuration["Jwt:Audience"] ?? "ClinicApp"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private string GeneratePasswordResetToken()
        {
            return Guid.NewGuid().ToString("N")[..8].ToUpper();
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpper && hasLower && hasDigit;
        }
    }
}