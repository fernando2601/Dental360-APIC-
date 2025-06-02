using DentalSpa.API.Models;
using Dapper;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DentalSpa.API.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User> CreateUserAsync(UserCreateRequest request);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<string> GenerateJwtTokenAsync(User user);
        Task SeedDefaultUsersAsync();
    }

    public class UserService : BaseService, IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration) : base()
        {
            _configuration = configuration;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var sql = "SELECT * FROM users WHERE username = @Username";
            var user = await QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
            
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                // Update last login
                await ExecuteAsync("UPDATE users SET lastlogin = @LastLogin WHERE id = @Id", 
                    new { LastLogin = DateTime.UtcNow, Id = user.Id });
                return user;
            }
            
            return null;
        }

        public async Task<User> CreateUserAsync(UserCreateRequest request)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            
            var sql = @"
                INSERT INTO users (username, password, fullname, role, email, phone, createdat)
                VALUES (@Username, @Password, @FullName, @Role, @Email, @Phone, @CreatedAt)
                RETURNING *";
            
            var user = await QuerySingleOrDefaultAsync<User>(sql, new
            {
                Username = request.Username,
                Password = hashedPassword,
                FullName = request.FullName,
                Role = request.Role,
                Email = request.Email,
                Phone = request.Phone,
                CreatedAt = DateTime.UtcNow
            });

            return user!;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var sql = "SELECT * FROM users WHERE id = @Id";
            return await QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var sql = "SELECT * FROM users WHERE username = @Username";
            return await QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "your-secret-key-here-make-it-very-long-and-secure";
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim("username", user.Username),
                    new Claim("role", user.Role),
                    new Claim("email", user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task SeedDefaultUsersAsync()
        {
            try
            {
                // Check if admin user exists
                var adminExists = await GetUserByUsernameAsync("admin");
                if (adminExists == null)
                {
                    await CreateUserAsync(new UserCreateRequest
                    {
                        Username = "admin",
                        Password = "admin123",
                        FullName = "Administrador",
                        Role = "admin",
                        Email = "admin@dentalspa.com"
                    });
                    Console.WriteLine("✓ Usuário admin criado");
                }

                // Check if user nerifernando2@gmail.com exists
                var userExists = await QuerySingleOrDefaultAsync<User>(
                    "SELECT * FROM users WHERE email = @Email", 
                    new { Email = "nerifernando2@gmail.com" });
                
                if (userExists == null)
                {
                    await CreateUserAsync(new UserCreateRequest
                    {
                        Username = "nerifernando2@gmail.com",
                        Password = "@Brazucas",
                        FullName = "Neri Fernando",
                        Role = "admin",
                        Email = "nerifernando2@gmail.com"
                    });
                    Console.WriteLine("✓ Usuário nerifernando2@gmail.com criado");
                }

                // Check if funcionario user exists
                var staffExists = await GetUserByUsernameAsync("funcionario");
                if (staffExists == null)
                {
                    await CreateUserAsync(new UserCreateRequest
                    {
                        Username = "funcionario",
                        Password = "@Brazucas",
                        FullName = "Funcionário",
                        Role = "staff",
                        Email = "funcionario@dentalspa.com"
                    });
                    Console.WriteLine("✓ Usuário funcionario criado");
                }

                Console.WriteLine("USUÁRIOS CRIADOS COM SUCESSO:");
                Console.WriteLine("- admin/admin123");
                Console.WriteLine("- nerifernando2@gmail.com/@Brazucas");
                Console.WriteLine("- funcionario/@Brazucas");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar usuários padrão: {ex.Message}");
            }
        }
    }
}