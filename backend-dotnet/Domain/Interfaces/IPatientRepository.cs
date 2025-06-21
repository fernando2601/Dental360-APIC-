using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient> CreateAsync(Patient patient);
        Task<Patient?> UpdateAsync(int id, Patient patient);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
        
        // Analytics methods
        Task<object> GetPatientAnalyticsAsync();
        Task<object> GetAgeDistributionAsync();
        Task<object> GetGenderDistributionAsync();
        Task<object> GetLocationDistributionAsync();
        Task<object> GetMonthlyRegistrationsAsync();
        Task<object> GetPatientMetricsAsync();
        Task<object> GetPatientSegmentationAsync();
        Task<object> GetPatientReportAsync();
        Task<object> GetPatientAppointmentHistoryAsync(int patientId);
        Task<object> GetPatientPaymentHistoryAsync(int patientId);
        Task<int> GetTotalPatientsAsync();
        Task<int> GetNewPatientsThisMonthAsync();
        Task<int> GetActivePatientsAsync();
        Task<object> GetPatientGrowthAsync();
        Task<object> GetPatientRetentionAsync();
        Task<Patient?> GetPatientByCPFAsync(string cpf);
        Task<Patient?> GetPatientByEmailAsync(string email);
    }
} 