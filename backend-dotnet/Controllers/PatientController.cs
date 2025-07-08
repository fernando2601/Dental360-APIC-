using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using System.Security.Claims;
using DentalSpa.Application.DTOs;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientResponse>>> GetAll()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientResponse>> GetById(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null)
                return NotFound();
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<PatientResponse>> Create([FromBody] PatientCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _patientService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), null, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PatientResponse>> Update(int id, [FromBody] PatientCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _patientService.UpdateAsync(id, request);
            if (response == null)
                return NotFound();
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PatientResponse>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term is required");

            var patients = await _patientService.SearchAsync(term);
            return Ok(patients);
        }

        [HttpGet("cpf/{cpf}")]
        public async Task<ActionResult<PatientResponse>> GetPatientByCPF(string cpf)
        {
            var patient = await _patientService.GetPatientByCPFAsync(cpf);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<PatientResponse>> GetPatientByEmail(string email)
        {
            var patient = await _patientService.GetPatientByEmailAsync(email);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        [HttpGet("analytics")]
        public async Task<ActionResult> GetPatientAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var analytics = await _patientService.GetPatientAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }

        [HttpGet("{id}/metrics")]
        public async Task<ActionResult> GetPatientMetrics(int id)
        {
            var metrics = await _patientService.GetPatientMetricsAsync(id);
            return Ok(metrics);
        }

        [HttpGet("segmentation")]
        public async Task<ActionResult> GetPatientSegmentation()
        {
            var segmentation = await _patientService.GetPatientSegmentationAsync();
            return Ok(segmentation);
        }

        [HttpGet("distribution/age")]
        public async Task<ActionResult> GetAgeDistribution()
        {
            var distribution = await _patientService.GetAgeDistributionAsync();
            return Ok(distribution);
        }

        [HttpGet("distribution/gender")]
        public async Task<ActionResult> GetGenderDistribution()
        {
            var distribution = await _patientService.GetGenderDistributionAsync();
            return Ok(distribution);
        }

        [HttpGet("distribution/location")]
        public async Task<ActionResult> GetLocationDistribution()
        {
            var distribution = await _patientService.GetLocationDistributionAsync();
            return Ok(distribution);
        }

        [HttpGet("{id}/report")]
        public async Task<ActionResult> GetPatientReport(
            int id,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var report = await _patientService.GetPatientReportAsync(id, startDate, endDate);
            return Ok(report);
        }

        [HttpGet("dashboard-metrics")]
        public async Task<ActionResult> GetDashboardMetrics()
        {
            var metrics = await _patientService.GetDashboardMetricsAsync();
            return Ok(metrics);
        }

        [HttpGet("growth")]
        public async Task<ActionResult> GetPatientGrowth([FromQuery] int months = 12)
        {
            var growth = await _patientService.GetPatientGrowthAsync(months);
            return Ok(growth);
        }

        [HttpGet("retention")]
        public async Task<ActionResult> GetPatientRetention()
        {
            var retention = await _patientService.GetPatientRetentionAsync();
            return Ok(retention);
        }
    }
}