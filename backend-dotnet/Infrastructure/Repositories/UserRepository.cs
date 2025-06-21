using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using Npgsql;
using System.Data;
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
                cmd.CommandText = "SELECT id, username, email, password_hash, role, is_active, created_at, updated_at FROM users WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                            Role = reader.GetString(reader.GetOrdinal("role")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(users);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, username, email, password_hash, role, is_active, created_at, updated_at FROM users WHERE id = @Id AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                            Role = reader.GetString(reader.GetOrdinal("role")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, username, email, password_hash, role, is_active, created_at, updated_at FROM users WHERE username = @Username AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Username", username));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                            Role = reader.GetString(reader.GetOrdinal("role")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, username, email, password_hash, role, is_active, created_at, updated_at FROM users WHERE email = @Email AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Email", email));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                            Role = reader.GetString(reader.GetOrdinal("role")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }

        public async Task<User> CreateAsync(User user)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO users (username, email, password_hash, role, is_active, created_at, updated_at) 
                                   VALUES (@Username, @Email, @PasswordHash, @Role, @IsActive, @CreatedAt, @UpdatedAt);
                                   SELECT CAST(SCOPE_IDENTITY() as int)";
                
                cmd.Parameters.Add(CreateParameter("@Username", user.Username));
                cmd.Parameters.Add(CreateParameter("@Email", user.Email));
                cmd.Parameters.Add(CreateParameter("@PasswordHash", user.PasswordHash));
                cmd.Parameters.Add(CreateParameter("@Role", user.Role));
                cmd.Parameters.Add(CreateParameter("@IsActive", true));
                cmd.Parameters.Add(CreateParameter("@CreatedAt", DateTime.Now));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                user.Id = id;
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                return await Task.FromResult(user);
            }
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE users SET username = @Username, email = @Email, password_hash = @PasswordHash, role = @Role, is_active = @IsActive, updated_at = @UpdatedAt WHERE id = @Id AND is_active = 1";
                
                cmd.Parameters.Add(CreateParameter("@Id", id));
                cmd.Parameters.Add(CreateParameter("@Username", user.Username));
                cmd.Parameters.Add(CreateParameter("@Email", user.Email));
                cmd.Parameters.Add(CreateParameter("@PasswordHash", user.PasswordHash));
                cmd.Parameters.Add(CreateParameter("@Role", user.Role));
                cmd.Parameters.Add(CreateParameter("@IsActive", user.IsActive));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? user : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET is_active = 0 WHERE id = @Id";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<bool> ExistsAsync(string username, string email)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM users WHERE (username = @Username OR email = @Email) AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Username", username));
                cmd.Parameters.Add(CreateParameter("@Email", email));
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count > 0);
            }
        }

        public async Task<IEnumerable<User>> SearchAsync(string term)
        {
            return Enumerable.Empty<User>();
        }

        private IDbDataParameter CreateParameter(string name, object? value)
        {
            var param = _connection.CreateCommand().CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, username, email, password_hash, role, is_active, created_at, updated_at FROM users WHERE email = @Email AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Email", email));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                            Role = reader.GetString(reader.GetOrdinal("role")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<User?>(null);
        }
    }
}