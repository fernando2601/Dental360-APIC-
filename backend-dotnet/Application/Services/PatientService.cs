using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<PatientResponse>> GetAllAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            return patients.Select(p => MapToResponse(p));
        }

        public async Task<PatientResponse?> GetByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            return patient == null ? null : MapToResponse(patient);
        }

        public async Task<PatientResponse> CreateAsync(PatientCreateRequest request)
        {
            var patient = new Patient
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                ClinicId = request.ClinicId,
                BirthDate = request.BirthDate,
                Gender = request.Gender,
                Address = request.Address,
                MedicalHistory = request.MedicalHistory,
                Allergies = request.Allergies,
                EmergencyContact = request.EmergencyContact,
                EmergencyPhone = request.EmergencyPhone,
                IsActive = request.IsActive,
                Nome = request.Nome,
                Idade = request.Idade,
                CPF = request.CPF,
                RG = request.RG,
                EstadoNascimento = request.EstadoNascimento,
                DataNascimento = request.DataNascimento,
                Sexo = request.Sexo,
                Telefone = request.Telefone,
                Endereco = request.Endereco,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Segment = request.Segment
            };
            var created = await _patientRepository.CreateAsync(patient);
            return MapToResponse(created);
        }

        public async Task<PatientResponse?> UpdateAsync(int id, PatientCreateRequest request)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null) return null;
            patient.Name = request.Name;
            patient.Email = request.Email;
            patient.Phone = request.Phone;
            patient.ClinicId = request.ClinicId;
            patient.BirthDate = request.BirthDate;
            patient.Gender = request.Gender;
            patient.Address = request.Address;
            patient.MedicalHistory = request.MedicalHistory;
            patient.Allergies = request.Allergies;
            patient.EmergencyContact = request.EmergencyContact;
            patient.EmergencyPhone = request.EmergencyPhone;
            patient.IsActive = request.IsActive;
            patient.Nome = request.Nome;
            patient.Idade = request.Idade;
            patient.CPF = request.CPF;
            patient.RG = request.RG;
            patient.EstadoNascimento = request.EstadoNascimento;
            patient.DataNascimento = request.DataNascimento;
            patient.Sexo = request.Sexo;
            patient.Telefone = request.Telefone;
            patient.Endereco = request.Endereco;
            patient.City = request.City;
            patient.State = request.State;
            patient.ZipCode = request.ZipCode;
            patient.Segment = request.Segment;
            var updated = await _patientRepository.UpdateAsync(id, patient);
            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _patientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllPatientsAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetPatientByIdAsync(id);
        }

        public async Task<PatientResponse?> GetPatientByCPFAsync(string cpf)
        {
            var patient = await _patientRepository.GetPatientByCPFAsync(cpf);
            return patient == null ? null : MapToResponse(patient);
        }

        public async Task<PatientResponse?> GetPatientByEmailAsync(string email)
        {
            var patient = await _patientRepository.GetPatientByEmailAsync(email);
            return patient == null ? null : MapToResponse(patient);
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

        private PatientResponse MapToResponse(Patient p)
        {
            return new PatientResponse
            {
                Name = p.Name,
                Email = p.Email,
                Phone = p.Phone,
                BirthDate = p.BirthDate,
                Gender = p.Gender,
                Address = p.Address,
                MedicalHistory = p.MedicalHistory,
                Allergies = p.Allergies,
                EmergencyContact = p.EmergencyContact,
                EmergencyPhone = p.EmergencyPhone,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Nome = p.Nome,
                Idade = p.Idade,
                CPF = p.CPF,
                RG = p.RG,
                EstadoNascimento = p.EstadoNascimento,
                DataNascimento = p.DataNascimento,
                Sexo = p.Sexo,
                Telefone = p.Telefone,
                Endereco = p.Endereco,
                City = p.City,
                State = p.State,
                ZipCode = p.ZipCode,
                Segment = p.Segment
            };
        }
    }
}