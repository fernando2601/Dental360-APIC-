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
using System.Collections.Generic;

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

            // Buscar permissão do usuário
            var permission = user.Role ?? "ADM";

            return GenerateJwtToken(user, permission);
        }

        private object GenerateJwtToken(User user, string permission)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "DentalSpa-Default-Secret-Key-For-JWT-Token-Generation-2024");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim("permission", permission)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return new { Token = tokenString, User = new { user.Email, user.Username, Permission = permission } };
        }

        public async Task<object> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email;
            var password = request.Password;
            var username = request.Username;
            var role = string.IsNullOrEmpty(request.Role) ? "User" : request.Role;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Email, username, and password are required.");
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
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            var createdUser = await _userRepository.CreateAsync(newUser);
            return createdUser;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false; // Ou lançar exceção
            }

            // Verificar a senha antiga
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                return false;
            }

            // Criptografar e salvar a nova senha
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            return true;
        }
        
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            // Função desabilitada: não há campos de token de reset na tabela User
            return false;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            // Função desabilitada: não há campos de token de reset na tabela User
            return false;
        }

        public async Task<object> GetProfileAsync(int userId)
        {
            return await _userRepository.GetProfileByIdAsync(userId) ?? new object();
        }

        public async Task<object?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            // Função desabilitada: não há campos de refresh token na tabela User
            return null;
        }
    }
} 