using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient> CreateAsync(Patient patient);
        Task<Patient?> UpdateAsync(int id, Patient patient);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
        
        // Analytics methods
        Task<object> GetPatientAnalyticsAsync();
        Task<object> GetPatientMetricsAsync();
        Task<object> GetPatientSegmentationAsync();
        Task<object> GetAgeDistributionAsync();
        Task<object> GetGenderDistributionAsync();
        Task<object> GetLocationDistributionAsync();
        Task<object> GetPatientReportAsync();
        Task<object> GetDashboardMetricsAsync();
        Task<object> GetPatientGrowthAsync();
        Task<object> GetPatientRetentionAsync();
        Task<Patient?> GetPatientByCPFAsync(string cpf);
        Task<Patient?> GetPatientByEmailAsync(string email);
    }
} 