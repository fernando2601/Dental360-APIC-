using Microsoft.AspNetCore.Mvc;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace ClinicApi.Controllers
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
        public async Task<ActionResult<IEnumerable<BeforeAfterResponse>>> GetAllBeforeAfter()
        {
            try
            {
                var cases = await _beforeAfterService.GetAllBeforeAfterAsync();
                return Ok(cases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after cases");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BeforeAfterModel>> GetBeforeAfterById(int id)
        {
            try
            {
                var beforeAfterCase = await _beforeAfterService.GetBeforeAfterByIdAsync(id);
                if (beforeAfterCase == null)
                {
                    return NotFound(new { message = "Caso não encontrado" });
                }

                // Increment view count
                await _beforeAfterService.IncrementViewCountAsync(id);

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
        public async Task<ActionResult<BeforeAfterResponse>> CreateBeforeAfter([FromBody] CreateBeforeAfterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var beforeAfterCase = await _beforeAfterService.CreateBeforeAfterAsync(request);
                return CreatedAtAction(nameof(GetBeforeAfterById), new { id = beforeAfterCase.Id }, beforeAfterCase);
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
        public async Task<ActionResult<BeforeAfterResponse>> UpdateBeforeAfter(int id, [FromBody] UpdateBeforeAfterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var beforeAfterCase = await _beforeAfterService.UpdateBeforeAfterAsync(id, request);
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
        public async Task<ActionResult> DeleteBeforeAfter(int id)
        {
            try
            {
                var success = await _beforeAfterService.DeleteBeforeAfterAsync(id);
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

        [HttpGet("stats")]
        public async Task<ActionResult<BeforeAfterStatsResponse>> GetBeforeAfterStats()
        {
            try
            {
                var stats = await _beforeAfterService.GetBeforeAfterStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after statistics");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("public")]
        public async Task<ActionResult<IEnumerable<BeforeAfterResponse>>> GetPublicBeforeAfter()
        {
            try
            {
                var cases = await _beforeAfterService.GetPublicBeforeAfterAsync();
                return Ok(cases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving public before/after cases");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("treatment-type/{treatmentType}")]
        public async Task<ActionResult<IEnumerable<BeforeAfterResponse>>> GetBeforeAfterByTreatmentType(string treatmentType)
        {
            try
            {
                var cases = await _beforeAfterService.GetBeforeAfterByTreatmentTypeAsync(treatmentType);
                return Ok(cases);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after cases by treatment type: {TreatmentType}", treatmentType);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BeforeAfterResponse>>> SearchBeforeAfter([FromQuery] string term)
        {
            try
            {
                var cases = await _beforeAfterService.SearchBeforeAfterAsync(term ?? string.Empty);
                return Ok(cases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching before/after cases with term: {SearchTerm}", term);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("treatment-types")]
        public async Task<ActionResult<IEnumerable<string>>> GetTreatmentTypes()
        {
            try
            {
                var treatmentTypes = await _beforeAfterService.GetTreatmentTypesAsync();
                return Ok(treatmentTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving treatment types");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("{id}/rating")]
        public async Task<ActionResult> AddRating(int id, [FromBody] CreateRatingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _beforeAfterService.AddRatingAsync(id, request);
                if (!success)
                {
                    return BadRequest(new { message = "Não foi possível adicionar a avaliação" });
                }

                return Ok(new { message = "Avaliação adicionada com sucesso" });
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
                _logger.LogError(ex, "Error adding rating for case: {CaseId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}/ratings")]
        public async Task<ActionResult<IEnumerable<BeforeAfterRatingModel>>> GetRatings(int id)
        {
            try
            {
                var ratings = await _beforeAfterService.GetRatingsAsync(id);
                return Ok(ratings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for case: {CaseId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}