using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IPatientRepository
    {
        // CRUD Básico
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient> CreatePatientAsync(CreatePatientDto patientDto, int createdBy);
        Task<Patient?> UpdatePatientAsync(int id, UpdatePatientDto patientDto);
        Task<bool> DeletePatientAsync(int id);

        // Pesquisa e Filtros
        Task<IEnumerable<Patient>> GetPatientsWithFiltersAsync(PatientFilters filters);
        Task<int> GetPatientsCountAsync(PatientFilters filters);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
        Task<Patient?> GetPatientByCPFAsync(string cpf);
        Task<Patient?> GetPatientByEmailAsync(string email);
        Task<Patient?> GetPatientByPhoneAsync(string phone);

        // Analytics e Métricas
        Task<PatientAnalytics> GetPatientAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<PatientMetrics> GetPatientMetricsAsync(int patientId);
        Task<IEnumerable<PatientWithMetrics>> GetPatientsWithMetricsAsync(PatientFilters filters);

        // Segmentação
        Task<IEnumerable<PatientSegmentation>> GetPatientSegmentationAsync();
        Task<IEnumerable<Patient>> GetPatientsBySegmentAsync(string segment);
        Task<bool> UpdatePatientSegmentAsync(int patientId, string segment);

        // Distribuições
        Task<IEnumerable<AgeDistribution>> GetAgeDistributionAsync();
        Task<IEnumerable<GenderDistribution>> GetGenderDistributionAsync();
        Task<IEnumerable<LocationDistribution>> GetLocationDistributionAsync();
        Task<IEnumerable<MonthlyRegistration>> GetMonthlyRegistrationsAsync(int months = 12);

        // Comunicação
        Task<IEnumerable<PatientCommunication>> GetPatientCommunicationsAsync(int patientId);
        Task<PatientCommunication> CreateCommunicationAsync(PatientCommunication communication);
        Task<bool> UpdateCommunicationStatusAsync(int communicationId, string status, DateTime? deliveredAt = null, DateTime? readAt = null);

        // Notas
        Task<IEnumerable<PatientNote>> GetPatientNotesAsync(int patientId);
        Task<PatientNote> CreatePatientNoteAsync(PatientNote note);
        Task<PatientNote?> UpdatePatientNoteAsync(int noteId, string title, string content, string priority);
        Task<bool> DeletePatientNoteAsync(int noteId);

        // Documentos
        Task<IEnumerable<PatientDocument>> GetPatientDocumentsAsync(int patientId);
        Task<PatientDocument> CreatePatientDocumentAsync(PatientDocument document);
        Task<bool> DeletePatientDocumentAsync(int documentId);

        // Operações em Lote
        Task<bool> BulkUpdatePatientsAsync(PatientBulkAction action, int updatedBy);
        Task<bool> BulkActivatePatientsAsync(int[] patientIds, int updatedBy);
        Task<bool> BulkDeactivatePatientsAsync(int[] patientIds, string reason, int updatedBy);

        // Relatórios
        Task<PatientReport> GetPatientReportAsync(int patientId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Patient>> GetPatientsForExportAsync(PatientExportRequest request);

        // Insights e Recomendações
        Task<IEnumerable<PatientInsight>> GetPatientInsightsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Patient>> GetInactivePatientsAsync(int daysSinceLastVisit = 90);
        Task<IEnumerable<Patient>> GetHighValuePatientsAsync(decimal minimumValue = 1000);
        Task<IEnumerable<Patient>> GetRiskPatientsAsync();

        // Estatísticas do Dashboard
        Task<object> GetDashboardMetricsAsync();
        Task<object> GetPatientGrowthAsync(int months = 12);
        Task<object> GetPatientRetentionAsync();

        // Histórico
        Task<IEnumerable<AppointmentSummary>> GetPatientAppointmentHistoryAsync(int patientId);
        Task<IEnumerable<PaymentSummary>> GetPatientPaymentHistoryAsync(int patientId);
        Task<bool> UpdateLastVisitAsync(int patientId, DateTime lastVisit);

        // Validações
        Task<bool> IsCPFExistsAsync(string cpf, int? excludePatientId = null);
        Task<bool> IsEmailExistsAsync(string email, int? excludePatientId = null);
        Task<bool> IsPhoneExistsAsync(string phone, int? excludePatientId = null);
    }
}