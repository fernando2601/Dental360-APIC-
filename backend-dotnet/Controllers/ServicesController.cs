using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClinicApi.Models;
using ClinicApi.Services;

namespace ClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetAllServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null)
                return NotFound();

            return Ok(service);
        }

        [HttpPost]
        public async Task<ActionResult<Service>> CreateService(CreateServiceDto serviceDto)
        {
            var service = await _serviceService.CreateServiceAsync(serviceDto);
            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Service>> UpdateService(int id, CreateServiceDto serviceDto)
        {
            var service = await _serviceService.UpdateServiceAsync(id, serviceDto);
            if (service == null)
                return NotFound();

            return Ok(service);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _serviceService.DeleteServiceAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Service>>> SearchServices([FromQuery] string term)
        {
            var services = await _serviceService.SearchServicesAsync(term);
            return Ok(services);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServicesByCategory(string category)
        {
            var services = await _serviceService.GetServicesByCategoryAsync(category);
            return Ok(services);
        }
    }
}