using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DentalSpa.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IUserRepository userRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<object> LoginAsync(object request)
        {
            var jsonElement = (JsonElement)request;
            var email = jsonElement.GetProperty("email").GetString();
            var password = jsonElement.GetProperty("password").GetString();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Email and password are required.");
            }

            var user = await _userRepository.FindByEmailAsync(email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return GenerateJwtToken(user);
        }

        private object GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "DentalSpa-Default-Secret-Key-For-JWT-Token-Generation-2024");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new { Token = tokenString, User = new { user.Id, user.Email, user.FullName } };
        }

        public async Task<object> RegisterAsync(object request)
        {
            var jsonElement = (JsonElement)request;
            var email = jsonElement.GetProperty("email").GetString();
            var password = jsonElement.GetProperty("password").GetString();
            var fullName = jsonElement.GetProperty("fullName").GetString();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("Email, password, and full name are required.");
            }

            var existingUser = await _userRepository.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email is already in use.");
            }

            var newUser = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FullName = fullName
            };
            
            var createdUser = await _userRepository.CreateAsync(newUser);
            return createdUser;
        }

        public async Task<bool> ChangePasswordAsync(object request)
        {
            var jsonElement = (JsonElement)request;
            var userId = jsonElement.GetProperty("userId").GetInt32();
            var currentPassword = jsonElement.GetProperty("currentPassword").GetString();
            var newPassword = jsonElement.GetProperty("newPassword").GetString();

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException("Current and new passwords are required.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials or user not found.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var updatedUser = await _userRepository.UpdateAsync(user.Id, user);

            return updatedUser != null;
        }

        public async Task<bool> ForgotPasswordAsync(object request) { await Task.CompletedTask; return true; }

        public async Task<bool> ResetPasswordAsync(object request)
        {
            var jsonElement = (JsonElement)request;
            var email = jsonElement.GetProperty("email").GetString();
            var newPassword = jsonElement.GetProperty("newPassword").GetString();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException("Email and new password are required.");
            }

            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
            {
                // Return true to not reveal if an email exists or not
                return true; 
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user.Id, user);

            return true;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            // Implementar validação de token
            return true;
        }

        public async Task<object> RefreshTokenAsync(object request) => new { Token = "new-fake-token" };

        public async Task<bool> LogoutAsync(int userId)
        {
            // Implementar logout
            return true;
        }

        public async Task<object> GetDashboardMetricsAsync(int userId)
        {
            return new { totalUsers = 100, activeUsers = 50 };
        }

        public async Task<object> GetRecentActivityAsync(int userId)
        {
            return new { activities = new List<object>() };
        }

        public async Task<object> GetSystemInfoAsync()
        {
            return new { version = "1.0.0", status = "online" };
        }

        public async Task<object> GetUserProfileAsync(int userId)
        {
            var user = await _authRepository.GetByIdAsync(userId);
            return user ?? new object();
        }

        public async Task<object> GetUserStatisticsAsync(int userId)
        {
            return new { loginCount = 10, lastLogin = DateTime.Now };
        }

        public async Task<object> GetSecurityAlertsAsync()
        {
            return new { alerts = new List<object>() };
        }

        public async Task<object> GetProfileAsync(int userId) => new { UserId = userId, Name = "Fake User" };
    }
} 