using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _patientRepository.GetAllAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _patientRepository.GetByIdAsync(id);
        }

        public async Task<Patient> CreateAsync(Patient patient)
        {
            patient.CreatedAt = DateTime.UtcNow;
            patient.UpdatedAt = DateTime.UtcNow;
            return await _patientRepository.CreateAsync(patient);
        }

        public async Task<Patient?> UpdateAsync(int id, Patient patient)
        {
            patient.UpdatedAt = DateTime.UtcNow;
            return await _patientRepository.UpdateAsync(id, patient);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _patientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm)
        {
            return await _patientRepository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllPatientsAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetPatientByIdAsync(id);
        }

        public async Task<Patient?> GetPatientByCPFAsync(string cpf)
        {
            return await _patientRepository.GetPatientByCPFAsync(cpf);
        }

        public async Task<Patient?> GetPatientByEmailAsync(string email)
        {
            return await _patientRepository.GetPatientByEmailAsync(email);
        }

        public async Task<object> GetPatientAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var analytics = await _patientRepository.GetPatientAnalyticsAsync(startDate, endDate);
            
            // Buscar distribuições
            var ageDistribution = await _patientRepository.GetAgeDistributionAsync();
            var genderDistribution = await _patientRepository.GetGenderDistributionAsync();
            var locationDistribution = await _patientRepository.GetLocationDistributionAsync();
            var monthlyRegistrations = await _patientRepository.GetMonthlyRegistrationsAsync();

            return new
            {
                analytics,
                ageDistribution,
                genderDistribution,
                locationDistribution,
                monthlyRegistrations
            };
        }

        public async Task<object> GetPatientMetricsAsync(int patientId)
        {
            return await _patientRepository.GetPatientMetricsAsync(patientId);
        }

        public async Task<object> GetPatientSegmentationAsync()
        {
            return await _patientRepository.GetPatientSegmentationAsync();
        }

        public async Task<object> GetAgeDistributionAsync()
        {
            return await _patientRepository.GetAgeDistributionAsync();
        }

        public async Task<object> GetGenderDistributionAsync()
        {
            return await _patientRepository.GetGenderDistributionAsync();
        }

        public async Task<object> GetLocationDistributionAsync()
        {
            return await _patientRepository.GetLocationDistributionAsync();
        }

        public async Task<object> GetPatientReportAsync(int patientId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var report = await _patientRepository.GetPatientReportAsync(patientId, startDate, endDate);
            
            // Buscar dados adicionais
            var appointments = await _patientRepository.GetPatientAppointmentHistoryAsync(patientId);
            var payments = await _patientRepository.GetPatientPaymentHistoryAsync(patientId);
            var metrics = await _patientRepository.GetPatientMetricsAsync(patientId);
            
            return new
            {
                report,
                appointments,
                payments,
                metrics
            };
        }

        public async Task<object> GetDashboardMetricsAsync()
        {
            var totalPatients = await _patientRepository.GetTotalPatientsAsync();
            var newPatientsThisMonth = await _patientRepository.GetNewPatientsThisMonthAsync();
            var activePatients = await _patientRepository.GetActivePatientsAsync();
            var patientGrowth = await _patientRepository.GetPatientGrowthAsync();

            return new
            {
                totalPatients,
                newPatientsThisMonth,
                activePatients,
                patientGrowth
            };
        }

        public async Task<object> GetPatientGrowthAsync(int months = 12)
        {
            return await _patientRepository.GetPatientGrowthAsync(months);
        }

        public async Task<object> GetPatientRetentionAsync()
        {
            return await _patientRepository.GetPatientRetentionAsync();
        }

        public async Task<bool> ValidatePatientDataAsync(Patient patient)
        {
            if (string.IsNullOrWhiteSpace(patient.Nome))
                return false;

            if (string.IsNullOrWhiteSpace(patient.CPF))
                return false;

            if (string.IsNullOrWhiteSpace(patient.Telefone))
                return false;

            // Verificar se CPF já existe
            var existingPatient = await _patientRepository.GetPatientByCPFAsync(patient.CPF);
            if (existingPatient != null)
                return false;

            return true;
        }

        public async Task<bool> ValidatePatientUpdateAsync(Patient patient)
        {
            if (string.IsNullOrWhiteSpace(patient.Nome))
                return false;

            if (string.IsNullOrWhiteSpace(patient.CPF))
                return false;

            if (string.IsNullOrWhiteSpace(patient.Telefone))
                return false;

            // Verificar se CPF já existe para outro paciente
            var existingPatient = await _patientRepository.GetPatientByCPFAsync(patient.CPF);
            if (existingPatient != null && existingPatient.Id != patient.Id)
                return false;

            return true;
        }
    }
}