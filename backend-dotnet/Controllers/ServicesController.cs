using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
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
        public async Task<ActionResult<IEnumerable<Service>>> GetAll()
            => Ok(await _serviceService.GetAllServicesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetById(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null) return NotFound();
            return Ok(service);
        }

        [HttpPost]
        public async Task<ActionResult<Service>> Create([FromBody] Service service)
        {
            var created = await _serviceService.CreateServiceAsync(service);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Service>> Update(int id, [FromBody] Service service)
        {
            var updated = await _serviceService.UpdateServiceAsync(id, service);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _serviceService.DeleteServiceAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Service>>> Search([FromQuery] string term)
            => Ok(await _serviceService.SearchServicesAsync(term));
    }
} 