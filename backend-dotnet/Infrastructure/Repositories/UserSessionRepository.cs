using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly string _connectionString;
        public UserSessionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<UserSession>> GetAllAsync()
        {
            var list = new List<UserSession>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT SessionId, UserId, Token, CreatedAt, ExpiresAt, IsActive, CreatedByUserId, UpdatedByUserId, UpdatedAt FROM UserSession", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new UserSession
                {
                    SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Token = reader.GetString(reader.GetOrdinal("Token")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("CreatedByUserId")) ? null : reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                    UpdatedByUserId = reader.IsDBNull(reader.GetOrdinal("UpdatedByUserId")) ? null : reader.GetInt32(reader.GetOrdinal("UpdatedByUserId")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                });
            }
            return list;
        }
        public async Task<UserSession?> GetByIdAsync(int sessionId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT SessionId, UserId, Token, CreatedAt, ExpiresAt, IsActive, CreatedByUserId, UpdatedByUserId, UpdatedAt FROM UserSession WHERE SessionId = @SessionId", connection);
            command.Parameters.AddWithValue("@SessionId", sessionId);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserSession
                {
                    SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Token = reader.GetString(reader.GetOrdinal("Token")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("CreatedByUserId")) ? null : reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                    UpdatedByUserId = reader.IsDBNull(reader.GetOrdinal("UpdatedByUserId")) ? null : reader.GetInt32(reader.GetOrdinal("UpdatedByUserId")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                };
            }
            return null;
        }
        public async Task<UserSession> CreateAsync(UserSession session)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("INSERT INTO UserSession (UserId, Token, CreatedAt, ExpiresAt, IsActive, CreatedByUserId, UpdatedByUserId, UpdatedAt) VALUES (@UserId, @Token, @CreatedAt, @ExpiresAt, @IsActive, @CreatedByUserId, @UpdatedByUserId, @UpdatedAt); SELECT SCOPE_IDENTITY();", connection);
            command.Parameters.AddWithValue("@UserId", session.UserId);
            command.Parameters.AddWithValue("@Token", session.Token);
            command.Parameters.AddWithValue("@CreatedAt", session.CreatedAt);
            command.Parameters.AddWithValue("@ExpiresAt", session.ExpiresAt);
            command.Parameters.AddWithValue("@IsActive", session.IsActive);
            command.Parameters.AddWithValue("@CreatedByUserId", (object?)session.CreatedByUserId ?? System.DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedByUserId", (object?)session.UpdatedByUserId ?? System.DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)session.UpdatedAt ?? System.DBNull.Value);
            var id = await command.ExecuteScalarAsync();
            session.SessionId = Convert.ToInt32(id);
            return session;
        }
        public async Task<UserSession?> UpdateAsync(int sessionId, UserSession session)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("UPDATE UserSession SET Token = @Token, ExpiresAt = @ExpiresAt, IsActive = @IsActive, UpdatedByUserId = @UpdatedByUserId, UpdatedAt = @UpdatedAt WHERE SessionId = @SessionId", connection);
            command.Parameters.AddWithValue("@SessionId", sessionId);
            command.Parameters.AddWithValue("@Token", session.Token);
            command.Parameters.AddWithValue("@ExpiresAt", session.ExpiresAt);
            command.Parameters.AddWithValue("@IsActive", session.IsActive);
            command.Parameters.AddWithValue("@UpdatedByUserId", (object?)session.UpdatedByUserId ?? System.DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)session.UpdatedAt ?? System.DBNull.Value);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0 ? session : null;
        }
        public async Task<bool> DeleteAsync(int sessionId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM UserSession WHERE SessionId = @SessionId", connection);
            command.Parameters.AddWithValue("@SessionId", sessionId);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
} 