using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.API.Models;
using DentalSpa.API.Services;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _userService.AuthenticateAsync(request.Username, request.Password);
                if (user == null)
                {
                    return Unauthorized(new { message = "Credenciais inválidas" });
                }

                var token = await _userService.GenerateJwtTokenAsync(user);
                
                return Ok(new LoginResponse
                {
                    Token = token,
                    User = new User
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FullName = user.FullName,
                        Email = user.Email,
                        Role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao processar login", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Verificar se usuário já existe
                var existingUsername = await _userService.GetUserByUsernameAsync(request.Username);
                if (existingUsername != null)
                {
                    return BadRequest(new { message = "Nome de usuário já está em uso" });
                }

                // Verificar se email já existe
                var existingEmail = await _userService.QuerySingleOrDefaultAsync<User>(
                    "SELECT * FROM users WHERE email = @Email", 
                    new { Email = request.Email });
                if (existingEmail != null)
                {
                    return BadRequest(new { message = "Email já está em uso" });
                }

                // Criar usuário
                var user = await _userService.CreateUserAsync(new UserCreateRequest
                {
                    Username = request.Username,
                    Password = request.Password,
                    FullName = request.FullName,
                    Role = request.Role,
                    Email = request.Email,
                    Phone = request.Phone
                });

                var token = await _userService.GenerateJwtTokenAsync(user);

                return StatusCode(201, new LoginResponse
                {
                    Token = token,
                    User = new User
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FullName = user.FullName,
                        Email = user.Email,
                        Role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao registrar usuário", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Token inválido" });
                }

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                return Ok(new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar usuário", error = ex.Message });
            }
        }
    }
}