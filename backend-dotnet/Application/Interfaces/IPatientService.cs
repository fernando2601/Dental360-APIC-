using DentalSpa.Domain.Entities;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientResponse>> GetAllAsync();
        Task<PatientResponse?> GetByIdAsync(int id);
        Task<PatientResponse> CreateAsync(PatientCreateRequest request);
        Task<PatientResponse?> UpdateAsync(int id, PatientCreateRequest request);
        Task<bool> DeleteAsync(int id);
        
        // Analytics methods
        Task<object> GetPatientAnalyticsAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetPatientMetricsAsync(int id);
        Task<object> GetPatientSegmentationAsync();
        Task<object> GetAgeDistributionAsync();
        Task<object> GetGenderDistributionAsync();
        Task<object> GetLocationDistributionAsync();
        Task<object> GetPatientReportAsync(int id, DateTime? startDate, DateTime? endDate);
        Task<object> GetDashboardMetricsAsync();
        Task<object> GetPatientGrowthAsync(int months);
        Task<object> GetPatientRetentionAsync();
        Task<PatientResponse?> GetPatientByCPFAsync(string cpf);
        Task<PatientResponse?> GetPatientByEmailAsync(string email);
    }
} 