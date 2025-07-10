using System.Data;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IDbConnection _connection;
        public PatientRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        // Implemente os métodos usando _connection
        public Task<IEnumerable<Patient>> GetAllAsync() => throw new System.NotImplementedException();
        public Task<Patient?> GetByIdAsync(int id) => throw new System.NotImplementedException();
        public Task<Patient> CreateAsync(Patient patient) => throw new System.NotImplementedException();
        public Task<Patient?> UpdateAsync(int id, Patient patient) => throw new System.NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new System.NotImplementedException();
        public Task<IEnumerable<Patient>> GetAllPatientsAsync() => throw new System.NotImplementedException();
        public Task<Patient?> GetPatientByIdAsync(int id) => throw new System.NotImplementedException();
        public Task<object> GetPatientAnalyticsAsync() => throw new System.NotImplementedException();
        public Task<object> GetAgeDistributionAsync() => throw new System.NotImplementedException();
        public Task<object> GetGenderDistributionAsync() => throw new System.NotImplementedException();
        public Task<object> GetLocationDistributionAsync() => throw new System.NotImplementedException();
        public Task<object> GetMonthlyRegistrationsAsync() => throw new System.NotImplementedException();
        public Task<object> GetPatientMetricsAsync() => throw new System.NotImplementedException();
        public Task<object> GetPatientSegmentationAsync() => throw new System.NotImplementedException();
        public Task<object> GetPatientReportAsync() => throw new System.NotImplementedException();
        public Task<object> GetPatientAppointmentHistoryAsync(int patientId) => throw new System.NotImplementedException();
        public Task<object> GetPatientPaymentHistoryAsync(int patientId) => throw new System.NotImplementedException();
        public Task<int> GetTotalPatientsAsync() => throw new System.NotImplementedException();
        public Task<int> GetNewPatientsThisMonthAsync() => throw new System.NotImplementedException();
        public Task<int> GetActivePatientsAsync() => throw new System.NotImplementedException();
        public Task<object> GetPatientGrowthAsync() => throw new System.NotImplementedException();
        public Task<object> GetPatientRetentionAsync() => throw new System.NotImplementedException();
        public Task<Patient?> GetPatientByCPFAsync(string cpf) => throw new System.NotImplementedException();
        public Task<Patient?> GetPatientByEmailAsync(string email) => throw new System.NotImplementedException();
    }
} 