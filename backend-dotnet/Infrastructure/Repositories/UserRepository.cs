using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
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
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null.");
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM public.\"Users\" WHERE \"Email\" = @Email", connection);
            command.Parameters.AddWithValue("@Email", email);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReaderToUser(reader) : null;
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM public.\"Users\" WHERE \"Username\" = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReaderToUser(reader) : null;
        }

        public async Task<User?> FindByRefreshTokenAsync(string refreshToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM public.\"Users\" WHERE \"RefreshToken\" = @RefreshToken", connection);
            command.Parameters.AddWithValue("@RefreshToken", refreshToken);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReaderToUser(reader) : null;
        }

        public async Task AddAsync(User user)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(
                "INSERT INTO public.\"Users\" (\"FullName\", \"Username\", \"Password\", \"Email\", \"PermissionId\") VALUES (@FullName, @Username, @Password, @Email, @PermissionId)",
                connection);
            command.Parameters.AddWithValue("@FullName", user.FullName);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PermissionId", user.PermissionId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(User user)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = new NpgsqlCommand(
                "UPDATE public.\"Users\" SET \"FullName\" = @FullName, \"Username\" = @Username, \"Password\" = @Password, \"Email\" = @Email, \"PermissionId\" = @PermissionId, " +
                "\"PasswordResetToken\" = @PasswordResetToken, \"ResetTokenExpires\" = @ResetTokenExpires, " +
                "\"RefreshToken\" = @RefreshToken, \"RefreshTokenExpiryTime\" = @RefreshTokenExpiryTime " +
                "WHERE \"Id\" = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@FullName", user.FullName);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PermissionId", user.PermissionId);
            command.Parameters.AddWithValue("@PasswordResetToken", (object)user.PasswordResetToken ?? DBNull.Value);
            command.Parameters.AddWithValue("@ResetTokenExpires", (object)user.ResetTokenExpires ?? DBNull.Value);
            command.Parameters.AddWithValue("@RefreshToken", (object)user.RefreshToken ?? DBNull.Value);
            command.Parameters.AddWithValue("@RefreshTokenExpiryTime", (object)user.RefreshTokenExpiryTime ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM public.\"Users\" WHERE \"Id\" = @Id", connection);
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
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand("DELETE FROM public.\"Users\" WHERE \"Id\" = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            var affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>();
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM public.\"Users\"", connection);
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
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand("SELECT * FROM public.\"Users\" WHERE \"FullName\" ILIKE @Query OR \"Email\" ILIKE @Query OR \"Username\" ILIKE @Query", connection);
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
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand("SELECT \"FullName\", \"Email\", \"Username\" FROM public.\"Users\" WHERE \"Id\" = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new
                {
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Username = reader.GetString(reader.GetOrdinal("Username"))
                };
            }
            // Retorna um objeto anônimo vazio se não encontrar, para evitar nulos
            return new { };
        }

        private User MapReaderToUser(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Password = reader.GetString(reader.GetOrdinal("Password")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PermissionId = reader.GetInt32(reader.GetOrdinal("PermissionId")),
                PasswordResetToken = reader.IsDBNull(reader.GetOrdinal("PasswordResetToken")) ? null : reader.GetString(reader.GetOrdinal("PasswordResetToken")),
                ResetTokenExpires = reader.IsDBNull(reader.GetOrdinal("ResetTokenExpires")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("ResetTokenExpires")),
                RefreshToken = reader.IsDBNull(reader.GetOrdinal("RefreshToken")) ? null : reader.GetString(reader.GetOrdinal("RefreshToken")),
                RefreshTokenExpiryTime = reader.IsDBNull(reader.GetOrdinal("RefreshTokenExpiryTime")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("RefreshTokenExpiryTime")),
            };
        }
    }
}