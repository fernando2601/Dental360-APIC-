using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAll()
            => Ok(await _patientService.GetAllPatientsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetById(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> Create([FromBody] Patient patient)
        {
            var created = await _patientService.CreatePatientAsync(patient);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Patient>> Update(int id, [FromBody] Patient patient)
        {
            var updated = await _patientService.UpdatePatientAsync(id, patient);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _patientService.DeletePatientAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Patient>>> Search([FromQuery] string query)
            => Ok(await _patientService.SearchPatientsAsync(query));
    }
} 