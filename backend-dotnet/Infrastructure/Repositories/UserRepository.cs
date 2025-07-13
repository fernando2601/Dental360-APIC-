using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServerConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null.");
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("SELECT * FROM [User] WHERE [Email] = @Email", connection);
            command.Parameters.AddWithValue("@Email", email);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReaderToUser(reader) : null;
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("SELECT * FROM [User] WHERE [Username] = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReaderToUser(reader) : null;
        }

        public async Task<User?> FindByRefreshTokenAsync(string refreshToken)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("SELECT * FROM [User] WHERE [RefreshToken] = @RefreshToken", connection);
            command.Parameters.AddWithValue("@RefreshToken", refreshToken);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReaderToUser(reader) : null;
        }

        public async Task AddAsync(User user)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(
                "INSERT INTO [User] ([Username], [PasswordHash], [Email], [Role]) VALUES (@Username, @PasswordHash, @Email, @Role)",
                connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Role", user.Role);
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(User user)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(
                "UPDATE [User] SET [Username] = @Username, [PasswordHash] = @PasswordHash, [Email] = @Email, [Role] = @Role, " +
                "[IsActive] = @IsActive, [IsDeleted] = @IsDeleted, [UpdatedAt] = @UpdatedAt, [UpdatedByUserId] = @UpdatedByUserId " +
                "WHERE [Id] = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Role", user.Role);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@IsDeleted", user.IsDeleted);
            command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);
            command.Parameters.AddWithValue("@UpdatedByUserId", user.UpdatedByUserId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("SELECT * FROM [User] WHERE [Id] = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReaderToUser(reader) : null;
        }

        public async Task<User> CreateAsync(User user)
        {
            await AddAsync(user);
            return user;
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            user.Id = id;
            await UpdateAsync(user);
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("DELETE FROM [User] WHERE [Id] = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            var affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("SELECT * FROM [User]", connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(MapReaderToUser(reader));
            }
            return users;
        }

        public async Task<IEnumerable<User>> SearchAsync(string query)
        {
            var users = new List<User>();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("SELECT * FROM [User] WHERE [Username] LIKE @Query OR [Email] LIKE @Query", connection);
            command.Parameters.AddWithValue("@Query", $"%{query}%");
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(MapReaderToUser(reader));
            }
            return users;
        }

        public async Task<object> GetProfileByIdAsync(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("SELECT [Username], [Email] FROM [User] WHERE [Id] = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new
                {
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                };
            }
            // Retorna um objeto anônimo vazio se não encontrar, para evitar nulos
            return new { };
        }

        private User MapReaderToUser(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Role = reader.GetString(reader.GetOrdinal("Role")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("CreatedByUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                UpdatedByUserId = reader.IsDBNull(reader.GetOrdinal("UpdatedByUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UpdatedByUserId")),
            };
        }
    }
}