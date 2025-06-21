using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Application.Interfaces;
using DentalSpa.Domain.Entities;
using System.Security.Claims;

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
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
        {
            var appointments = await _agendaService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _agendaService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] Appointment appointment)
        {
            try
            {
                var newAppointment = await _agendaService.CreateAppointmentAsync(appointment);
                return CreatedAtAction(nameof(GetAppointment), new { id = newAppointment.Id }, newAppointment);
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
        public async Task<ActionResult<Appointment>> UpdateAppointment(int id, [FromBody] Appointment appointment)
        {
            try
            {
                var updatedAppointment = await _agendaService.UpdateAppointmentAsync(id, appointment);
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
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var result = await _agendaService.DeleteAppointmentAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Appointment>>> SearchAppointments([FromQuery] string searchTerm)
        {
            var appointments = await _agendaService.SearchAppointmentsAsync(searchTerm);
            return Ok(appointments);
        }
    }
} 