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
            var permission = user.Permission?.Name ?? "ADM";

            return GenerateJwtToken(user, permission);
        }

        private object GenerateJwtToken(User user, string permission)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "DentalSpa-Default-Secret-Key-For-JWT-Token-Generation-2024");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.FullName ?? ""),
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
            return new { Token = tokenString, User = new { user.Email, user.FullName, Permission = permission } };
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

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false; // Ou lançar exceção
            }

            // Verificar a senha antiga
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
            {
                return false;
            }

            // Criptografar e salvar a nova senha
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            return true;
        }
        
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Não revele que o usuário não existe.
                return true;
            }

            // Gerar token
            var token = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
            user.PasswordResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1); // Token válido por 1 hora

            await _userRepository.UpdateAsync(user);

            // TODO: Enviar o token por e-mail para o usuário.
            // Por enquanto, vamos pular o envio de e-mail real.
            // _emailService.SendPasswordResetEmail(user.Email, token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userRepository.FindByEmailAsync(request.Email);

            if (user == null || 
                user.PasswordResetToken != request.Token || 
                user.ResetTokenExpires < DateTime.UtcNow)
            {
                return false; // Token inválido, expirado ou usuário não encontrado
            }

            // Token válido, redefinir a senha
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordResetToken = null; // Limpar o token
            user.ResetTokenExpires = null;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<object> GetProfileAsync(int userId)
        {
            return await _userRepository.GetProfileByIdAsync(userId) ?? new object();
        }

        public async Task<object?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            // A lógica de validação do token (se é JWT, etc.) foi omitida por simplicidade.
            // Aqui, estamos apenas procurando o usuário pelo refresh token.
            var user = await _userRepository.FindByRefreshTokenAsync(request.Token);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null; // Token inválido ou expirado
            }

            // Gerar um novo token de acesso
            var newAccessToken = GenerateJwtToken(user);
            // Opcional: Gerar um novo refresh token e atualizar o usuário
            // var newRefreshToken = ...
            // user.RefreshToken = newRefreshToken;
            // user.RefreshTokenExpiryTime = ...
            // await _userRepository.UpdateAsync(user);

            return new { token = newAccessToken };
        }
    }
} 