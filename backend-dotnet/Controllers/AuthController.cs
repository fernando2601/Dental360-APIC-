using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Application.Interfaces;
using System.Threading.Tasks;
using DentalSpa.Application.DTOs;
using System.Security.Claims;

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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            return Ok(result);
        }

        [HttpPost("register")]
        [Authorize(Roles = "admin")] 
        public async Task<ActionResult<object>> Register([FromBody] RegisterRequest request)
        {
            var user = await _authService.RegisterAsync(request);
            return Ok(user);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] object request) // Placeholder
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            // O ideal é criar um ChangePasswordRequest DTO aqui.
            // Por enquanto, a lógica no serviço vai lidar com o 'object'.
            // var result = await _authService.ChangePasswordAsync(userId, request...);
            return Ok(new { message = "Password change endpoint is under construction." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] object request) // Placeholder
        {
            await _authService.ForgotPasswordAsync(request);
            return Ok(new { message = "If the email exists, a recovery link will be sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] object request) // Placeholder
        {
            await _authService.ResetPasswordAsync(request);
            return Ok(new { message = "Password reset successfully." });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<object>> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _authService.GetProfileAsync(userId);
            return Ok(user);
        }

        [HttpGet("verify")]
        [Authorize]
        public IActionResult VerifyToken()
        {
            return Ok(new { message = "Token is valid." });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<object>> RefreshToken([FromBody] object request) // Placeholder
        {
            var response = await _authService.RefreshTokenAsync(request);
            return Ok(response);
        }
    }
} 