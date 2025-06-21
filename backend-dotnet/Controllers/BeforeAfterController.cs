using Microsoft.AspNetCore.Mvc;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/before-after")]
    public class BeforeAfterController : ControllerBase
    {
        private readonly IBeforeAfterService _beforeAfterService;
        private readonly ILogger<BeforeAfterController> _logger;

        public BeforeAfterController(IBeforeAfterService beforeAfterService, ILogger<BeforeAfterController> logger)
        {
            _beforeAfterService = beforeAfterService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BeforeAfter>>> GetAll()
        {
            try
            {
                var cases = await _beforeAfterService.GetAllAsync();
                return Ok(cases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after cases");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BeforeAfter>> GetById(int id)
        {
            try
            {
                var beforeAfterCase = await _beforeAfterService.GetByIdAsync(id);
                if (beforeAfterCase == null)
                {
                    return NotFound(new { message = "Caso não encontrado" });
                }
                return Ok(beforeAfterCase);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after case with ID: {CaseId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<BeforeAfter>> Create([FromBody] BeforeAfter beforeAfter)
        {
            try
            {
                var created = await _beforeAfterService.CreateAsync(beforeAfter);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating before/after case");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BeforeAfter>> Update(int id, [FromBody] BeforeAfter beforeAfter)
        {
            try
            {
                var updated = await _beforeAfterService.UpdateAsync(id, beforeAfter);
                if (updated == null)
                {
                    return NotFound(new { message = "Caso não encontrado" });
                }
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating before/after case with ID: {CaseId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var success = await _beforeAfterService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Caso não encontrado" });
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting before/after case with ID: {CaseId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("public")]
        public async Task<ActionResult<IEnumerable<BeforeAfter>>> GetPublic()
        {
            try
            {
                var cases = await _beforeAfterService.GetPublicAsync();
                return Ok(cases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving public before/after cases");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("service/{serviceId}")]
        public async Task<ActionResult<IEnumerable<BeforeAfter>>> GetByService(int serviceId)
        {
            try
            {
                var cases = await _beforeAfterService.GetByServiceAsync(serviceId);
                return Ok(cases);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after cases by service: {ServiceId}", serviceId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BeforeAfter>>> Search([FromQuery] string term)
        {
            try
            {
                var cases = await _beforeAfterService.SearchAsync(term ?? string.Empty);
                return Ok(cases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching before/after cases with term: {SearchTerm}", term);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}