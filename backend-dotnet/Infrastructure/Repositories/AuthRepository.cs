using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

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
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, username, full_name, email, role, created_at FROM users WHERE email = @Email AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Email";
                param.Value = email;
                cmd.Parameters.Add(param);
                
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Role = reader.GetString(reader.GetOrdinal("role")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, username, full_name, email, role, created_at FROM users WHERE id = @Id AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Role = reader.GetString(reader.GetOrdinal("role")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }

        public async Task<User> CreateAsync(User user, string passwordHash)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO users (username, full_name, email, password, role, is_active, created_at) VALUES (@Username, @FullName, @Email, @Password, @Role, 1, @CreatedAt); SELECT LASTVAL();";
                
                var usernameParam = cmd.CreateParameter();
                usernameParam.ParameterName = "@Username";
                usernameParam.Value = user.Username;
                cmd.Parameters.Add(usernameParam);

                var fullNameParam = cmd.CreateParameter();
                fullNameParam.ParameterName = "@FullName";
                fullNameParam.Value = user.FullName;
                cmd.Parameters.Add(fullNameParam);

                var emailParam = cmd.CreateParameter();
                emailParam.ParameterName = "@Email";
                emailParam.Value = user.Email;
                cmd.Parameters.Add(emailParam);

                var passwordParam = cmd.CreateParameter();
                passwordParam.ParameterName = "@Password";
                passwordParam.Value = passwordHash;
                cmd.Parameters.Add(passwordParam);

                var roleParam = cmd.CreateParameter();
                roleParam.ParameterName = "@Role";
                roleParam.Value = user.Role;
                cmd.Parameters.Add(roleParam);

                var createdParam = cmd.CreateParameter();
                createdParam.ParameterName = "@CreatedAt";
                createdParam.Value = DateTime.UtcNow;
                cmd.Parameters.Add(createdParam);

                var id = Convert.ToInt32(cmd.ExecuteScalar());
                user.Id = id;
                user.CreatedAt = DateTime.UtcNow;
                return await Task.FromResult(user);
            }
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET username = @Username, full_name = @FullName, email = @Email, role = @Role WHERE id = @Id AND is_active = 1";
                
                var idParam = cmd.CreateParameter();
                idParam.ParameterName = "@Id";
                idParam.Value = id;
                cmd.Parameters.Add(idParam);

                var usernameParam = cmd.CreateParameter();
                usernameParam.ParameterName = "@Username";
                usernameParam.Value = user.Username;
                cmd.Parameters.Add(usernameParam);

                var fullNameParam = cmd.CreateParameter();
                fullNameParam.ParameterName = "@FullName";
                fullNameParam.Value = user.FullName;
                cmd.Parameters.Add(fullNameParam);

                var emailParam = cmd.CreateParameter();
                emailParam.ParameterName = "@Email";
                emailParam.Value = user.Email;
                cmd.Parameters.Add(emailParam);

                var roleParam = cmd.CreateParameter();
                roleParam.ParameterName = "@Role";
                roleParam.Value = user.Role;
                cmd.Parameters.Add(roleParam);

                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? user : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET is_active = 0 WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, username, full_name, email, password, role, created_at FROM users WHERE email = @Email AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Email";
                param.Value = email;
                cmd.Parameters.Add(param);
                
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Password = reader.GetString(reader.GetOrdinal("password")),
                            Role = reader.GetString(reader.GetOrdinal("role")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET password = @NewPassword WHERE id = @Id";
                var idParam = cmd.CreateParameter();
                idParam.ParameterName = "@Id";
                idParam.Value = userId;
                cmd.Parameters.Add(idParam);

                var passwordParam = cmd.CreateParameter();
                passwordParam.ParameterName = "@NewPassword";
                passwordParam.Value = newPassword;
                cmd.Parameters.Add(passwordParam);

                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET password = @NewPassword WHERE email = @Email";
                var emailParam = cmd.CreateParameter();
                emailParam.ParameterName = "@Email";
                emailParam.Value = email;
                cmd.Parameters.Add(emailParam);

                var passwordParam = cmd.CreateParameter();
                passwordParam.ParameterName = "@NewPassword";
                passwordParam.Value = newPassword;
                cmd.Parameters.Add(passwordParam);

                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            using (var cmd = _connection.CreateCommand())
            {
                if (excludeId.HasValue)
                {
                    cmd.CommandText = "SELECT 1 FROM users WHERE email = @Email AND id != @ExcludeId AND is_active = 1";
                    var excludeParam = cmd.CreateParameter();
                    excludeParam.ParameterName = "@ExcludeId";
                    excludeParam.Value = excludeId.Value;
                    cmd.Parameters.Add(excludeParam);
                }
                else
                {
                    cmd.CommandText = "SELECT 1 FROM users WHERE email = @Email AND is_active = 1";
                }

                var emailParam = cmd.CreateParameter();
                emailParam.ParameterName = "@Email";
                emailParam.Value = email;
                cmd.Parameters.Add(emailParam);

                using (var reader = cmd.ExecuteReader())
                {
                    return await Task.FromResult(reader.Read());
                }
            }
        }

        public async Task<bool> UsernameExistsAsync(string username, int? excludeId = null)
        {
            using (var cmd = _connection.CreateCommand())
            {
                if (excludeId.HasValue)
                {
                    cmd.CommandText = "SELECT 1 FROM users WHERE username = @Username AND id != @ExcludeId AND is_active = 1";
                    var excludeParam = cmd.CreateParameter();
                    excludeParam.ParameterName = "@ExcludeId";
                    excludeParam.Value = excludeId.Value;
                    cmd.Parameters.Add(excludeParam);
                }
                else
                {
                    cmd.CommandText = "SELECT 1 FROM users WHERE username = @Username AND is_active = 1";
                }

                var usernameParam = cmd.CreateParameter();
                usernameParam.ParameterName = "@Username";
                usernameParam.Value = username;
                cmd.Parameters.Add(usernameParam);

                using (var reader = cmd.ExecuteReader())
                {
                    return await Task.FromResult(reader.Read());
                }
            }
        }
    }
} 