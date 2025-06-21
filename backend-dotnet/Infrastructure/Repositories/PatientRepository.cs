using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace DentalSpa.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IDbConnection _connection;

        public PatientRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            const string sql = "SELECT * FROM patients WHERE is_active = 1";
            return await Task.FromResult(_connection.Query<Patient>(sql));
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM patients WHERE id = @Id AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirstOrDefault<Patient>(sql, new { Id = id }));
        }

        public async Task<Patient> CreateAsync(Patient patient)
        {
            const string sql = @"
                INSERT INTO patients (name, email, phone, birth_date, address, is_active, created_at, updated_at)
                VALUES (@Name, @Email, @Phone, @BirthDate, @Address, 1, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            
            patient.CreatedAt = DateTime.UtcNow;
            patient.UpdatedAt = DateTime.UtcNow;
            
            var id = await Task.FromResult(_connection.QuerySingle<int>(sql, patient));
            patient.Id = id;
            return patient;
        }

        public async Task<Patient?> UpdateAsync(int id, Patient patient)
        {
            const string sql = @"
                UPDATE patients 
                SET name = @Name, email = @Email, phone = @Phone, 
                    birth_date = @BirthDate, address = @Address, updated_at = @UpdatedAt
                WHERE id = @Id AND is_active = 1";
            
            patient.Id = id;
            patient.UpdatedAt = DateTime.UtcNow;
            
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, patient));
            return rowsAffected > 0 ? patient : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE patients SET is_active = 0 WHERE id = @Id";
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, new { Id = id }));
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm)
        {
            const string sql = "SELECT * FROM patients WHERE is_active = 1 AND name LIKE @SearchTerm";
            return await Task.FromResult(_connection.Query<Patient>(sql, new { SearchTerm = $"%{searchTerm}%" }));
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            const string sql = @"
                SELECT Id, Name, Email, Phone, CPF, DateOfBirth, Gender, Address, 
                       City, State, ZipCode, EmergencyContact, EmergencyPhone, 
                       MedicalHistory, Allergies, InsuranceProvider, InsuranceNumber,
                       IsActive, CreatedAt, UpdatedAt, LastVisit
                FROM Patients 
                WHERE IsActive = 1 
                ORDER BY Name";

            return await _connection.QueryAsync<Patient>(sql);
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            const string sql = @"
                SELECT Id, Name, Email, Phone, CPF, DateOfBirth, Gender, Address, 
                       City, State, ZipCode, EmergencyContact, EmergencyPhone, 
                       MedicalHistory, Allergies, InsuranceProvider, InsuranceNumber,
                       IsActive, CreatedAt, UpdatedAt, LastVisit
                FROM Patients 
                WHERE Id = @Id AND IsActive = 1";

            return await _connection.QuerySingleOrDefaultAsync<Patient>(sql, new { Id = id });
        }

        public async Task<Patient?> GetPatientByCPFAsync(string cpf)
        {
            const string sql = @"
                SELECT Id, Name, Email, Phone, CPF, DateOfBirth, Gender, Address, 
                       City, State, ZipCode, EmergencyContact, EmergencyPhone, 
                       MedicalHistory, Allergies, InsuranceProvider, InsuranceNumber,
                       IsActive, CreatedAt, UpdatedAt, LastVisit
                FROM Patients 
                WHERE CPF = @CPF AND IsActive = 1";

            return await _connection.QuerySingleOrDefaultAsync<Patient>(sql, new { CPF = cpf });
        }

        public async Task<Patient?> GetPatientByEmailAsync(string email)
        {
            const string sql = @"
                SELECT Id, Name, Email, Phone, CPF, DateOfBirth, Gender, Address, 
                       City, State, ZipCode, EmergencyContact, EmergencyPhone, 
                       MedicalHistory, Allergies, InsuranceProvider, InsuranceNumber,
                       IsActive, CreatedAt, UpdatedAt, LastVisit
                FROM Patients 
                WHERE Email = @Email AND IsActive = 1";

            return await _connection.QuerySingleOrDefaultAsync<Patient>(sql, new { Email = email });
        }

        public async Task<Patient?> GetPatientByPhoneAsync(string phone)
        {
            const string sql = @"
                SELECT Id, Name, Email, Phone, CPF, DateOfBirth, Gender, Address, 
                       City, State, ZipCode, EmergencyContact, EmergencyPhone, 
                       MedicalHistory, Allergies, InsuranceProvider, InsuranceNumber,
                       IsActive, CreatedAt, UpdatedAt, LastVisit
                FROM Patients 
                WHERE Phone = @Phone AND IsActive = 1";

            return await _connection.QuerySingleOrDefaultAsync<Patient>(sql, new { Phone = phone });
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

        // Simplified methods that return basic data structures
        public async Task<object> GetDashboardMetricsAsync()
        {
            const string sql = @"
                SELECT 
                    COUNT(*) as TotalPatients,
                    COUNT(CASE WHEN LastVisit >= DATEADD(month, -6, GETDATE()) THEN 1 END) as ActivePatients,
                    COUNT(CASE WHEN CreatedAt >= DATEADD(month, -1, GETDATE()) THEN 1 END) as NewThisMonth,
                    AVG(DATEDIFF(YEAR, DateOfBirth, GETDATE())) as AverageAge
                FROM Patients 
                WHERE IsActive = 1";

            return await _connection.QuerySingleAsync<object>(sql);
        }

        public async Task<IEnumerable<Patient>> GetInactivePatientsAsync(int daysSinceLastVisit = 90)
        {
            const string sql = @"
                SELECT Id, Name, Email, Phone, CPF, DateOfBirth, Gender, Address, 
                       City, State, ZipCode, EmergencyContact, EmergencyPhone, 
                       MedicalHistory, Allergies, InsuranceProvider, InsuranceNumber,
                       IsActive, CreatedAt, UpdatedAt, LastVisit
                FROM Patients 
                WHERE IsActive = 1 
                AND (LastVisit IS NULL OR LastVisit < DATEADD(day, -@Days, GETDATE()))
                ORDER BY LastVisit";

            return await _connection.QueryAsync<Patient>(sql, new { Days = daysSinceLastVisit });
        }
    }
} 