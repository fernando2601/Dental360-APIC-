using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IPatientRepository
    {
        // CRUD Básico
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient> CreateAsync(CreatePatientDto patient);
        Task<Patient?> UpdateAsync(int id, CreatePatientDto patient);
        Task<bool> DeleteAsync(int id);

        // Patient Profile & Details
        Task<PatientProfile> GetPatientProfileAsync(int id);
        Task<IEnumerable<PatientSearchResult>> SearchPatientsAsync(string query);
        Task<IEnumerable<PatientSearchResult>> GetPatientsWithFiltersAsync(
            string? name = null, string? email = null, string? phone = null, string? cpf = null,
            string? city = null, string? healthPlan = null, string? status = null,
            DateTime? birthStart = null, DateTime? birthEnd = null,
            int page = 1, int limit = 25);
        Task<int> GetPatientsCountAsync(
            string? name = null, string? email = null, string? phone = null, string? cpf = null,
            string? city = null, string? healthPlan = null, string? status = null,
            DateTime? birthStart = null, DateTime? birthEnd = null);

        // Medical History
        Task<IEnumerable<PatientMedicalHistory>> GetMedicalHistoryAsync(int patientId);
        Task<PatientMedicalHistory> AddMedicalHistoryAsync(PatientMedicalHistory history);
        Task<PatientMedicalHistory?> UpdateMedicalHistoryAsync(int id, PatientMedicalHistory history);
        Task<bool> DeleteMedicalHistoryAsync(int id);

        // Documents
        Task<IEnumerable<PatientDocument>> GetPatientDocumentsAsync(int patientId);
        Task<PatientDocument> AddDocumentAsync(PatientDocument document);
        Task<bool> DeleteDocumentAsync(int id);

        // Notes
        Task<IEnumerable<PatientNote>> GetPatientNotesAsync(int patientId);
        Task<PatientNote> AddNoteAsync(PatientNote note);
        Task<PatientNote?> UpdateNoteAsync(int id, PatientNote note);
        Task<bool> DeleteNoteAsync(int id);

        // Analytics & Reports
        Task<PatientAnalytics> GetPatientAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<PatientDashboardMetrics> GetDashboardMetricsAsync();
        Task<IEnumerable<PatientSegment>> GetPatientSegmentsAsync();
        Task<IEnumerable<PatientBirthday>> GetBirthdaysAsync(DateTime? date = null);

        // Communications
        Task<IEnumerable<PatientCommunication>> GetPatientCommunicationsAsync(int patientId);
        Task<PatientCommunication> AddCommunicationAsync(PatientCommunication communication);

        // Bulk Operations
        Task<bool> BulkUpdatePatientsAsync(PatientBulkAction action);
        Task<object> ExportPatientsAsync(PatientExportRequest request);

        // Statistics
        Task<object> GetRetentionAnalysisAsync(DateTime startDate, DateTime endDate);
        Task<object> GetPatientValueAnalysisAsync();
        Task<object> GetGeographicDistributionAsync();
    }
}