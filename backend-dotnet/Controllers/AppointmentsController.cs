using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAgendaService _agendaService;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAgendaService agendaService, IAppointmentService appointmentService)
        {
            _agendaService = agendaService;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAll()
        {
            var appointments = await _agendaService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentResponse>> GetById(int id)
        {
            var appointment = await _agendaService.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentResponse>> Create([FromBody] Appointment appointment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var newAppointment = await _agendaService.CreateAsync(appointment);
                var response = new AppointmentResponse
                {
                    ClientId = newAppointment.ClientId,
                    StaffId = newAppointment.StaffId,
                    ServiceId = newAppointment.ServiceId,
                    StartTime = newAppointment.StartTime,
                    EndTime = newAppointment.EndTime,
                    Status = newAppointment.Status,
                    Notes = newAppointment.Notes,
                    Room = newAppointment.Room,
                    Price = newAppointment.Price,
                    PaymentStatus = newAppointment.PaymentStatus,
                    PaymentMethod = newAppointment.PaymentMethod,
                    IsRecurring = newAppointment.IsRecurring,
                    RecurrencePattern = newAppointment.RecurrencePattern,
                    RecurrenceEndDate = newAppointment.RecurrenceEndDate,
                    ParentAppointmentId = newAppointment.ParentAppointmentId,
                    CancellationReason = newAppointment.CancellationReason,
                    CancelledAt = newAppointment.CancelledAt,
                    ConfirmedAt = newAppointment.ConfirmedAt,
                    CompletedAt = newAppointment.CompletedAt,
                    ClientFeedback = newAppointment.ClientFeedback,
                    Rating = newAppointment.Rating,
                    CreatedAt = newAppointment.CreatedAt,
                    UpdatedAt = newAppointment.UpdatedAt
                };
                return CreatedAtAction(nameof(GetById), null, response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
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

        [HttpGet("staff/{staffId}/busy-times")]
        public async Task<ActionResult<IEnumerable<object>>> GetBusyTimes(int staffId, [FromQuery] DateTime date)
        {
            var appointments = await _agendaService.GetAppointmentsByStaffAndDateAsync(staffId, date);
            var busyTimes = appointments.Select(a => new { a.StartTime, a.EndTime });
            return Ok(busyTimes);
        }

        [HttpGet("staff/{staffId}/busy-times-service")]
        public async Task<ActionResult<IEnumerable<object>>> GetBusyTimesService(int staffId, [FromQuery] DateTime date)
        {
            var appointments = await _appointmentService.GetBusyTimesAsync(staffId, date);
            var busyTimes = appointments.Select(a => new { a.StartTime, a.EndTime });
            return Ok(busyTimes);
        }
    }
}