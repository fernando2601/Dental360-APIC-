using ClinicApi.Models;

namespace ClinicApi.Services
{
    public interface IPatientService
    {
        // CRUD Básico
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient> CreatePatientAsync(CreatePatientDto patient);
        Task<Patient?> UpdatePatientAsync(int id, CreatePatientDto patient);
        Task<bool> DeletePatientAsync(int id);

        // Patient Profile & Management
        Task<PatientProfile> GetPatientProfileAsync(int id);
        Task<object> SearchPatientsAsync(string query);
        Task<object> GetPatientsWithFiltersAsync(
            string? name = null, string? email = null, string? phone = null, string? cpf = null,
            string? city = null, string? healthPlan = null, string? status = null,
            DateTime? birthStart = null, DateTime? birthEnd = null,
            int page = 1, int limit = 25);

        // Medical History Management
        Task<object> GetPatientMedicalHistoryAsync(int patientId);
        Task<PatientMedicalHistory> AddMedicalHistoryAsync(PatientMedicalHistory history);
        Task<PatientMedicalHistory?> UpdateMedicalHistoryAsync(int id, PatientMedicalHistory history);
        Task<bool> DeleteMedicalHistoryAsync(int id);

        // Document Management
        Task<object> GetPatientDocumentsAsync(int patientId);
        Task<PatientDocument> UploadDocumentAsync(int patientId, object documentData);
        Task<bool> DeleteDocumentAsync(int id);
        Task<object> DownloadDocumentAsync(int id);

        // Notes Management
        Task<object> GetPatientNotesAsync(int patientId);
        Task<PatientNote> AddPatientNoteAsync(PatientNote note);
        Task<PatientNote?> UpdatePatientNoteAsync(int id, PatientNote note);
        Task<bool> DeletePatientNoteAsync(int id);

        // Analytics & Reports
        Task<PatientAnalytics> GetPatientAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<PatientDashboardMetrics> GetDashboardMetricsAsync();
        Task<object> GetPatientSegmentsAsync();
        Task<object> GetBirthdayRemindersAsync(DateTime? date = null);

        // Communication Management
        Task<object> GetPatientCommunicationsAsync(int patientId);
        Task<object> SendCommunicationAsync(int patientId, object communicationData);
        Task<object> GetCommunicationTemplatesAsync();

        // Bulk Operations
        Task<object> BulkUpdatePatientsAsync(PatientBulkAction action);
        Task<object> ExportPatientsAsync(PatientExportRequest request);
        Task<object> ImportPatientsAsync(object importData);

        // Advanced Analytics
        Task<object> GetRetentionAnalysisAsync(DateTime startDate, DateTime endDate);
        Task<object> GetPatientValueAnalysisAsync();
        Task<object> GetGeographicDistributionAsync();
        Task<object> GetPatientLifecycleAnalysisAsync();

        // Predictive Analytics
        Task<object> GetChurnPredictionAsync();
        Task<object> GetPatientRecommendationsAsync(int patientId);
        Task<object> GetSegmentationInsightsAsync();

        // Data Quality & Validation
        Task<object> ValidatePatientDataAsync(int patientId);
        Task<object> GetDuplicatePatientsAsync();
        Task<object> CleanupPatientDataAsync();
    }
}