using Microsoft.AspNetCore.Mvc;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(IServiceService serviceService, ILogger<ServiceController> logger)
        {
            _serviceService = serviceService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetAll()
        {
            try
            {
                var services = await _serviceService.GetAllAsync();
                return Ok(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar serviços");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse>> GetById(int id)
        {
            try
            {
                var service = await _serviceService.GetByIdAsync(id);
                if (service == null)
                {
                    return NotFound(new { message = "Serviço não encontrado" });
                }

                return Ok(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar serviço {ServiceId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> Create([FromBody] ServiceCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _serviceService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), null, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse>> Update(int id, [FromBody] ServiceCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _serviceService.UpdateAsync(id, request);
            if (response == null)
                return NotFound();
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteService(int id)
        {
            try
            {
                var deleted = await _serviceService.DeleteServiceAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Serviço não encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir serviço {ServiceId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetServicesByCategory(string category)
        {
            try
            {
                var services = await _serviceService.GetServicesByCategoryAsync(category);
                return Ok(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar serviços da categoria {Category}", category);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetServiceStats()
        {
            try
            {
                var stats = await _serviceService.GetServiceStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas dos serviços");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            try
            {
                var categories = await _serviceService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}