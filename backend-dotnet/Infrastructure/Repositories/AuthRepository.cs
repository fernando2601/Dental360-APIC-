using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DentalSpa.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _connection;

        public AuthRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            const string sql = "SELECT * FROM users WHERE email = @Email AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirstOrDefault<User>(sql, new { Email = email }));
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM users WHERE id = @Id AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirstOrDefault<User>(sql, new { Id = id }));
        }

        public async Task<User> CreateAsync(User user, string passwordHash)
        {
            const string sql = @"
                INSERT INTO users (name, email, password_hash, role, is_active, created_at, updated_at)
                VALUES (@Name, @Email, @PasswordHash, @Role, 1, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            
            var id = await Task.FromResult(_connection.QuerySingle<int>(sql, new
            {
                user.Name,
                user.Email,
                PasswordHash = passwordHash,
                user.Role,
                user.CreatedAt,
                user.UpdatedAt
            }));
            
            user.Id = id;
            return user;
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            const string sql = @"
                UPDATE users 
                SET name = @Name, email = @Email, role = @Role, updated_at = @UpdatedAt
                WHERE id = @Id AND is_active = 1";
            
            user.Id = id;
            user.UpdatedAt = DateTime.UtcNow;
            
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, user));
            return rowsAffected > 0 ? user : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE users SET is_active = 0 WHERE id = @Id";
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, new { Id = id }));
            return rowsAffected > 0;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            const string sql = "SELECT * FROM users WHERE email = @Email AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirstOrDefault<User>(sql, new { Email = email }));
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            const string sql = "UPDATE users SET password_hash = @NewPasswordHash WHERE id = @Id";
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, new { Id = userId, NewPasswordHash = newPassword }));
            return rowsAffected > 0;
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            const string sql = "UPDATE users SET password_hash = @NewPasswordHash WHERE email = @Email";
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, new { Email = email, NewPasswordHash = newPassword }));
            return rowsAffected > 0;
        }
    }
}