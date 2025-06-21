using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using Npgsql;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace DentalSpa.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE is_active = true";
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(MapReaderToUser(reader));
                    }
                }
            }
            return await Task.FromResult(users);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE id = @Id AND is_active = true";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToUser(reader);
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE email = @Email AND is_active = true";
                cmd.Parameters.Add(CreateParameter("@Email", email));
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToUser(reader);
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users WHERE username = @Username AND is_active = true";
                cmd.Parameters.Add(CreateParameter("@Username", username));
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToUser(reader);
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }

        public async Task<User> CreateAsync(User user)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO users (username, email, password_hash, role, is_active, created_at, updated_at, full_name) 
                                   VALUES (@Username, @Email, @PasswordHash, @Role, @IsActive, @CreatedAt, @UpdatedAt, @FullName)
                                   RETURNING id;";
                
                cmd.Parameters.Add(CreateParameter("@Username", user.Username));
                cmd.Parameters.Add(CreateParameter("@Email", user.Email));
                cmd.Parameters.Add(CreateParameter("@PasswordHash", user.PasswordHash));
                cmd.Parameters.Add(CreateParameter("@Role", user.Role ?? "user"));
                cmd.Parameters.Add(CreateParameter("@IsActive", true));
                cmd.Parameters.Add(CreateParameter("@CreatedAt", DateTime.UtcNow));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.UtcNow));
                cmd.Parameters.Add(CreateParameter("@FullName", user.FullName));
                
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                user.Id = id;
                return user;
            }
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE users SET 
                                    username = @Username, 
                                    email = @Email, 
                                    password_hash = @PasswordHash, 
                                    role = @Role, 
                                    is_active = @IsActive, 
                                    updated_at = @UpdatedAt,
                                    full_name = @FullName
                                   WHERE id = @Id AND is_active = true";
                
                cmd.Parameters.Add(CreateParameter("@Id", id));
                cmd.Parameters.Add(CreateParameter("@Username", user.Username));
                cmd.Parameters.Add(CreateParameter("@Email", user.Email));
                cmd.Parameters.Add(CreateParameter("@PasswordHash", user.PasswordHash));
                cmd.Parameters.Add(CreateParameter("@Role", user.Role));
                cmd.Parameters.Add(CreateParameter("@IsActive", user.IsActive));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.UtcNow));
                cmd.Parameters.Add(CreateParameter("@FullName", user.FullName));
                
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                var rows = cmd.ExecuteNonQuery();
                return rows > 0 ? user : null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET is_active = false WHERE id = @Id";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public async Task<bool> ExistsAsync(string username, string email)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM users WHERE (username = @Username OR email = @Email) AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Username", username));
                cmd.Parameters.Add(CreateParameter("@Email", email));
                if (_connection.State == ConnectionState.Closed) _connection.Open();
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count > 0);
            }
        }

        public async Task<IEnumerable<User>> SearchAsync(string query)
        {
            return await Task.FromResult(Enumerable.Empty<User>());
        }

        private IDbDataParameter CreateParameter(string name, object? value)
        {
            var param = _connection.CreateCommand().CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }

        private User MapReaderToUser(IDataReader reader)
        {
            return new User
            {
                Id = Convert.ToInt32(reader["id"]),
                Username = reader["username"]?.ToString(),
                PasswordHash = reader["password_hash"]?.ToString(),
                FullName = reader["full_name"]?.ToString(),
                Email = reader["email"]?.ToString(),
                Role = reader["role"]?.ToString(),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                IsActive = Convert.ToBoolean(reader["is_active"])
            };
        }
    }
} 