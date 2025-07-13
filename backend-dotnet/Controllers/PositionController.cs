using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _service;
        public PositionController(IPositionService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IEnumerable<Position>> GetAll() => await _service.GetAllAsync();
        [HttpGet("{id}")]
        public async Task<ActionResult<Position>> GetById(int id)
        {
            var position = await _service.GetByIdAsync(id);
            if (position == null) return NotFound();
            return position;
        }
        [HttpPost]
        public async Task<ActionResult<Position>> Create(Position position)
        {
            var created = await _service.CreateAsync(position);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Position>> Update(int id, Position position)
        {
            var updated = await _service.UpdateAsync(id, position);
            if (updated == null) return NotFound();
            return updated;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 