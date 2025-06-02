using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // Dashboard Principal
        [HttpGet("dashboard")]
        public async Task<ActionResult> GetAnalyticsDashboard(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var dashboard = await _analyticsService.GetAnalyticsDashboardAsync(startDate, endDate);
            return Ok(dashboard);
        }

        // Análises de Pacientes
        [HttpGet("patients")]
        public async Task<ActionResult> GetPatientAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var analytics = await _analyticsService.GetPatientAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }

        // Análises de Agendamentos
        [HttpGet("appointments")]
        public async Task<ActionResult> GetAppointmentAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var analytics = await _analyticsService.GetAppointmentAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }

        // Análises Financeiras
        [HttpGet("financial")]
        public async Task<ActionResult> GetFinancialAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var analytics = await _analyticsService.GetFinancialAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }

        // Análises de Performance
        [HttpGet("performance")]
        public async Task<ActionResult> GetPerformanceAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var analytics = await _analyticsService.GetPerformanceAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }

        // Relatórios Customizados
        [HttpPost("custom-report")]
        public async Task<ActionResult> GenerateCustomReport([FromBody] CustomReportDto reportDto)
        {
            var report = await _analyticsService.GenerateCustomReportAsync(reportDto);
            return Ok(report);
        }

        // Exportar Dados
        [HttpGet("export")]
        public async Task<ActionResult> ExportAnalytics(
            [FromQuery] string format = "excel",
            [FromQuery] string type = "complete",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var export = await _analyticsService.ExportAnalyticsAsync(format, type, startDate, endDate);
            return File(export.Content, export.ContentType, export.FileName);
        }
    }
}