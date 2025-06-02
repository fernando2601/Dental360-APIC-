using Dapper;
using System.Data;
using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IDbConnection _connection;

        public PatientRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            const string sql = @"
                SELECT Id, Name, Email, Phone, CPF, RG, DateOfBirth, Gender, 
                       Address, City, State, ZipCode, EmergencyContact, EmergencyPhone,
                       MedicalHistory, Allergies, Medications, Notes, Photo, Status,
                       PreferredContactMethod, Profession, MaritalStatus, IsActive,
                       CreatedAt, UpdatedAt, LastVisit, CreatedBy
                FROM Patients 
                ORDER BY Name";
            
            return await _connection.QueryAsync<Patient>(sql);
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            const string sql = @"
                SELECT Id, Name, Email, Phone, CPF, RG, DateOfBirth, Gender, 
                       Address, City, State, ZipCode, EmergencyContact, EmergencyPhone,
                       MedicalHistory, Allergies, Medications, Notes, Photo, Status,
                       PreferredContactMethod, Profession, MaritalStatus, IsActive,
                       CreatedAt, UpdatedAt, LastVisit, CreatedBy
                FROM Patients 
                WHERE Id = @Id";
            
            return await _connection.QueryFirstOrDefaultAsync<Patient>(sql, new { Id = id });
        }

        public async Task<Patient> CreatePatientAsync(CreatePatientDto patientDto, int createdBy)
        {
            const string sql = @"
                INSERT INTO Patients (Name, Email, Phone, CPF, RG, DateOfBirth, Gender, 
                                    Address, City, State, ZipCode, EmergencyContact, EmergencyPhone,
                                    MedicalHistory, Allergies, Medications, Notes, 
                                    PreferredContactMethod, Profession, MaritalStatus, 
                                    Status, IsActive, CreatedAt, CreatedBy)
                VALUES (@Name, @Email, @Phone, @CPF, @RG, @DateOfBirth, @Gender, 
                        @Address, @City, @State, @ZipCode, @EmergencyContact, @EmergencyPhone,
                        @MedicalHistory, @Allergies, @Medications, @Notes, 
                        @PreferredContactMethod, @Profession, @MaritalStatus, 
                        'active', 1, GETDATE(), @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            var id = await _connection.QuerySingleAsync<int>(sql, new 
            {
                patientDto.Name,
                patientDto.Email,
                patientDto.Phone,
                patientDto.CPF,
                patientDto.RG,
                patientDto.DateOfBirth,
                patientDto.Gender,
                patientDto.Address,
                patientDto.City,
                patientDto.State,
                patientDto.ZipCode,
                patientDto.EmergencyContact,
                patientDto.EmergencyPhone,
                patientDto.MedicalHistory,
                patientDto.Allergies,
                patientDto.Medications,
                patientDto.Notes,
                patientDto.PreferredContactMethod,
                patientDto.Profession,
                patientDto.MaritalStatus,
                CreatedBy = createdBy
            });

            return await GetPatientByIdAsync(id) ?? throw new InvalidOperationException("Failed to retrieve created patient");
        }

        public async Task<Patient?> UpdatePatientAsync(int id, UpdatePatientDto patientDto)
        {
            var setParts = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            if (!string.IsNullOrEmpty(patientDto.Name))
            {
                setParts.Add("Name = @Name");
                parameters.Add("Name", patientDto.Name);
            }

            if (!string.IsNullOrEmpty(patientDto.Email))
            {
                setParts.Add("Email = @Email");
                parameters.Add("Email", patientDto.Email);
            }

            if (!string.IsNullOrEmpty(patientDto.Phone))
            {
                setParts.Add("Phone = @Phone");
                parameters.Add("Phone", patientDto.Phone);
            }

            if (!string.IsNullOrEmpty(patientDto.CPF))
            {
                setParts.Add("CPF = @CPF");
                parameters.Add("CPF", patientDto.CPF);
            }

            if (patientDto.DateOfBirth.HasValue)
            {
                setParts.Add("DateOfBirth = @DateOfBirth");
                parameters.Add("DateOfBirth", patientDto.DateOfBirth);
            }

            if (!string.IsNullOrEmpty(patientDto.Status))
            {
                setParts.Add("Status = @Status");
                parameters.Add("Status", patientDto.Status);
            }

            if (setParts.Count == 0)
                return await GetPatientByIdAsync(id);

            setParts.Add("UpdatedAt = GETDATE()");

            var sql = $@"
                UPDATE Patients 
                SET {string.Join(", ", setParts)}
                WHERE Id = @Id";

            await _connection.ExecuteAsync(sql, parameters);
            return await GetPatientByIdAsync(id);
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            const string sql = "UPDATE Patients SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id = @Id";
            var rowsAffected = await _connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Patient>> GetPatientsWithFiltersAsync(PatientFilters filters)
        {
            var conditions = new List<string> { "IsActive = 1" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(filters.Search))
            {
                conditions.Add("(Name LIKE @Search OR Email LIKE @Search OR Phone LIKE @Search OR CPF LIKE @Search)");
                parameters.Add("Search", $"%{filters.Search}%");
            }

            if (!string.IsNullOrEmpty(filters.Status))
            {
                conditions.Add("Status = @Status");
                parameters.Add("Status", filters.Status);
            }

            if (!string.IsNullOrEmpty(filters.Gender))
            {
                conditions.Add("Gender = @Gender");
                parameters.Add("Gender", filters.Gender);
            }

            if (!string.IsNullOrEmpty(filters.City))
            {
                conditions.Add("City = @City");
                parameters.Add("City", filters.City);
            }

            var orderBy = $"ORDER BY {filters.SortBy} {filters.SortOrder.ToUpper()}";
            var offset = (filters.Page - 1) * filters.Limit;

            var sql = $@"
                SELECT * FROM Patients
                WHERE {string.Join(" AND ", conditions)}
                {orderBy}
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY";

            parameters.Add("Offset", offset);
            parameters.Add("Limit", filters.Limit);

            return await _connection.QueryAsync<Patient>(sql, parameters);
        }

        public async Task<int> GetPatientsCountAsync(PatientFilters filters)
        {
            var conditions = new List<string> { "IsActive = 1" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(filters.Search))
            {
                conditions.Add("(Name LIKE @Search OR Email LIKE @Search OR Phone LIKE @Search)");
                parameters.Add("Search", $"%{filters.Search}%");
            }

            if (!string.IsNullOrEmpty(filters.Status))
            {
                conditions.Add("Status = @Status");
                parameters.Add("Status", filters.Status);
            }

            var sql = $"SELECT COUNT(*) FROM Patients WHERE {string.Join(" AND ", conditions)}";
            return await _connection.QuerySingleAsync<int>(sql, parameters);
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            const string sql = @"
                SELECT TOP 20 * FROM Patients
                WHERE IsActive = 1 
                AND (Name LIKE @Search OR Email LIKE @Search OR Phone LIKE @Search OR CPF LIKE @Search)
                ORDER BY Name";
            
            return await _connection.QueryAsync<Patient>(sql, new { Search = $"%{searchTerm}%" });
        }

        public async Task<Patient?> GetPatientByCPFAsync(string cpf)
        {
            const string sql = "SELECT * FROM Patients WHERE CPF = @CPF AND IsActive = 1";
            return await _connection.QueryFirstOrDefaultAsync<Patient>(sql, new { CPF = cpf });
        }

        public async Task<Patient?> GetPatientByEmailAsync(string email)
        {
            const string sql = "SELECT * FROM Patients WHERE Email = @Email AND IsActive = 1";
            return await _connection.QueryFirstOrDefaultAsync<Patient>(sql, new { Email = email });
        }

        public async Task<Patient?> GetPatientByPhoneAsync(string phone)
        {
            const string sql = "SELECT * FROM Patients WHERE Phone = @Phone AND IsActive = 1";
            return await _connection.QueryFirstOrDefaultAsync<Patient>(sql, new { Phone = phone });
        }

        public async Task<PatientAnalytics> GetPatientAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var dateFilter = "";
            var parameters = new DynamicParameters();

            if (startDate.HasValue && endDate.HasValue)
            {
                dateFilter = "AND CreatedAt BETWEEN @StartDate AND @EndDate";
                parameters.Add("StartDate", startDate);
                parameters.Add("EndDate", endDate);
            }

            const string sql = $@"
                SELECT 
                    COUNT(*) as TotalPatients,
                    COUNT(CASE WHEN Status = 'active' THEN 1 END) as ActivePatients,
                    COUNT(CASE WHEN MONTH(CreatedAt) = MONTH(GETDATE()) AND YEAR(CreatedAt) = YEAR(GETDATE()) THEN 1 END) as NewPatientsThisMonth,
                    COUNT(CASE WHEN MONTH(CreatedAt) = MONTH(DATEADD(month, -1, GETDATE())) AND YEAR(CreatedAt) = YEAR(DATEADD(month, -1, GETDATE())) THEN 1 END) as NewPatientsLastMonth,
                    AVG(CAST(DATEDIFF(year, DateOfBirth, GETDATE()) as DECIMAL)) as AverageAge
                FROM Patients 
                WHERE IsActive = 1 {dateFilter}";

            return await _connection.QuerySingleAsync<PatientAnalytics>(sql, parameters);
        }

        public async Task<PatientMetrics> GetPatientMetricsAsync(int patientId)
        {
            const string sql = @"
                SELECT 
                    0 as TotalAppointments,
                    0 as CompletedAppointments,
                    0 as CancelledAppointments,
                    0 as NoShowAppointments,
                    0.0 as TotalSpent,
                    0.0 as AverageSpentPerVisit,
                    NULL as LastVisit,
                    NULL as NextAppointment,
                    0 as DaysSinceLastVisit,
                    0.0 as CompletionRate,
                    0.0 as NoShowRate,
                    'Regular' as PatientSegment,
                    'Low' as RiskLevel";

            return await _connection.QuerySingleAsync<PatientMetrics>(sql);
        }

        public async Task<IEnumerable<PatientWithMetrics>> GetPatientsWithMetricsAsync(PatientFilters filters)
        {
            var patients = await GetPatientsWithFiltersAsync(filters);
            var result = new List<PatientWithMetrics>();

            foreach (var patient in patients)
            {
                var metrics = await GetPatientMetricsAsync(patient.Id);
                result.Add(new PatientWithMetrics { Patient = patient, Metrics = metrics });
            }

            return result;
        }

        // Implementações simplificadas para os métodos restantes
        public async Task<IEnumerable<PatientSegmentation>> GetPatientSegmentationAsync()
        {
            return new List<PatientSegmentation>
            {
                new() { Segment = "VIP", Description = "Pacientes VIP", Color = "#gold", PatientCount = 0 },
                new() { Segment = "Regular", Description = "Pacientes Regulares", Color = "#blue", PatientCount = 0 },
                new() { Segment = "New", Description = "Novos Pacientes", Color = "#green", PatientCount = 0 }
            };
        }

        public async Task<IEnumerable<Patient>> GetPatientsBySegmentAsync(string segment)
        {
            return await GetPatientsWithFiltersAsync(new PatientFilters { Segment = segment });
        }

        public async Task<bool> UpdatePatientSegmentAsync(int patientId, string segment)
        {
            // Implementação futura
            return true;
        }

        public async Task<IEnumerable<AgeDistribution>> GetAgeDistributionAsync()
        {
            const string sql = @"
                SELECT 
                    CASE 
                        WHEN DATEDIFF(year, DateOfBirth, GETDATE()) < 18 THEN 'Menor de 18'
                        WHEN DATEDIFF(year, DateOfBirth, GETDATE()) BETWEEN 18 AND 30 THEN '18-30'
                        WHEN DATEDIFF(year, DateOfBirth, GETDATE()) BETWEEN 31 AND 50 THEN '31-50'
                        WHEN DATEDIFF(year, DateOfBirth, GETDATE()) BETWEEN 51 AND 70 THEN '51-70'
                        ELSE 'Acima de 70'
                    END as AgeRange,
                    COUNT(*) as Count,
                    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Patients WHERE IsActive = 1) as DECIMAL(5,2)) as Percentage
                FROM Patients
                WHERE IsActive = 1
                GROUP BY 
                    CASE 
                        WHEN DATEDIFF(year, DateOfBirth, GETDATE()) < 18 THEN 'Menor de 18'
                        WHEN DATEDIFF(year, DateOfBirth, GETDATE()) BETWEEN 18 AND 30 THEN '18-30'
                        WHEN DATEDIFF(year, DateOfBirth, GETDATE()) BETWEEN 31 AND 50 THEN '31-50'
                        WHEN DATEDIFF(year, DateOfBirth, GETDATE()) BETWEEN 51 AND 70 THEN '51-70'
                        ELSE 'Acima de 70'
                    END";

            return await _connection.QueryAsync<AgeDistribution>(sql);
        }

        public async Task<IEnumerable<GenderDistribution>> GetGenderDistributionAsync()
        {
            const string sql = @"
                SELECT 
                    Gender,
                    CASE Gender 
                        WHEN 'M' THEN 'Masculino'
                        WHEN 'F' THEN 'Feminino'
                        ELSE 'Outro'
                    END as GenderLabel,
                    COUNT(*) as Count,
                    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Patients WHERE IsActive = 1) as DECIMAL(5,2)) as Percentage,
                    CASE Gender 
                        WHEN 'M' THEN '#3B82F6'
                        WHEN 'F' THEN '#EC4899'
                        ELSE '#6B7280'
                    END as Color
                FROM Patients
                WHERE IsActive = 1
                GROUP BY Gender";

            return await _connection.QueryAsync<GenderDistribution>(sql);
        }

        public async Task<IEnumerable<LocationDistribution>> GetLocationDistributionAsync()
        {
            const string sql = @"
                SELECT 
                    ISNULL(City, 'Não informado') as City,
                    ISNULL(State, 'Não informado') as State,
                    COUNT(*) as Count,
                    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Patients WHERE IsActive = 1) as DECIMAL(5,2)) as Percentage
                FROM Patients
                WHERE IsActive = 1
                GROUP BY City, State
                ORDER BY Count DESC";

            return await _connection.QueryAsync<LocationDistribution>(sql);
        }

        public async Task<IEnumerable<MonthlyRegistration>> GetMonthlyRegistrationsAsync(int months = 12)
        {
            const string sql = @"
                SELECT 
                    FORMAT(CreatedAt, 'yyyy-MM') as Month,
                    FORMAT(CreatedAt, 'MMM yyyy', 'pt-BR') as MonthLabel,
                    COUNT(*) as Count,
                    0.0 as GrowthRate
                FROM Patients
                WHERE IsActive = 1 
                AND CreatedAt >= DATEADD(month, -@Months, GETDATE())
                GROUP BY FORMAT(CreatedAt, 'yyyy-MM'), FORMAT(CreatedAt, 'MMM yyyy', 'pt-BR')
                ORDER BY Month";

            return await _connection.QueryAsync<MonthlyRegistration>(sql, new { Months = months });
        }

        // Métodos simplificados para implementação futura
        public async Task<IEnumerable<PatientCommunication>> GetPatientCommunicationsAsync(int patientId)
        {
            return new List<PatientCommunication>();
        }

        public async Task<PatientCommunication> CreateCommunicationAsync(PatientCommunication communication)
        {
            return communication;
        }

        public async Task<bool> UpdateCommunicationStatusAsync(int communicationId, string status, DateTime? deliveredAt = null, DateTime? readAt = null)
        {
            return true;
        }

        public async Task<IEnumerable<PatientNote>> GetPatientNotesAsync(int patientId)
        {
            return new List<PatientNote>();
        }

        public async Task<PatientNote> CreatePatientNoteAsync(PatientNote note)
        {
            return note;
        }

        public async Task<PatientNote?> UpdatePatientNoteAsync(int noteId, string title, string content, string priority)
        {
            return null;
        }

        public async Task<bool> DeletePatientNoteAsync(int noteId)
        {
            return true;
        }

        public async Task<IEnumerable<PatientDocument>> GetPatientDocumentsAsync(int patientId)
        {
            return new List<PatientDocument>();
        }

        public async Task<PatientDocument> CreatePatientDocumentAsync(PatientDocument document)
        {
            return document;
        }

        public async Task<bool> DeletePatientDocumentAsync(int documentId)
        {
            return true;
        }

        public async Task<bool> BulkUpdatePatientsAsync(PatientBulkAction action, int updatedBy)
        {
            return true;
        }

        public async Task<bool> BulkActivatePatientsAsync(int[] patientIds, int updatedBy)
        {
            return true;
        }

        public async Task<bool> BulkDeactivatePatientsAsync(int[] patientIds, string reason, int updatedBy)
        {
            return true;
        }

        public async Task<PatientReport> GetPatientReportAsync(int patientId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var patient = await GetPatientByIdAsync(patientId);
            return new PatientReport 
            { 
                Patient = patient ?? new Patient(),
                GeneratedAt = DateTime.Now.ToString(),
                GeneratedBy = "Sistema"
            };
        }

        public async Task<IEnumerable<Patient>> GetPatientsForExportAsync(PatientExportRequest request)
        {
            return await GetPatientsWithFiltersAsync(request.Filters ?? new PatientFilters());
        }

        public async Task<IEnumerable<PatientInsight>> GetPatientInsightsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return new List<PatientInsight>();
        }

        public async Task<IEnumerable<Patient>> GetInactivePatientsAsync(int daysSinceLastVisit = 90)
        {
            return new List<Patient>();
        }

        public async Task<IEnumerable<Patient>> GetHighValuePatientsAsync(decimal minimumValue = 1000)
        {
            return new List<Patient>();
        }

        public async Task<IEnumerable<Patient>> GetRiskPatientsAsync()
        {
            return new List<Patient>();
        }

        public async Task<object> GetDashboardMetricsAsync()
        {
            var analytics = await GetPatientAnalyticsAsync();
            return new
            {
                totalPatients = analytics.TotalPatients,
                activePatients = analytics.ActivePatients,
                newThisMonth = analytics.NewPatientsThisMonth,
                averageAge = analytics.AverageAge
            };
        }

        public async Task<object> GetPatientGrowthAsync(int months = 12)
        {
            return await GetMonthlyRegistrationsAsync(months);
        }

        public async Task<object> GetPatientRetentionAsync()
        {
            return new { retentionRate = 85.5 };
        }

        public async Task<IEnumerable<AppointmentSummary>> GetPatientAppointmentHistoryAsync(int patientId)
        {
            return new List<AppointmentSummary>();
        }

        public async Task<IEnumerable<PaymentSummary>> GetPatientPaymentHistoryAsync(int patientId)
        {
            return new List<PaymentSummary>();
        }

        public async Task<bool> UpdateLastVisitAsync(int patientId, DateTime lastVisit)
        {
            const string sql = "UPDATE Patients SET LastVisit = @LastVisit, UpdatedAt = GETDATE() WHERE Id = @PatientId";
            var rowsAffected = await _connection.ExecuteAsync(sql, new { PatientId = patientId, LastVisit = lastVisit });
            return rowsAffected > 0;
        }

        public async Task<bool> IsCPFExistsAsync(string cpf, int? excludePatientId = null)
        {
            var sql = "SELECT COUNT(*) FROM Patients WHERE CPF = @CPF AND IsActive = 1";
            var parameters = new DynamicParameters();
            parameters.Add("CPF", cpf);

            if (excludePatientId.HasValue)
            {
                sql += " AND Id != @ExcludeId";
                parameters.Add("ExcludeId", excludePatientId);
            }

            var count = await _connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludePatientId = null)
        {
            var sql = "SELECT COUNT(*) FROM Patients WHERE Email = @Email AND IsActive = 1";
            var parameters = new DynamicParameters();
            parameters.Add("Email", email);

            if (excludePatientId.HasValue)
            {
                sql += " AND Id != @ExcludeId";
                parameters.Add("ExcludeId", excludePatientId);
            }

            var count = await _connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }

        public async Task<bool> IsPhoneExistsAsync(string phone, int? excludePatientId = null)
        {
            var sql = "SELECT COUNT(*) FROM Patients WHERE Phone = @Phone AND IsActive = 1";
            var parameters = new DynamicParameters();
            parameters.Add("Phone", phone);

            if (excludePatientId.HasValue)
            {
                sql += " AND Id != @ExcludeId";
                parameters.Add("ExcludeId", excludePatientId);
            }

            var count = await _connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }
    }
}