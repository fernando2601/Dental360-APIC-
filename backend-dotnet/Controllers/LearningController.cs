using Microsoft.AspNetCore.Mvc;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningController : ControllerBase
    {
        private readonly ILearningService _learningService;

        public LearningController(ILearningService learningService)
        {
            _learningService = learningService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LearningArea>>> GetAll()
            => Ok(await _learningService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<LearningArea>> GetById(int id)
        {
            var entity = await _learningService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult<LearningArea>> Create([FromBody] LearningArea entity)
        {
            var created = await _learningService.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LearningArea>> Update(int id, [FromBody] LearningArea entity)
        {
            var updated = await _learningService.UpdateAsync(id, entity);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _learningService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 