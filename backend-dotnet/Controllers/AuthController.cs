using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClinicApi.Models;
using ClinicApi.Services;
using System.Security.Claims;

namespace ClinicApi.Controllers
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
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
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

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request);
                return CreatedAtAction(nameof(GetProfile), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _authService.LogoutAsync(token);
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(refreshToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserProfile>> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var profile = await _authService.GetUserProfileAsync(userId);
            
            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult> UpdateProfile(UserProfile profile)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _authService.UpdateUserProfileAsync(userId, profile);
            
            if (!result)
                return BadRequest(new { message = "Não foi possível atualizar o perfil" });

            return Ok(new { message = "Perfil atualizado com sucesso" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var result = await _authService.ChangePasswordAsync(userId, request);
                
                if (!result)
                    return BadRequest(new { message = "Não foi possível alterar a senha" });

                return Ok(new { message = "Senha alterada com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            await _authService.ForgotPasswordAsync(request);
            return Ok(new { message = "Se o email existir, você receberá instruções para redefinir sua senha" });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(request);
                
                if (!result)
                    return BadRequest(new { message = "Token inválido ou expirado" });

                return Ok(new { message = "Senha redefinida com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("validate-reset-token")]
        public async Task<ActionResult> ValidateResetToken([FromQuery] string token, [FromQuery] string email)
        {
            var isValid = await _authService.ValidateResetTokenAsync(token, email);
            return Ok(new { isValid });
        }

        [HttpGet("dashboard")]
        [Authorize]
        public async Task<ActionResult<DashboardMetrics>> GetDashboard()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var metrics = await _authService.GetDashboardMetricsAsync(userId);
            return Ok(metrics);
        }

        [HttpGet("recent-activities")]
        [Authorize]
        public async Task<ActionResult> GetRecentActivities([FromQuery] int limit = 10)
        {
            var activities = await _authService.GetRecentActivitiesAsync(limit);
            return Ok(activities);
        }

        [HttpGet("system-info")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<SystemInfo>> GetSystemInfo()
        {
            var systemInfo = await _authService.GetSystemInfoAsync();
            return Ok(systemInfo);
        }

        [HttpGet("users")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.FullName,
                u.Role,
                u.IsActive,
                u.LastLogin,
                u.CreatedAt
            }));
        }

        [HttpGet("users/{id}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.Role,
                user.IsActive,
                user.LastLogin,
                user.CreatedAt
            });
        }

        [HttpPut("users/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateUser(int id, User user)
        {
            try
            {
                var updatedUser = await _authService.UpdateUserAsync(id, user);
                if (updatedUser == null)
                    return NotFound();

                return Ok(new { message = "Usuário atualizado com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("users/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await _authService.DeleteUserAsync(id);
            if (!result)
                return NotFound();

            return Ok(new { message = "Usuário desativado com sucesso" });
        }

        [HttpGet("check-username")]
        public async Task<ActionResult> CheckUsername([FromQuery] string username)
        {
            var isAvailable = await _authService.IsUsernameAvailableAsync(username);
            return Ok(new { isAvailable });
        }

        [HttpGet("check-email")]
        public async Task<ActionResult> CheckEmail([FromQuery] string email)
        {
            var isAvailable = await _authService.IsEmailAvailableAsync(email);
            return Ok(new { isAvailable });
        }

        [HttpPost("revoke-all-sessions")]
        [Authorize]
        public async Task<ActionResult> RevokeAllSessions()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _authService.RevokeAllSessionsAsync(userId);
            return Ok(new { message = "Todas as sessões foram revogadas" });
        }

        [HttpGet("statistics")]
        [Authorize]
        public async Task<ActionResult> GetUserStatistics()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var statistics = await _authService.GetUserStatisticsAsync(userId);
            return Ok(statistics);
        }

        [HttpGet("activity-log")]
        [Authorize]
        public async Task<ActionResult> GetActivityLog(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var log = await _authService.GetUserActivityLogAsync(userId, startDate, endDate);
            return Ok(log);
        }

        [HttpGet("security-alerts")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> GetSecurityAlerts()
        {
            var alerts = await _authService.GetSecurityAlertsAsync();
            return Ok(alerts);
        }

        [HttpPost("validate-token")]
        public async Task<ActionResult> ValidateToken([FromBody] string token)
        {
            var isValid = await _authService.ValidateTokenAsync(token);
            return Ok(new { isValid });
        }
    }
}