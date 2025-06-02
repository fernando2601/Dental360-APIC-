using Microsoft.AspNetCore.Mvc;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace ClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningController : ControllerBase
    {
        private readonly ILearningService _learningService;
        private readonly ILogger<LearningController> _logger;

        public LearningController(ILearningService learningService, ILogger<LearningController> logger)
        {
            _learningService = learningService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LearningContentResponse>>> GetAllContent([FromQuery] int? userId = null)
        {
            try
            {
                var content = await _learningService.GetAllContentAsync(userId);
                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo de aprendizado");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LearningContentResponse>> GetContentById(int id, [FromQuery] int? userId = null)
        {
            try
            {
                var content = await _learningService.GetContentByIdAsync(id, userId);
                if (content == null)
                {
                    return NotFound(new { message = "Conteúdo não encontrado" });
                }

                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo {ContentId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<LearningContentResponse>> CreateContent([FromBody] CreateLearningContentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // In a real implementation, get user ID from authentication context
                var createdBy = 1; // TODO: Get from authenticated user context

                var content = await _learningService.CreateContentAsync(request, createdBy);
                return CreatedAtAction(nameof(GetContentById), new { id = content.Id }, content);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conteúdo");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LearningContentResponse>> UpdateContent(int id, [FromBody] UpdateLearningContentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var content = await _learningService.UpdateContentAsync(id, request);
                if (content == null)
                {
                    return NotFound(new { message = "Conteúdo não encontrado" });
                }

                return Ok(content);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar conteúdo {ContentId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteContent(int id)
        {
            try
            {
                var deleted = await _learningService.DeleteContentAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Conteúdo não encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir conteúdo {ContentId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<LearningContentResponse>>> GetContentByCategory(string category)
        {
            try
            {
                var content = await _learningService.GetContentByCategoryAsync(category);
                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo da categoria {Category}", category);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<LearningContentResponse>>> SearchContent([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(new { message = "Termo de busca é obrigatório" });
                }

                var content = await _learningService.SearchContentAsync(term);
                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo com termo {SearchTerm}", term);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<LearningStatsResponse>> GetStats()
        {
            try
            {
                var stats = await _learningService.GetStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas de aprendizado");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            try
            {
                var categories = await _learningService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("{contentId}/progress")]
        public async Task<ActionResult<LearningProgressModel>> UpdateProgress(int contentId, [FromBody] UpdateProgressRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // In a real implementation, get user ID from authentication context
                var userId = 1; // TODO: Get from authenticated user context

                var progress = await _learningService.UpdateProgressAsync(contentId, userId, request);
                if (progress == null)
                {
                    return NotFound(new { message = "Conteúdo não encontrado" });
                }

                return Ok(progress);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar progresso do conteúdo {ContentId}", contentId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost("{contentId}/comments")]
        public async Task<ActionResult<LearningCommentModel>> AddComment(int contentId, [FromBody] AddCommentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // In a real implementation, get user ID from authentication context
                var userId = 1; // TODO: Get from authenticated user context

                var comment = await _learningService.AddCommentAsync(contentId, userId, request);
                return CreatedAtAction(nameof(GetContentById), new { id = contentId }, comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar comentário ao conteúdo {ContentId}", contentId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("progress/user/{userId}")]
        public async Task<ActionResult<IEnumerable<LearningProgressModel>>> GetUserProgress(int userId)
        {
            try
            {
                var progress = await _learningService.GetUserProgressAsync(userId);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar progresso do usuário {UserId}", userId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("recommended/user/{userId}")]
        public async Task<ActionResult<IEnumerable<LearningContentResponse>>> GetRecommendedContent(int userId)
        {
            try
            {
                var content = await _learningService.GetRecommendedContentAsync(userId);
                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo recomendado para usuário {UserId}", userId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}