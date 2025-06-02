using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // CRUD Básico
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> CreatePatient(CreatePatientDto patientDto)
        {
            try
            {
                var patient = await _patientService.CreatePatientAsync(patientDto);
                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Client>> UpdatePatient(int id, CreateClientDto patientDto)
        {
            try
            {
                var patient = await _patientService.UpdatePatientAsync(id, patientDto);
                if (patient == null)
                    return NotFound();

                return Ok(patient);
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

        // Patient Profile & Management
        [HttpGet("{id}/profile")]
        public async Task<ActionResult> GetPatientProfile(int id)
        {
            try
            {
                var profile = await _patientService.GetPatientProfileAsync(id);
                return Ok(profile);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchPatients([FromQuery] string query)
        {
            var result = await _patientService.SearchPatientsAsync(query);
            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<ActionResult> GetPatientsWithFilters(
            [FromQuery] string? name = null,
            [FromQuery] string? email = null,
            [FromQuery] string? phone = null,
            [FromQuery] string? cpf = null,
            [FromQuery] string? city = null,
            [FromQuery] string? healthPlan = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? birthStart = null,
            [FromQuery] DateTime? birthEnd = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25)
        {
            var result = await _patientService.GetPatientsWithFiltersAsync(
                name, email, phone, cpf, city, healthPlan, status, 
                birthStart, birthEnd, page, limit);
            return Ok(result);
        }

        // Medical History
        [HttpGet("{id}/medical-history")]
        public async Task<ActionResult> GetPatientMedicalHistory(int id)
        {
            var result = await _patientService.GetPatientMedicalHistoryAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/medical-history")]
        public async Task<ActionResult> AddMedicalHistory(int id, [FromBody] PatientMedicalHistory history)
        {
            try
            {
                history.PatientId = id;
                var result = await _patientService.AddMedicalHistoryAsync(history);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("medical-history/{historyId}")]
        public async Task<ActionResult> UpdateMedicalHistory(int historyId, [FromBody] PatientMedicalHistory history)
        {
            var result = await _patientService.UpdateMedicalHistoryAsync(historyId, history);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("medical-history/{historyId}")]
        public async Task<ActionResult> DeleteMedicalHistory(int historyId)
        {
            var result = await _patientService.DeleteMedicalHistoryAsync(historyId);
            return Ok(new { success = result });
        }

        // Documents
        [HttpGet("{id}/documents")]
        public async Task<ActionResult> GetPatientDocuments(int id)
        {
            var result = await _patientService.GetPatientDocumentsAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/documents")]
        public async Task<ActionResult> UploadDocument(int id, [FromBody] object documentData)
        {
            var result = await _patientService.UploadDocumentAsync(id, documentData);
            return Ok(result);
        }

        [HttpDelete("documents/{documentId}")]
        public async Task<ActionResult> DeleteDocument(int documentId)
        {
            var result = await _patientService.DeleteDocumentAsync(documentId);
            return Ok(new { success = result });
        }

        [HttpGet("documents/{documentId}/download")]
        public async Task<ActionResult> DownloadDocument(int documentId)
        {
            var result = await _patientService.DownloadDocumentAsync(documentId);
            return Ok(result);
        }

        // Notes
        [HttpGet("{id}/notes")]
        public async Task<ActionResult> GetPatientNotes(int id)
        {
            var result = await _patientService.GetPatientNotesAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/notes")]
        public async Task<ActionResult> AddPatientNote(int id, [FromBody] PatientNote note)
        {
            try
            {
                note.PatientId = id;
                var result = await _patientService.AddPatientNoteAsync(note);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("notes/{noteId}")]
        public async Task<ActionResult> UpdatePatientNote(int noteId, [FromBody] PatientNote note)
        {
            var result = await _patientService.UpdatePatientNoteAsync(noteId, note);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("notes/{noteId}")]
        public async Task<ActionResult> DeletePatientNote(int noteId)
        {
            var result = await _patientService.DeletePatientNoteAsync(noteId);
            return Ok(new { success = result });
        }

        // Analytics & Reports
        [HttpGet("analytics")]
        public async Task<ActionResult> GetPatientAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _patientService.GetPatientAnalyticsAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("dashboard-metrics")]
        public async Task<ActionResult> GetDashboardMetrics()
        {
            var result = await _patientService.GetDashboardMetricsAsync();
            return Ok(result);
        }

        [HttpGet("segments")]
        public async Task<ActionResult> GetPatientSegments()
        {
            var result = await _patientService.GetPatientSegmentsAsync();
            return Ok(result);
        }

        [HttpGet("birthdays")]
        public async Task<ActionResult> GetBirthdayReminders([FromQuery] DateTime? date = null)
        {
            var result = await _patientService.GetBirthdayRemindersAsync(date);
            return Ok(result);
        }

        // Communications
        [HttpGet("{id}/communications")]
        public async Task<ActionResult> GetPatientCommunications(int id)
        {
            var result = await _patientService.GetPatientCommunicationsAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/communications")]
        public async Task<ActionResult> SendCommunication(int id, [FromBody] object communicationData)
        {
            var result = await _patientService.SendCommunicationAsync(id, communicationData);
            return Ok(result);
        }

        [HttpGet("communication-templates")]
        public async Task<ActionResult> GetCommunicationTemplates()
        {
            var result = await _patientService.GetCommunicationTemplatesAsync();
            return Ok(result);
        }

        // Bulk Operations
        [HttpPost("bulk-update")]
        public async Task<ActionResult> BulkUpdatePatients([FromBody] PatientBulkAction action)
        {
            var result = await _patientService.BulkUpdatePatientsAsync(action);
            return Ok(result);
        }

        [HttpPost("export")]
        public async Task<ActionResult> ExportPatients([FromBody] PatientExportRequest request)
        {
            var result = await _patientService.ExportPatientsAsync(request);
            return Ok(result);
        }

        [HttpPost("import")]
        public async Task<ActionResult> ImportPatients([FromBody] object importData)
        {
            var result = await _patientService.ImportPatientsAsync(importData);
            return Ok(result);
        }

        // Advanced Analytics
        [HttpGet("retention-analysis")]
        public async Task<ActionResult> GetRetentionAnalysis(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _patientService.GetRetentionAnalysisAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("value-analysis")]
        public async Task<ActionResult> GetPatientValueAnalysis()
        {
            var result = await _patientService.GetPatientValueAnalysisAsync();
            return Ok(result);
        }

        [HttpGet("geographic-distribution")]
        public async Task<ActionResult> GetGeographicDistribution()
        {
            var result = await _patientService.GetGeographicDistributionAsync();
            return Ok(result);
        }

        [HttpGet("lifecycle-analysis")]
        public async Task<ActionResult> GetPatientLifecycleAnalysis()
        {
            var result = await _patientService.GetPatientLifecycleAnalysisAsync();
            return Ok(result);
        }

        // Predictive Analytics
        [HttpGet("churn-prediction")]
        public async Task<ActionResult> GetChurnPrediction()
        {
            var result = await _patientService.GetChurnPredictionAsync();
            return Ok(result);
        }

        [HttpGet("{id}/recommendations")]
        public async Task<ActionResult> GetPatientRecommendations(int id)
        {
            var result = await _patientService.GetPatientRecommendationsAsync(id);
            return Ok(result);
        }

        [HttpGet("segmentation-insights")]
        public async Task<ActionResult> GetSegmentationInsights()
        {
            var result = await _patientService.GetSegmentationInsightsAsync();
            return Ok(result);
        }

        // Data Quality & Validation
        [HttpGet("{id}/validate")]
        public async Task<ActionResult> ValidatePatientData(int id)
        {
            var result = await _patientService.ValidatePatientDataAsync(id);
            return Ok(result);
        }

        [HttpGet("duplicates")]
        public async Task<ActionResult> GetDuplicatePatients()
        {
            var result = await _patientService.GetDuplicatePatientsAsync();
            return Ok(result);
        }

        [HttpPost("cleanup")]
        public async Task<ActionResult> CleanupPatientData()
        {
            var result = await _patientService.CleanupPatientDataAsync();
            return Ok(result);
        }
    }
}