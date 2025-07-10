using DentalSpa.Application.DTOs;
using DentalSpa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfissionalSaudeController : ControllerBase
    {
        private readonly IProfissionalSaudeService _service;
        public ProfissionalSaudeController(IProfissionalSaudeService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfissionalSaudeResponse>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("staff/{staffId}")]
        public async Task<ActionResult<ProfissionalSaudeResponse>> GetByStaffId(int staffId)
        {
            var result = await _service.GetByStaffIdAsync(staffId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfissionalSaudeResponse>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ProfissionalSaudeResponse>> Create(ProfissionalSaudeCreateRequest request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProfissionalSaudeResponse>> Update(int id, ProfissionalSaudeCreateRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
} 