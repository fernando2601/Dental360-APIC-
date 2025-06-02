using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Application.DTOs;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="request">Dados de login (username e password)</param>
        /// <returns>Token JWT e dados do usuário</returns>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="request">Dados do usuário para registro</param>
        /// <returns>Dados do usuário criado</returns>
        [HttpPost("register")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Solicita recuperação de senha
        /// </summary>
        /// <param name="request">Email para recuperação</param>
        /// <returns>Confirmação de envio</returns>
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request);
                return Ok(new { message = "Se o email existir, um link de recuperação será enviado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Redefine senha usando token
        /// </summary>
        /// <param name="request">Token e nova senha</param>
        /// <returns>Confirmação de redefinição</returns>
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _authService.ResetPasswordAsync(request);
                return Ok(new { message = "Senha redefinida com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtém perfil do usuário autenticado
        /// </summary>
        /// <returns>Dados do usuário</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("nameid")?.Value ?? "0");
                var user = await _authService.GetProfileAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se o token JWT é válido
        /// </summary>
        /// <returns>Status de validação</returns>
        [HttpGet("verify")]
        [Authorize]
        public ActionResult VerifyToken()
        {
            return Ok(new { message = "Token válido", userId = User.FindFirst("nameid")?.Value });
        }
    }
}