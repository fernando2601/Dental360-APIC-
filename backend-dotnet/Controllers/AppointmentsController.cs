using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAgendaService _agendaService;

        public AppointmentsController(IAgendaService agendaService)
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
            var newAppointment = await _agendaService.CreateAsync(appointment);
            return CreatedAtAction(nameof(GetById), new { id = newAppointment.Id }, newAppointment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Appointment>> Update(int id, [FromBody] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return BadRequest("O ID do agendamento não corresponde.");
            }

            var updatedAppointment = await _agendaService.UpdateAsync(id, appointment);
            if (updatedAppointment == null)
                return NotFound();

            return Ok(updatedAppointment);
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