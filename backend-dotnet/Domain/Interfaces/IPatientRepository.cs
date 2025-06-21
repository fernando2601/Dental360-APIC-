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

        // Pesquisa
        Task<Patient?> GetPatientByCPFAsync(string cpf);
        Task<Patient?> GetPatientByEmailAsync(string email);
        Task<Patient?> GetPatientByPhoneAsync(string phone);

        // Dashboard e Métricas
        Task<object> GetDashboardMetricsAsync();
        Task<IEnumerable<Patient>> GetInactivePatientsAsync(int daysSinceLastVisit = 90);

        // Histórico
        Task<bool> UpdateLastVisitAsync(int patientId, DateTime lastVisit);

        // Validações
        Task<bool> IsCPFExistsAsync(string cpf, int? excludePatientId = null);
        Task<bool> IsEmailExistsAsync(string email, int? excludePatientId = null);
        Task<bool> IsPhoneExistsAsync(string phone, int? excludePatientId = null);
    }
} 