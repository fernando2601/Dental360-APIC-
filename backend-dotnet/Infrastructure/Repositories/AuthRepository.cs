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
                cmd.CommandText = "SELECT Id, Username, FullName, Email, Role, CreatedAt FROM [User] WHERE Email = @Email AND IsActive = 1";
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
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            PermissionId = reader.GetInt32(reader.GetOrdinal("Role")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
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
                cmd.CommandText = "SELECT Id, Username, FullName, Email, Role, CreatedAt FROM [User] WHERE Id = @Id AND IsActive = 1";
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
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            PermissionId = reader.GetInt32(reader.GetOrdinal("Role")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
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
                cmd.CommandText = "INSERT INTO [User] (Username, FullName, Email, PasswordHash, Role, IsActive, CreatedAt) VALUES (@Username, @FullName, @Email, @PasswordHash, @Role, 1, @CreatedAt); SELECT SCOPE_IDENTITY();";
                
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
                passwordParam.ParameterName = "@PasswordHash";
                passwordParam.Value = passwordHash;
                cmd.Parameters.Add(passwordParam);

                var roleParam = cmd.CreateParameter();
                roleParam.ParameterName = "@Role";
                roleParam.Value = user.PermissionId;
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
                cmd.CommandText = "UPDATE [User] SET Username = @Username, FullName = @FullName, Email = @Email, Role = @Role WHERE Id = @Id AND IsActive = 1";
                
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
                roleParam.Value = user.PermissionId;
                cmd.Parameters.Add(roleParam);

                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? user : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE [User] SET IsActive = 0 WHERE Id = @Id";
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
                cmd.CommandText = "SELECT Id, Username, FullName, Email, PasswordHash, Role, CreatedAt FROM [User] WHERE Email = @Email AND IsActive = 1";
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
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            PermissionId = reader.GetInt32(reader.GetOrdinal("Role")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
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
                cmd.CommandText = "UPDATE [User] SET PasswordHash = @NewPassword WHERE Id = @Id";
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
                cmd.CommandText = "UPDATE [User] SET PasswordHash = @NewPassword WHERE Email = @Email";
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
                    cmd.CommandText = "SELECT 1 FROM [User] WHERE Email = @Email AND Id != @ExcludeId AND IsActive = 1";
                    var excludeParam = cmd.CreateParameter();
                    excludeParam.ParameterName = "@ExcludeId";
                    excludeParam.Value = excludeId.Value;
                    cmd.Parameters.Add(excludeParam);
                }
                else
                {
                    cmd.CommandText = "SELECT 1 FROM [User] WHERE Email = @Email AND IsActive = 1";
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
                    cmd.CommandText = "SELECT 1 FROM [User] WHERE Username = @Username AND Id != @ExcludeId AND IsActive = 1";
                    var excludeParam = cmd.CreateParameter();
                    excludeParam.ParameterName = "@ExcludeId";
                    excludeParam.Value = excludeId.Value;
                    cmd.Parameters.Add(excludeParam);
                }
                else
                {
                    cmd.CommandText = "SELECT 1 FROM [User] WHERE Username = @Username AND IsActive = 1";
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