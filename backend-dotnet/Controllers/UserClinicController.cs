using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserClinicController : ControllerBase
    {
        private readonly IUserClinicService _service;
        public UserClinicController(IUserClinicService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IEnumerable<UserClinic>> GetAll() => await _service.GetAllAsync();
        [HttpGet("{userId}/{clinicId}")]
        public async Task<ActionResult<UserClinic>> GetByIds(int userId, int clinicId)
        {
            var uc = await _service.GetByIdsAsync(userId, clinicId);
            if (uc == null) return NotFound();
            return uc;
        }
        [HttpPost]
        public async Task<ActionResult<UserClinic>> Create(UserClinic userClinic)
        {
            var created = await _service.CreateAsync(userClinic);
            return CreatedAtAction(nameof(GetByIds), new { userId = created.UserId, clinicId = created.ClinicId }, created);
        }
        [HttpPut("{userId}/{clinicId}")]
        public async Task<ActionResult<UserClinic>> Update(int userId, int clinicId, UserClinic userClinic)
        {
            var updated = await _service.UpdateAsync(userId, clinicId, userClinic);
            if (updated == null) return NotFound();
            return updated;
        }
        [HttpDelete("{userId}/{clinicId}")]
        public async Task<IActionResult> Delete(int userId, int clinicId)
        {
            var deleted = await _service.DeleteAsync(userId, clinicId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 