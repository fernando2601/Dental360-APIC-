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

        public async Task<object> GetPatientAnalyticsAsync(DateTime? startDate, DateTime? endDate)
        {
            var patients = await _patientRepository.GetAllAsync();
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;
            
            var filteredPatients = patients.Where(p => p.CreatedAt >= start && p.CreatedAt <= end);
            var totalPatients = filteredPatients.Count();
            var activePatients = filteredPatients.Count(p => p.IsActive);
            var newPatientsThisMonth = filteredPatients.Count(p => p.CreatedAt.Month == DateTime.Now.Month && p.CreatedAt.Year == DateTime.Now.Year);

            return new
            {
                TotalPatients = totalPatients,
                ActivePatients = activePatients,
                NewPatientsThisMonth = newPatientsThisMonth,
                InactivePatients = totalPatients - activePatients,
                Period = new { Start = start, End = end }
            };
        }

        public async Task<object> GetPatientMetricsAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return new { Error = "Patient not found" };

            var patients = await _patientRepository.GetAllAsync();
            var totalPatients = patients.Count();
            var activePatients = patients.Count(p => p.IsActive);
            var averageAge = patients.Where(p => p.BirthDate.HasValue).Average(p => DateTime.Now.Year - p.BirthDate.Value.Year);

            return new
            {
                Patient = new
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    Age = patient.BirthDate.HasValue ? DateTime.Now.Year - patient.BirthDate.Value.Year : 0,
                    IsActive = patient.IsActive,
                    CreatedAt = patient.CreatedAt
                },
                Metrics = new
                {
                    TotalPatients = totalPatients,
                    ActivePatients = activePatients,
                    AverageAge = Math.Round(averageAge, 1),
                    RetentionRate = totalPatients > 0 ? (double)activePatients / totalPatients * 100 : 0
                }
            };
        }

        public async Task<object> GetPatientSegmentationAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var segments = patients.GroupBy(p => p.Segment ?? "Não Definido")
                                 .Select(g => new { Segment = g.Key, Count = g.Count() })
                                 .OrderByDescending(x => x.Count);

            return segments;
        }

        public async Task<object> GetAgeDistributionAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var ageGroups = patients.Where(p => p.BirthDate.HasValue)
                                  .GroupBy(p => GetAgeGroup(DateTime.Now.Year - p.BirthDate.Value.Year))
                                  .Select(g => new { AgeGroup = g.Key, Count = g.Count() })
                                  .OrderBy(x => x.AgeGroup);

            return ageGroups;
        }

        public async Task<object> GetGenderDistributionAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var genderDistribution = patients.GroupBy(p => p.Gender ?? "Não Informado")
                                           .Select(g => new { Gender = g.Key, Count = g.Count() })
                                           .OrderByDescending(x => x.Count);

            return genderDistribution;
        }

        public async Task<object> GetLocationDistributionAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var locationDistribution = patients.GroupBy(p => p.City ?? "Não Informado")
                                             .Select(g => new { City = g.Key, Count = g.Count() })
                                             .OrderByDescending(x => x.Count)
                                             .Take(10);

            return locationDistribution;
        }

        public async Task<object> GetPatientReportAsync(int id, DateTime? startDate, DateTime? endDate)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return new { Error = "Patient not found" };

            var patients = await _patientRepository.GetAllAsync();
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;
            
            var report = new
            {
                Patient = new
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    Email = patient.Email,
                    Phone = patient.Phone,
                    Age = patient.BirthDate.HasValue ? DateTime.Now.Year - patient.BirthDate.Value.Year : 0,
                    City = patient.City,
                    State = patient.State,
                    IsActive = patient.IsActive,
                    CreatedAt = patient.CreatedAt
                },
                Period = new { Start = start, End = end },
                Statistics = new
                {
                    TotalPatients = patients.Count(),
                    ActivePatients = patients.Count(p => p.IsActive),
                    NewPatientsThisMonth = patients.Count(p => p.CreatedAt.Month == DateTime.Now.Month && p.CreatedAt.Year == DateTime.Now.Year),
                    NewPatientsThisYear = patients.Count(p => p.CreatedAt.Year == DateTime.Now.Year),
                    AverageAge = patients.Where(p => p.BirthDate.HasValue).Average(p => DateTime.Now.Year - p.BirthDate.Value.Year),
                    TopCities = patients.GroupBy(p => p.City ?? "Não Informado")
                                      .Select(g => new { City = g.Key, Count = g.Count() })
                                      .OrderByDescending(x => x.Count)
                                      .Take(5)
                }
            };

            return report;
        }

        public async Task<object> GetDashboardMetricsAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var metrics = new
            {
                TotalPatients = patients.Count(),
                ActivePatients = patients.Count(p => p.IsActive),
                NewPatientsToday = patients.Count(p => p.CreatedAt.Date == DateTime.Now.Date),
                NewPatientsThisWeek = patients.Count(p => p.CreatedAt >= DateTime.Now.AddDays(-7)),
                NewPatientsThisMonth = patients.Count(p => p.CreatedAt.Month == DateTime.Now.Month && p.CreatedAt.Year == DateTime.Now.Year)
            };

            return metrics;
        }

        public async Task<object> GetPatientGrowthAsync(int months)
        {
            var patients = await _patientRepository.GetAllAsync();
            var monthlyGrowth = patients.GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                                      .Select(g => new { 
                                          Month = $"{g.Key.Year}-{g.Key.Month:D2}", 
                                          Count = g.Count() 
                                      })
                                      .OrderByDescending(x => x.Month)
                                      .Take(months)
                                      .Reverse();

            return monthlyGrowth;
        }

        public async Task<object> GetPatientRetentionAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var totalPatients = patients.Count();
            var activePatients = patients.Count(p => p.IsActive);
            var retentionRate = totalPatients > 0 ? (double)activePatients / totalPatients * 100 : 0;

            return new
            {
                TotalPatients = totalPatients,
                ActivePatients = activePatients,
                RetentionRate = Math.Round(retentionRate, 2)
            };
        }

        private string GetAgeGroup(int age)
        {
            return age switch
            {
                < 18 => "0-17",
                < 30 => "18-29",
                < 40 => "30-39",
                < 50 => "40-49",
                < 60 => "50-59",
                < 70 => "60-69",
                _ => "70+"
            };
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