using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecializationController : ControllerBase
    {
        private readonly ISpecializationService _service;
        public SpecializationController(ISpecializationService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IEnumerable<Specialization>> GetAll() => await _service.GetAllAsync();
        [HttpGet("{id}")]
        public async Task<ActionResult<Specialization>> GetById(int id)
        {
            var specialization = await _service.GetByIdAsync(id);
            if (specialization == null) return NotFound();
            return specialization;
        }
        [HttpPost]
        public async Task<ActionResult<Specialization>> Create(Specialization specialization)
        {
            var created = await _service.CreateAsync(specialization);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Specialization>> Update(int id, Specialization specialization)
        {
            var updated = await _service.UpdateAsync(id, specialization);
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