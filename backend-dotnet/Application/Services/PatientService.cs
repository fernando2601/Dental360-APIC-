using ClinicApi.Models;
using ClinicApi.Repositories;

namespace ClinicApi.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllPatientsAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetPatientByIdAsync(id);
        }

        public async Task<Patient> CreatePatientAsync(CreatePatientDto patientDto, int createdBy)
        {
            // Validações de negócio
            await ValidatePatientDataAsync(patientDto);
            
            return await _patientRepository.CreatePatientAsync(patientDto, createdBy);
        }

        public async Task<Patient?> UpdatePatientAsync(int id, UpdatePatientDto patientDto)
        {
            await ValidatePatientUpdateAsync(id, patientDto);
            return await _patientRepository.UpdatePatientAsync(id, patientDto);
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            return await _patientRepository.DeletePatientAsync(id);
        }

        public async Task<object> GetPatientsWithFiltersAsync(PatientFilters filters)
        {
            var patients = await _patientRepository.GetPatientsWithFiltersAsync(filters);
            var totalCount = await _patientRepository.GetPatientsCountAsync(filters);
            
            return new
            {
                patients = patients.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    email = p.Email,
                    phone = p.FormattedPhone,
                    cpf = p.FormattedCPF,
                    age = p.Age,
                    gender = p.Gender,
                    city = p.City,
                    status = p.Status,
                    lastVisit = p.LastVisit?.ToString("yyyy-MM-dd"),
                    createdAt = p.CreatedAt?.ToString("yyyy-MM-dd")
                }),
                pagination = new
                {
                    currentPage = filters.Page,
                    totalPages = (int)Math.Ceiling((double)totalCount / filters.Limit),
                    totalItems = totalCount,
                    itemsPerPage = filters.Limit
                },
                filters = new
                {
                    search = filters.Search,
                    status = filters.Status,
                    gender = filters.Gender,
                    city = filters.City,
                    sortBy = filters.SortBy,
                    sortOrder = filters.SortOrder
                }
            };
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            return await _patientRepository.SearchPatientsAsync(searchTerm);
        }

        public async Task<Patient?> GetPatientByCPFAsync(string cpf)
        {
            return await _patientRepository.GetPatientByCPFAsync(cpf);
        }

        public async Task<Patient?> GetPatientByEmailAsync(string email)
        {
            return await _patientRepository.GetPatientByEmailAsync(email);
        }

        public async Task<PatientAnalytics> GetPatientAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var analytics = await _patientRepository.GetPatientAnalyticsAsync(startDate, endDate);
            
            // Calcular taxa de crescimento
            if (analytics.NewPatientsLastMonth > 0)
            {
                analytics.NewPatientGrowthRate = Math.Round(
                    ((decimal)analytics.NewPatientsThisMonth - analytics.NewPatientsLastMonth) / 
                    analytics.NewPatientsLastMonth * 100, 2);
            }

            // Buscar distribuições
            analytics.AgeDistribution = (await _patientRepository.GetAgeDistributionAsync()).ToList();
            analytics.GenderDistribution = (await _patientRepository.GetGenderDistributionAsync()).ToList();
            analytics.LocationDistribution = (await _patientRepository.GetLocationDistributionAsync()).ToList();
            analytics.MonthlyRegistrations = (await _patientRepository.GetMonthlyRegistrationsAsync()).ToList();

            return analytics;
        }

        public async Task<PatientMetrics> GetPatientMetricsAsync(int patientId)
        {
            return await _patientRepository.GetPatientMetricsAsync(patientId);
        }

        public async Task<object> GetPatientsWithMetricsAsync(PatientFilters filters)
        {
            var patientsWithMetrics = await _patientRepository.GetPatientsWithMetricsAsync(filters);
            var totalCount = await _patientRepository.GetPatientsCountAsync(filters);
            
            return new
            {
                patients = patientsWithMetrics.Select(pm => new
                {
                    patient = new
                    {
                        id = pm.Patient.Id,
                        name = pm.Patient.Name,
                        email = pm.Patient.Email,
                        phone = pm.Patient.FormattedPhone,
                        age = pm.Patient.Age,
                        status = pm.Patient.Status
                    },
                    metrics = new
                    {
                        totalAppointments = pm.Metrics.TotalAppointments,
                        completedAppointments = pm.Metrics.CompletedAppointments,
                        totalSpent = pm.Metrics.TotalSpent,
                        lastVisit = pm.Metrics.LastVisit?.ToString("yyyy-MM-dd"),
                        segment = pm.Metrics.PatientSegment,
                        riskLevel = pm.Metrics.RiskLevel
                    }
                }),
                pagination = new
                {
                    currentPage = filters.Page,
                    totalPages = (int)Math.Ceiling((double)totalCount / filters.Limit),
                    totalItems = totalCount
                }
            };
        }

        public async Task<IEnumerable<PatientSegmentation>> GetPatientSegmentationAsync()
        {
            return await _patientRepository.GetPatientSegmentationAsync();
        }

        public async Task<IEnumerable<AgeDistribution>> GetAgeDistributionAsync()
        {
            return await _patientRepository.GetAgeDistributionAsync();
        }

        public async Task<IEnumerable<GenderDistribution>> GetGenderDistributionAsync()
        {
            return await _patientRepository.GetGenderDistributionAsync();
        }

        public async Task<IEnumerable<LocationDistribution>> GetLocationDistributionAsync()
        {
            return await _patientRepository.GetLocationDistributionAsync();
        }

        public async Task<PatientReport> GetPatientReportAsync(int patientId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var report = await _patientRepository.GetPatientReportAsync(patientId, startDate, endDate);
            
            // Buscar dados adicionais
            report.Appointments = (await _patientRepository.GetPatientAppointmentHistoryAsync(patientId)).ToList();
            report.Payments = (await _patientRepository.GetPatientPaymentHistoryAsync(patientId)).ToList();
            report.Metrics = await _patientRepository.GetPatientMetricsAsync(patientId);
            
            return report;
        }

        public async Task<object> ExportPatientsAsync(PatientExportRequest request)
        {
            var patients = await _patientRepository.GetPatientsForExportAsync(request);
            
            return new
            {
                format = request.Format,
                totalRecords = patients.Count(),
                exportedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                data = patients.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Email,
                    p.Phone,
                    p.CPF,
                    DateOfBirth = p.DateOfBirth.ToString("yyyy-MM-dd"),
                    Age = p.Age,
                    p.Gender,
                    p.City,
                    p.State,
                    p.Status,
                    CreatedAt = p.CreatedAt?.ToString("yyyy-MM-dd")
                })
            };
        }

        public async Task<object> GetDashboardMetricsAsync()
        {
            var analytics = await _patientRepository.GetPatientAnalyticsAsync();
            var dashboardMetrics = await _patientRepository.GetDashboardMetricsAsync();
            
            return new
            {
                summary = new
                {
                    totalPatients = analytics.TotalPatients,
                    activePatients = analytics.ActivePatients,
                    newThisMonth = analytics.NewPatientsThisMonth,
                    growthRate = analytics.NewPatientGrowthRate
                },
                demographics = new
                {
                    averageAge = Math.Round(analytics.AverageAge, 1),
                    genderDistribution = await _patientRepository.GetGenderDistributionAsync(),
                    ageDistribution = await _patientRepository.GetAgeDistributionAsync()
                },
                trends = new
                {
                    monthlyRegistrations = await _patientRepository.GetMonthlyRegistrationsAsync(6),
                    retentionRate = 85.5, // Valor exemplo
                    averageLifetimeValue = analytics.AverageLifetimeValue
                }
            };
        }

        public async Task<object> GetPatientGrowthAsync(int months = 12)
        {
            var monthlyData = await _patientRepository.GetMonthlyRegistrationsAsync(months);
            
            return new
            {
                period = new
                {
                    months,
                    startDate = DateTime.Now.AddMonths(-months).ToString("yyyy-MM"),
                    endDate = DateTime.Now.ToString("yyyy-MM")
                },
                growth = monthlyData.Select((data, index) => new
                {
                    month = data.Month,
                    monthLabel = data.MonthLabel,
                    count = data.Count,
                    growthRate = index > 0 ? 
                        Math.Round(((decimal)data.Count - monthlyData.ElementAt(index - 1).Count) / 
                        Math.Max(monthlyData.ElementAt(index - 1).Count, 1) * 100, 1) : 0
                }),
                summary = new
                {
                    totalNewPatients = monthlyData.Sum(m => m.Count),
                    averagePerMonth = Math.Round((decimal)monthlyData.Sum(m => m.Count) / months, 1),
                    peakMonth = monthlyData.OrderByDescending(m => m.Count).FirstOrDefault()?.MonthLabel
                }
            };
        }

        public async Task<object> GetPatientRetentionAsync()
        {
            var retention = await _patientRepository.GetPatientRetentionAsync();
            
            return new
            {
                retentionMetrics = retention,
                insights = new
                {
                    activePatients = "Pacientes que visitaram nos últimos 90 dias",
                    riskPatients = "Pacientes com mais de 120 dias sem consulta",
                    highValuePatients = "Pacientes com gastos acima de R$ 1.000"
                },
                recommendations = new[]
                {
                    "Criar campanha de reativação para pacientes inativos",
                    "Implementar programa de fidelidade para pacientes VIP",
                    "Enviar lembretes automáticos de consultas de retorno"
                }
            };
        }

        public async Task<bool> ValidatePatientDataAsync(CreatePatientDto patientDto)
        {
            var validationErrors = new List<string>();

            // Validar nome
            if (string.IsNullOrWhiteSpace(patientDto.Name))
                validationErrors.Add("Nome é obrigatório");

            // Validar email
            if (string.IsNullOrWhiteSpace(patientDto.Email))
                validationErrors.Add("Email é obrigatório");
            else if (await _patientRepository.IsEmailExistsAsync(patientDto.Email))
                validationErrors.Add("Email já está em uso");

            // Validar telefone
            if (string.IsNullOrWhiteSpace(patientDto.Phone))
                validationErrors.Add("Telefone é obrigatório");
            else if (await _patientRepository.IsPhoneExistsAsync(patientDto.Phone))
                validationErrors.Add("Telefone já está em uso");

            // Validar CPF se fornecido
            if (!string.IsNullOrEmpty(patientDto.CPF))
            {
                if (await _patientRepository.IsCPFExistsAsync(patientDto.CPF))
                    validationErrors.Add("CPF já está em uso");
            }

            // Validar data de nascimento
            if (patientDto.DateOfBirth > DateTime.Now)
                validationErrors.Add("Data de nascimento não pode ser futura");

            if (validationErrors.Any())
                throw new ArgumentException(string.Join("; ", validationErrors));

            return true;
        }

        public async Task<bool> ValidatePatientUpdateAsync(int id, UpdatePatientDto patientDto)
        {
            var validationErrors = new List<string>();

            // Validar email se fornecido
            if (!string.IsNullOrEmpty(patientDto.Email))
            {
                if (await _patientRepository.IsEmailExistsAsync(patientDto.Email, id))
                    validationErrors.Add("Email já está em uso por outro paciente");
            }

            // Validar telefone se fornecido
            if (!string.IsNullOrEmpty(patientDto.Phone))
            {
                if (await _patientRepository.IsPhoneExistsAsync(patientDto.Phone, id))
                    validationErrors.Add("Telefone já está em uso por outro paciente");
            }

            // Validar CPF se fornecido
            if (!string.IsNullOrEmpty(patientDto.CPF))
            {
                if (await _patientRepository.IsCPFExistsAsync(patientDto.CPF, id))
                    validationErrors.Add("CPF já está em uso por outro paciente");
            }

            // Validar data de nascimento se fornecida
            if (patientDto.DateOfBirth.HasValue && patientDto.DateOfBirth > DateTime.Now)
                validationErrors.Add("Data de nascimento não pode ser futura");

            if (validationErrors.Any())
                throw new ArgumentException(string.Join("; ", validationErrors));

            return true;
        }
    }
}