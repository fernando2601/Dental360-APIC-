using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClinicApi.Models;
using ClinicApi.Services;

namespace ClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(CreateAppointmentDto appointmentDto)
        {
            var appointment = await _appointmentService.CreateAppointmentAsync(appointmentDto);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Appointment>> UpdateAppointment(int id, CreateAppointmentDto appointmentDto)
        {
            var appointment = await _appointmentService.UpdateAppointmentAsync(id, appointmentDto);
            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var result = await _appointmentService.DeleteAppointmentAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("reports")]
        public async Task<ActionResult> GetAppointmentReports(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string[]? status,
            [FromQuery] int? professionalId,
            [FromQuery] int? clientId,
            [FromQuery] string? convenio,
            [FromQuery] string? sala,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25)
        {
            var result = await _appointmentService.GetAppointmentReportsAsync(
                startDate, endDate, status, professionalId, clientId, convenio, sala, page, limit);
            
            return Ok(result);
        }
    }
}