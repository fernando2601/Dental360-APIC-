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

        // CRUD Básico
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
            var appointment = await _agendaService.CreateAppointmentAsync(appointmentDto);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Appointment>> UpdateAppointment(int id, CreateAppointmentDto appointmentDto)
        {
            var appointment = await _agendaService.UpdateAppointmentAsync(id, appointmentDto);
            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var result = await _agendaService.DeleteAppointmentAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Calendar Views
        [HttpGet("calendar")]
        public async Task<ActionResult> GetCalendarView(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? staffId = null,
            [FromQuery] string viewType = "month")
        {
            var result = await _agendaService.GetCalendarViewAsync(startDate, endDate, staffId, viewType);
            return Ok(result);
        }

        [HttpGet("day/{date}")]
        public async Task<ActionResult> GetDayView(DateTime date, [FromQuery] int? staffId = null)
        {
            var result = await _agendaService.GetDayViewAsync(date, staffId);
            return Ok(result);
        }

        [HttpGet("week/{weekStart}")]
        public async Task<ActionResult> GetWeekView(DateTime weekStart, [FromQuery] int? staffId = null)
        {
            var result = await _agendaService.GetWeekViewAsync(weekStart, staffId);
            return Ok(result);
        }

        [HttpGet("month/{monthStart}")]
        public async Task<ActionResult> GetMonthView(DateTime monthStart, [FromQuery] int? staffId = null)
        {
            var result = await _agendaService.GetMonthViewAsync(monthStart, staffId);
            return Ok(result);
        }

        // Schedule Management
        [HttpGet("schedule-overview/{date}")]
        public async Task<ActionResult> GetScheduleOverview(DateTime date)
        {
            var result = await _agendaService.GetScheduleOverviewAsync(date);
            return Ok(result);
        }

        [HttpGet("staff-availability/{date}")]
        public async Task<ActionResult> GetStaffAvailability(DateTime date, [FromQuery] int? staffId = null)
        {
            var result = await _agendaService.GetStaffAvailabilityAsync(date, staffId);
            return Ok(result);
        }

        [HttpGet("available-slots")]
        public async Task<ActionResult> GetAvailableSlots(
            [FromQuery] DateTime date,
            [FromQuery] int staffId,
            [FromQuery] int serviceDuration = 60)
        {
            var result = await _agendaService.GetAvailableSlotsAsync(date, staffId, serviceDuration);
            return Ok(result);
        }

        // Smart Scheduling
        [HttpPost("find-best-slot")]
        public async Task<ActionResult> FindBestSlot([FromBody] object request)
        {
            // Parsing do request seria feito aqui
            var result = await _agendaService.FindBestAvailableSlotAsync(1, 1, DateTime.Today.AddDays(1), 60);
            return Ok(result);
        }

        [HttpPost("suggest-alternatives")]
        public async Task<ActionResult> SuggestAlternatives([FromBody] CreateAppointmentDto appointment)
        {
            var result = await _agendaService.SuggestAlternativeSlotsAsync(appointment);
            return Ok(result);
        }

        [HttpPost("validate")]
        public async Task<ActionResult> ValidateAppointment([FromBody] CreateAppointmentDto appointment)
        {
            var isValid = await _agendaService.ValidateAppointmentAsync(appointment);
            return Ok(new { isValid });
        }

        // Recurring Appointments
        [HttpPost("recurring")]
        public async Task<ActionResult> CreateRecurringAppointments(
            [FromBody] object request) // Seria CreateRecurringAppointmentRequest
        {
            // Parsing seria feito aqui
            var baseAppointment = new CreateAppointmentDto(); // Parse do request
            var pattern = new RecurringAppointmentPattern(); // Parse do request
            
            var result = await _agendaService.CreateRecurringAppointmentsAsync(baseAppointment, pattern);
            return Ok(result);
        }

        [HttpGet("recurring/{parentId}")]
        public async Task<ActionResult> GetRecurringSeries(int parentId)
        {
            var result = await _agendaService.GetRecurringSeriesAsync(parentId);
            return Ok(result);
        }

        [HttpPut("recurring/{parentId}")]
        public async Task<ActionResult> UpdateRecurringSeries(
            int parentId, 
            [FromBody] CreateAppointmentDto updates,
            [FromQuery] bool updateFutureOnly = false)
        {
            var result = await _agendaService.UpdateRecurringSeriesAsync(parentId, updates, updateFutureOnly);
            return Ok(new { success = result });
        }

        // Status Management
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(
            int id, 
            [FromBody] object request) // { status: string, notes?: string }
        {
            // Parse do request seria feito aqui
            var result = await _agendaService.UpdateAppointmentStatusAsync(id, "completed", null);
            return Ok(new { success = result });
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult> GetAppointmentsByStatus(string status, [FromQuery] DateTime? date = null)
        {
            var result = await _agendaService.GetAppointmentsByStatusAsync(status, date);
            return Ok(result);
        }

        [HttpPatch("bulk-status")]
        public async Task<ActionResult> BulkUpdateStatus([FromBody] object request)
        {
            // Parse seria: { appointmentIds: int[], status: string }
            var result = await _agendaService.BulkUpdateStatusAsync(new[] { 1, 2, 3 }, "completed");
            return Ok(new { success = result });
        }

        // Reports
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
            var result = await _agendaService.GetAppointmentReportsAsync(
                startDate, endDate, status, professionalId, clientId, convenio, sala, page, limit);
            return Ok(result);
        }

        // Statistics & Analytics
        [HttpGet("statistics")]
        public async Task<ActionResult> GetStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _agendaService.GetAppointmentStatisticsAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("dashboard-metrics")]
        public async Task<ActionResult> GetDashboardMetrics([FromQuery] DateTime? date = null)
        {
            var result = await _agendaService.GetDashboardMetricsAsync(date);
            return Ok(result);
        }

        [HttpGet("utilization-report")]
        public async Task<ActionResult> GetUtilizationReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _agendaService.GetUtilizationReportAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("performance-analytics")]
        public async Task<ActionResult> GetPerformanceAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _agendaService.GetPerformanceAnalyticsAsync(startDate, endDate);
            return Ok(result);
        }

        // Reminders
        [HttpGet("reminders/pending")]
        public async Task<ActionResult> GetPendingReminders()
        {
            var result = await _agendaService.GetPendingRemindersAsync();
            return Ok(result);
        }

        [HttpPost("{id}/reminder")]
        public async Task<ActionResult> CreateReminder(
            int id,
            [FromBody] object request) // { type: string, scheduledFor?: DateTime }
        {
            var result = await _agendaService.CreateReminderAsync(id, "sms", null);
            return Ok(new { success = result });
        }

        [HttpPost("reminders/send")]
        public async Task<ActionResult> SendReminders()
        {
            var result = await _agendaService.SendRemindersAsync();
            return Ok(new { success = result });
        }

        [HttpGet("reminder-settings")]
        public async Task<ActionResult> GetReminderSettings()
        {
            var result = await _agendaService.GetReminderSettingsAsync();
            return Ok(result);
        }

        // Working Hours
        [HttpGet("staff/{staffId}/working-hours")]
        public async Task<ActionResult> GetStaffWorkingHours(int staffId)
        {
            var result = await _agendaService.GetStaffWorkingHoursAsync(staffId);
            return Ok(result);
        }

        [HttpPut("staff/{staffId}/working-hours")]
        public async Task<ActionResult> UpdateWorkingHours(int staffId, [FromBody] List<WorkingHours> workingHours)
        {
            var result = await _agendaService.UpdateWorkingHoursAsync(staffId, workingHours);
            return Ok(new { success = result });
        }

        [HttpGet("staff/{staffId}/schedule")]
        public async Task<ActionResult> GetStaffSchedule(
            int staffId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _agendaService.GetStaffScheduleAsync(staffId, startDate, endDate);
            return Ok(result);
        }

        // Room Management
        [HttpGet("rooms/available")]
        public async Task<ActionResult> GetAvailableRooms(
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime)
        {
            var result = await _agendaService.GetAvailableRoomsAsync(startTime, endTime);
            return Ok(result);
        }

        [HttpGet("rooms/utilization/{date}")]
        public async Task<ActionResult> GetRoomUtilization(DateTime date)
        {
            var result = await _agendaService.GetRoomUtilizationAsync(date);
            return Ok(result);
        }

        [HttpGet("rooms/{room}/schedule/{date}")]
        public async Task<ActionResult> GetRoomSchedule(string room, DateTime date)
        {
            var result = await _agendaService.GetRoomScheduleAsync(room, date);
            return Ok(result);
        }

        // Conflicts
        [HttpPost("check-conflicts")]
        public async Task<ActionResult> CheckConflicts([FromBody] CreateAppointmentDto appointment)
        {
            var result = await _agendaService.CheckConflictsAsync(appointment);
            return Ok(result);
        }

        [HttpPost("{id}/resolve-conflicts")]
        public async Task<ActionResult> ResolveConflicts(int id)
        {
            var result = await _agendaService.ResolveSchedulingConflictsAsync(id);
            return Ok(result);
        }

        // Export & Import
        [HttpGet("export")]
        public async Task<ActionResult> ExportSchedule(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string format = "pdf")
        {
            var result = await _agendaService.ExportScheduleAsync(startDate, endDate, format);
            return Ok(result);
        }

        [HttpPost("import")]
        public async Task<ActionResult> ImportAppointments([FromBody] object importData)
        {
            var result = await _agendaService.ImportAppointmentsAsync(importData);
            return Ok(result);
        }
    }
}