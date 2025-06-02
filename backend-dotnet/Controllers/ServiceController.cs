using Microsoft.AspNetCore.Mvc;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace ClinicApi.Controllers
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
        public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetAllServices()
        {
            try
            {
                var services = await _serviceService.GetAllServicesAsync();
                return Ok(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar serviços");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse>> GetServiceById(int id)
        {
            try
            {
                var service = await _serviceService.GetServiceByIdAsync(id);
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
        public async Task<ActionResult<ServiceResponse>> CreateService([FromBody] CreateServiceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var service = await _serviceService.CreateServiceAsync(request);
                return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, service);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar serviço");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse>> UpdateService(int id, [FromBody] UpdateServiceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var service = await _serviceService.UpdateServiceAsync(id, request);
                if (service == null)
                {
                    return NotFound(new { message = "Serviço não encontrado" });
                }

                return Ok(service);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar serviço {ServiceId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
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
        public async Task<ActionResult<ServiceStatsResponse>> GetServiceStats()
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