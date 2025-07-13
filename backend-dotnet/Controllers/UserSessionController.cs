using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserSessionController : ControllerBase
    {
        private readonly IUserSessionService _service;
        public UserSessionController(IUserSessionService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IEnumerable<UserSession>> GetAll() => await _service.GetAllAsync();
        [HttpGet("{sessionId}")]
        public async Task<ActionResult<UserSession>> GetById(int sessionId)
        {
            var session = await _service.GetByIdAsync(sessionId);
            if (session == null) return NotFound();
            return session;
        }
        [HttpPost]
        public async Task<ActionResult<UserSession>> Create(UserSession session)
        {
            var created = await _service.CreateAsync(session);
            return CreatedAtAction(nameof(GetById), new { sessionId = created.SessionId }, created);
        }
        [HttpPut("{sessionId}")]
        public async Task<ActionResult<UserSession>> Update(int sessionId, UserSession session)
        {
            var updated = await _service.UpdateAsync(sessionId, session);
            if (updated == null) return NotFound();
            return updated;
        }
        [HttpDelete("{sessionId}")]
        public async Task<IActionResult> Delete(int sessionId)
        {
            var deleted = await _service.DeleteAsync(sessionId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 