using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<object?> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Username))
            {
                throw new ArgumentException("Email or Username is required.");
            }

            User? user;
            if (!string.IsNullOrEmpty(request.Email))
            {
                user = await _userRepository.FindByEmailAsync(request.Email);
            }
            else
            {
                user = await _userRepository.FindByUsernameAsync(request.Username!);
            }

            if (user == null || user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
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
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Name, user.FullName ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new { Token = tokenString, User = new { user.Id, user.Email, user.FullName } };
        }

        public async Task<object> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email;
            var password = request.Password;
            var fullName = request.FullName;
            var username = request.Username;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Email, username, password, and full name are required.");
            }

            var existingUser = await _userRepository.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email is already in use.");
            }
            
            existingUser = await _userRepository.FindByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username is already in use.");
            }

            var newUser = new User
            {
                Email = email,
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FullName = fullName,
                Role = "user",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            var createdUser = await _userRepository.CreateAsync(newUser);
            return createdUser;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException("Current and new passwords are required.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials or user not found.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var updatedUser = await _userRepository.UpdateAsync(user.Id, user);

            return updatedUser != null;
        }
        
        public Task<bool> ForgotPasswordAsync(object request)
        {
            // Placeholder implementation
            return Task.FromResult(true);
        }

        public Task<bool> ResetPasswordAsync(object request)
        {
            // Placeholder implementation
            return Task.FromResult(true);
        }

        public async Task<object> GetProfileAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId) ?? new object();
        }

        public Task<object> RefreshTokenAsync(object request)
        {
            // Placeholder implementation
            return Task.FromResult<object>(new { Token = "new-refreshed-fake-token" });
        }
    }
} 