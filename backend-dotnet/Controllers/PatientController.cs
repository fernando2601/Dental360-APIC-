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
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            var result = patients.Select(p => new PatientDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Idade = p.Idade,
                CPF = p.CPF,
                RG = p.RG,
                EstadoNascimento = p.EstadoNascimento,
                DataNascimento = p.DataNascimento,
                Sexo = p.Sexo,
                Telefone = p.Telefone,
                Email = p.Email,
                Endereco = p.Endereco
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound();
            var result = new PatientDto
            {
                Id = patient.Id,
                Nome = patient.Nome,
                Idade = patient.Idade,
                CPF = patient.CPF,
                RG = patient.RG,
                EstadoNascimento = patient.EstadoNascimento,
                DataNascimento = patient.DataNascimento,
                Sexo = patient.Sexo,
                Telefone = patient.Telefone,
                Email = patient.Email,
                Endereco = patient.Endereco
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientDto patientDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var patient = await _patientService.CreatePatientAsync(patientDto, userId);
                var result = new PatientDto
                {
                    Id = patient.Id,
                    Nome = patient.Nome,
                    Idade = patient.Idade,
                    CPF = patient.CPF,
                    RG = patient.RG,
                    EstadoNascimento = patient.EstadoNascimento,
                    DataNascimento = patient.DataNascimento,
                    Sexo = patient.Sexo,
                    Telefone = patient.Telefone,
                    Email = patient.Email,
                    Endereco = patient.Endereco
                };
                return CreatedAtAction(nameof(GetPatient), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PatientDto>> UpdatePatient(int id, [FromBody] UpdatePatientDto patientDto)
        {
            try
            {
                var patient = await _patientService.UpdatePatientAsync(id, patientDto);
                if (patient == null)
                    return NotFound();
                var result = new PatientDto
                {
                    Id = patient.Id,
                    Nome = patient.Nome,
                    Idade = patient.Idade,
                    CPF = patient.CPF,
                    RG = patient.RG,
                    EstadoNascimento = patient.EstadoNascimento,
                    DataNascimento = patient.DataNascimento,
                    Sexo = patient.Sexo,
                    Telefone = patient.Telefone,
                    Email = patient.Email,
                    Endereco = patient.Endereco
                };
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var result = await _patientService.DeletePatientAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<ActionResult> GetPatientsWithFilters([FromQuery] PatientFilters filters)
        {
            var result = await _patientService.GetPatientsWithFiltersAsync(filters);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Patient>>> SearchPatients([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term is required");

            var patients = await _patientService.SearchPatientsAsync(term);
            return Ok(patients);
        }

        [HttpGet("cpf/{cpf}")]
        public async Task<ActionResult<Patient>> GetPatientByCPF(string cpf)
        {
            var patient = await _patientService.GetPatientByCPFAsync(cpf);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<Patient>> GetPatientByEmail(string email)
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

        [HttpGet("with-metrics")]
        public async Task<ActionResult> GetPatientsWithMetrics([FromQuery] PatientFilters filters)
        {
            var result = await _patientService.GetPatientsWithMetricsAsync(filters);
            return Ok(result);
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

        [HttpPost("export")]
        public async Task<ActionResult> ExportPatients([FromBody] PatientExportRequest request)
        {
            var exportData = await _patientService.ExportPatientsAsync(request);
            return Ok(exportData);
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