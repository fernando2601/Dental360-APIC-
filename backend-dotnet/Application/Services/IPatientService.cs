using ClinicApi.Models;

namespace ClinicApi.Services
{
    public interface IPatientService
    {
        // CRUD Básico
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient> CreatePatientAsync(CreatePatientDto patientDto, int createdBy);
        Task<Patient?> UpdatePatientAsync(int id, UpdatePatientDto patientDto);
        Task<bool> DeletePatientAsync(int id);

        // Pesquisa e Filtros
        Task<object> GetPatientsWithFiltersAsync(PatientFilters filters);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
        Task<Patient?> GetPatientByCPFAsync(string cpf);
        Task<Patient?> GetPatientByEmailAsync(string email);

        // Analytics e Métricas
        Task<PatientAnalytics> GetPatientAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<PatientMetrics> GetPatientMetricsAsync(int patientId);
        Task<object> GetPatientsWithMetricsAsync(PatientFilters filters);

        // Segmentação e Distribuições
        Task<IEnumerable<PatientSegmentation>> GetPatientSegmentationAsync();
        Task<IEnumerable<AgeDistribution>> GetAgeDistributionAsync();
        Task<IEnumerable<GenderDistribution>> GetGenderDistributionAsync();
        Task<IEnumerable<LocationDistribution>> GetLocationDistributionAsync();

        // Relatórios e Exports
        Task<PatientReport> GetPatientReportAsync(int patientId, DateTime? startDate = null, DateTime? endDate = null);
        Task<object> ExportPatientsAsync(PatientExportRequest request);

        // Dashboard e Estatísticas
        Task<object> GetDashboardMetricsAsync();
        Task<object> GetPatientGrowthAsync(int months = 12);
        Task<object> GetPatientRetentionAsync();

        // Validações
        Task<bool> ValidatePatientDataAsync(CreatePatientDto patientDto);
        Task<bool> ValidatePatientUpdateAsync(int id, UpdatePatientDto patientDto);
    }
}