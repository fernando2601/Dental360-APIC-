using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Application.Interfaces;
using DentalSpa.Domain.Entities;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AgendaController : ControllerBase
    {
        private readonly IAgendaService _agendaService;

        public AgendaController(IAgendaService agendaService)
        {
            _agendaService = agendaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAll()
        {
            var appointments = await _agendaService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetById(int id)
        {
            var appointment = await _agendaService.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> Create([FromBody] Appointment appointment)
        {
            try
            {
                var newAppointment = await _agendaService.CreateAsync(appointment);
                return CreatedAtAction(nameof(GetById), new { id = newAppointment.Id }, newAppointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Appointment>> Update(int id, [FromBody] Appointment appointment)
        {
            try
            {
                var updatedAppointment = await _agendaService.UpdateAsync(id, appointment);
                if (updatedAppointment == null)
                    return NotFound();
                
                return Ok(updatedAppointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _agendaService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Appointment>>> Search([FromQuery] string searchTerm)
        {
            var appointments = await _agendaService.SearchAsync(searchTerm);
            return Ok(appointments);
        }
    }
} 