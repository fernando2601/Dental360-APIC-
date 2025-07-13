using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _service;
        public RoomController(IRoomService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IEnumerable<Room>> GetAll() => await _service.GetAllAsync();
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetById(int id)
        {
            var room = await _service.GetByIdAsync(id);
            if (room == null) return NotFound();
            return room;
        }
        [HttpPost]
        public async Task<ActionResult<Room>> Create(Room room)
        {
            var created = await _service.CreateAsync(room);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Room>> Update(int id, Room room)
        {
            var updated = await _service.UpdateAsync(id, room);
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