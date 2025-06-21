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
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (!result)
            {
                return BadRequest(new { message = "Failed to change password. Check old password." });
            }
            return Ok(new { message = "Password changed successfully." });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.ForgotPasswordAsync(request);
            if (!result)
            {
                // Não retorne erro para não revelar se um e-mail existe ou não.
                // Apenas retorne uma mensagem genérica.
            }
            return Ok(new { message = "If an account with that email exists, we have sent a password reset link." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);
            if (!result)
            {
                return BadRequest(new { message = "Invalid token or email." });
            }
            return Ok(new { message = "Password has been reset successfully." });
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
        [AllowAnonymous]
        public async Task<ActionResult<object>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _authService.RefreshTokenAsync(request);
            if (response == null)
            {
                return Unauthorized("Invalid refresh token.");
            }
            return Ok(response);
        }
    }
} 