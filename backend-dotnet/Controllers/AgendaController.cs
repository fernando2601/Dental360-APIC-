using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
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
        public async Task<ActionResult<Appointment>> CreateAppointment(CreateAppointmentDto appointmentDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var appointment = await _agendaService.CreateAppointmentAsync(appointmentDto, userId);
                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
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
        public async Task<ActionResult<Appointment>> UpdateAppointment(int id, UpdateAppointmentDto appointmentDto)
        {
            try
            {
                var appointment = await _agendaService.UpdateAppointmentAsync(id, appointmentDto);
                if (appointment == null)
                    return NotFound();

                return Ok(appointment);
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

        [HttpGet("calendar")]
        public async Task<ActionResult> GetCalendarView(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _agendaService.GetCalendarViewAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("today")]
        public async Task<ActionResult> GetTodayAppointments()
        {
            var result = await _agendaService.GetTodayAppointmentsAsync();
            return Ok(result);
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult> GetUpcomingAppointments([FromQuery] int days = 7)
        {
            var result = await _agendaService.GetUpcomingAppointmentsAsync(days);
            return Ok(result);
        }

        [HttpGet("availability/check")]
        public async Task<ActionResult> CheckAvailability(
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime,
            [FromQuery] int? staffId = null)
        {
            var result = await _agendaService.CheckAvailabilityAsync(startTime, endTime, staffId);
            return Ok(result);
        }

        [HttpGet("availability/slots")]
        public async Task<ActionResult> GetAvailableTimeSlots(
            [FromQuery] DateTime date,
            [FromQuery] int durationMinutes = 60,
            [FromQuery] int? staffId = null)
        {
            var result = await _agendaService.GetAvailableTimeSlotsAsync(date, durationMinutes, staffId);
            return Ok(result);
        }

        [HttpPost("{id}/confirm")]
        public async Task<ActionResult> ConfirmAppointment(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _agendaService.ConfirmAppointmentAsync(id, userId);
            
            if (!result)
                return NotFound();

            return Ok(new { message = "Consulta confirmada com sucesso" });
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelAppointment(int id, [FromBody] object request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                // Em uma implementação real, extrairia o motivo do request
                var result = await _agendaService.CancelAppointmentAsync(id, "Cancelado pelo usuário", userId);
                
                if (!result)
                    return NotFound();

                return Ok(new { message = "Consulta cancelada com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> CompleteAppointment(int id, [FromBody] object request)
        {
            // Em uma implementação real, extrairia actualCost e notes do request
            var result = await _agendaService.CompleteAppointmentAsync(id, null, null);
            
            if (!result)
                return NotFound();

            return Ok(new { message = "Consulta finalizada com sucesso" });
        }

        [HttpPost("{id}/no-show")]
        public async Task<ActionResult> MarkNoShow(int id, [FromBody] object request)
        {
            // Em uma implementação real, extrairia notes do request
            var result = await _agendaService.MarkNoShowAsync(id, null);
            
            if (!result)
                return NotFound();

            return Ok(new { message = "Paciente marcado como faltoso" });
        }

        [HttpPost("{id}/reschedule")]
        public async Task<ActionResult> RescheduleAppointment(int id, [FromBody] RescheduleRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                request.AppointmentId = id;
                var result = await _agendaService.RescheduleAppointmentAsync(request, userId);
                
                if (!result)
                    return NotFound();

                return Ok(new { message = "Consulta reagendada com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult> GetAppointmentsWithFilters([FromQuery] AgendaFilters filters)
        {
            var result = await _agendaService.GetAppointmentsWithFiltersAsync(filters);
            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult> GetAppointmentsByPatient(int patientId)
        {
            var result = await _agendaService.GetAppointmentsByPatientAsync(patientId);
            return Ok(result);
        }

        [HttpGet("staff/{staffId}")]
        public async Task<ActionResult> GetAppointmentsByStaff(
            int staffId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _agendaService.GetAppointmentsByStaffAsync(staffId, startDate, endDate);
            return Ok(result);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult> GetAgendaStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _agendaService.GetAgendaStatisticsAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("dashboard-metrics")]
        public async Task<ActionResult> GetDashboardMetrics()
        {
            var result = await _agendaService.GetDashboardMetricsAsync();
            return Ok(result);
        }

        [HttpGet("analytics/hourly")]
        public async Task<ActionResult> GetHourlyDistribution(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _agendaService.GetHourlyDistributionAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("analytics/weekly")]
        public async Task<ActionResult> GetWeeklyDistribution(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _agendaService.GetWeeklyDistributionAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("analytics/status")]
        public async Task<ActionResult> GetStatusDistribution(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _agendaService.GetStatusDistributionAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpPost("bulk-update")]
        public async Task<ActionResult> BulkUpdateAppointments([FromBody] BulkAppointmentAction action)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _agendaService.BulkUpdateAppointmentsAsync(action, userId);
            
            return Ok(new { success = result, message = "Operação em lote realizada com sucesso" });
        }

        [HttpGet("reports/appointments")]
        public async Task<ActionResult> GetAppointmentReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string reportType = "summary")
        {
            var result = await _agendaService.GetAppointmentReportAsync(startDate, endDate, reportType);
            return Ok(result);
        }

        [HttpGet("reports/staff-productivity")]
        public async Task<ActionResult> GetStaffProductivityReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _agendaService.GetStaffProductivityReportAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("reports/service-utilization")]
        public async Task<ActionResult> GetServiceUtilizationReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _agendaService.GetServiceUtilizationReportAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("rooms/available")]
        public async Task<ActionResult> GetAvailableRooms(
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime)
        {
            var result = await _agendaService.GetAvailableRoomsAsync(startTime, endTime);
            return Ok(result);
        }

        [HttpGet("rooms/utilization")]
        public async Task<ActionResult> GetRoomUtilization(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _agendaService.GetRoomUtilizationAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpPost("{id}/notify")]
        public async Task<ActionResult> SendAppointmentNotification(int id, [FromQuery] string type)
        {
            var result = await _agendaService.SendAppointmentNotificationAsync(id, type);
            
            if (!result)
                return BadRequest(new { message = "Não foi possível enviar a notificação" });

            return Ok(new { message = "Notificação enviada com sucesso" });
        }
    }
}