using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public interface IStaffRepository
    {
        Task<IEnumerable<StaffDetailedModel>> GetAllWithDetailsAsync();
        Task<StaffDetailedModel?> GetByIdWithDetailsAsync(int id);
        Task<StaffModel> CreateAsync(CreateStaffRequest request);
        Task<StaffModel?> UpdateAsync(int id, UpdateStaffRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<StaffModel>> GetByDepartmentAsync(string department);
        Task<IEnumerable<StaffModel>> GetByPositionAsync(string position);
        Task<IEnumerable<StaffModel>> SearchAsync(string searchTerm);
        Task<StaffStatsResponse> GetStatsAsync();
        Task<IEnumerable<string>> GetDepartmentsAsync();
        Task<IEnumerable<string>> GetPositionsAsync();
        Task<IEnumerable<StaffModel>> GetTeamMembersAsync(int managerId);
    }

    public class StaffRepository : IStaffRepository
    {
        private readonly string _connectionString;

        public StaffRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<StaffDetailedModel>> GetAllWithDetailsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT 
                    s.id, s.full_name, s.email, s.phone, s.position, s.specialization, 
                    s.department, s.salary, s.hire_date, s.is_active, s.bio, 
                    s.profile_image_url, s.years_of_experience, s.license, 
                    s.created_at, s.updated_at, s.manager_id,
                    m.full_name as ManagerName,
                    COALESCE(patient_count.total, 0) as TotalPatients,
                    COALESCE(appointment_count.total, 0) as AppointmentsThisMonth,
                    COALESCE(revenue.total, 0) as RevenuThisMonth,
                    COALESCE(rating.avg_rating, 0) as AverageRating,
                    COALESCE(rating.total_reviews, 0) as TotalReviews,
                    s.certifications,
                    s.skills
                FROM staff s
                LEFT JOIN staff m ON s.manager_id = m.id
                LEFT JOIN (
                    SELECT staff_id, COUNT(*) as total 
                    FROM appointments 
                    WHERE status = 'completed'
                    GROUP BY staff_id
                ) patient_count ON s.id = patient_count.staff_id
                LEFT JOIN (
                    SELECT staff_id, COUNT(*) as total 
                    FROM appointments 
                    WHERE DATE_TRUNC('month', appointment_date) = DATE_TRUNC('month', CURRENT_DATE)
                    GROUP BY staff_id
                ) appointment_count ON s.id = appointment_count.staff_id
                LEFT JOIN (
                    SELECT a.staff_id, SUM(ft.amount) as total
                    FROM appointments a
                    INNER JOIN financial_transactions ft ON a.id = ft.appointment_id
                    WHERE DATE_TRUNC('month', a.appointment_date) = DATE_TRUNC('month', CURRENT_DATE)
                    AND ft.type = 'receita'
                    GROUP BY a.staff_id
                ) revenue ON s.id = revenue.staff_id
                LEFT JOIN (
                    SELECT staff_id, AVG(rating) as avg_rating, COUNT(*) as total_reviews
                    FROM staff_reviews
                    GROUP BY staff_id
                ) rating ON s.id = rating.staff_id
                ORDER BY s.full_name";

            var staff = await connection.QueryAsync<StaffDetailedModel>(sql);
            
            // Load certifications and skills for each staff member
            foreach (var member in staff)
            {
                member.Certifications = await GetCertificationsAsync(member.Id);
                member.Skills = await GetSkillsAsync(member.Id);
                member.TeamMembers = (await GetTeamMembersAsync(member.Id)).ToList();
            }

            return staff;
        }

        public async Task<StaffDetailedModel?> GetByIdWithDetailsAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT 
                    s.id, s.full_name, s.email, s.phone, s.position, s.specialization, 
                    s.department, s.salary, s.hire_date, s.is_active, s.bio, 
                    s.profile_image_url, s.years_of_experience, s.license, 
                    s.created_at, s.updated_at, s.manager_id,
                    m.full_name as ManagerName,
                    COALESCE(patient_count.total, 0) as TotalPatients,
                    COALESCE(appointment_count.total, 0) as AppointmentsThisMonth,
                    COALESCE(revenue.total, 0) as RevenuThisMonth,
                    COALESCE(rating.avg_rating, 0) as AverageRating,
                    COALESCE(rating.total_reviews, 0) as TotalReviews
                FROM staff s
                LEFT JOIN staff m ON s.manager_id = m.id
                LEFT JOIN (
                    SELECT staff_id, COUNT(DISTINCT patient_id) as total 
                    FROM appointments 
                    WHERE staff_id = @Id AND status = 'completed'
                    GROUP BY staff_id
                ) patient_count ON s.id = patient_count.staff_id
                LEFT JOIN (
                    SELECT staff_id, COUNT(*) as total 
                    FROM appointments 
                    WHERE staff_id = @Id 
                    AND DATE_TRUNC('month', appointment_date) = DATE_TRUNC('month', CURRENT_DATE)
                    GROUP BY staff_id
                ) appointment_count ON s.id = appointment_count.staff_id
                LEFT JOIN (
                    SELECT a.staff_id, SUM(ft.amount) as total
                    FROM appointments a
                    INNER JOIN financial_transactions ft ON a.id = ft.appointment_id
                    WHERE a.staff_id = @Id
                    AND DATE_TRUNC('month', a.appointment_date) = DATE_TRUNC('month', CURRENT_DATE)
                    AND ft.type = 'receita'
                    GROUP BY a.staff_id
                ) revenue ON s.id = revenue.staff_id
                LEFT JOIN (
                    SELECT staff_id, AVG(rating) as avg_rating, COUNT(*) as total_reviews
                    FROM staff_reviews
                    WHERE staff_id = @Id
                    GROUP BY staff_id
                ) rating ON s.id = rating.staff_id
                WHERE s.id = @Id";

            var staff = await connection.QueryFirstOrDefaultAsync<StaffDetailedModel>(sql, new { Id = id });
            
            if (staff != null)
            {
                staff.Certifications = await GetCertificationsAsync(staff.Id);
                staff.Skills = await GetSkillsAsync(staff.Id);
                staff.TeamMembers = (await GetTeamMembersAsync(staff.Id)).ToList();
            }

            return staff;
        }

        public async Task<StaffModel> CreateAsync(CreateStaffRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string sql = @"
                    INSERT INTO staff (
                        full_name, email, phone, position, specialization, department, 
                        salary, hire_date, is_active, bio, profile_image_url, 
                        years_of_experience, license, manager_id, created_at
                    )
                    VALUES (
                        @FullName, @Email, @Phone, @Position, @Specialization, @Department,
                        @Salary, @HireDate, @IsActive, @Bio, @ProfileImageUrl,
                        @YearsOfExperience, @License, @ManagerId, @CreatedAt
                    )
                    RETURNING id, full_name, email, phone, position, specialization, 
                             department, salary, hire_date, is_active, bio, 
                             profile_image_url, years_of_experience, license, 
                             created_at, updated_at, manager_id";

                var staff = await connection.QueryFirstAsync<StaffModel>(sql, new
                {
                    request.FullName,
                    request.Email,
                    request.Phone,
                    request.Position,
                    request.Specialization,
                    request.Department,
                    request.Salary,
                    request.HireDate,
                    request.IsActive,
                    request.Bio,
                    request.ProfileImageUrl,
                    request.YearsOfExperience,
                    request.License,
                    request.ManagerId,
                    CreatedAt = DateTime.UtcNow
                }, transaction);

                // Insert certifications
                if (request.Certifications.Any())
                {
                    await InsertCertificationsAsync(connection, transaction, staff.Id, request.Certifications);
                }

                // Insert skills
                if (request.Skills.Any())
                {
                    await InsertSkillsAsync(connection, transaction, staff.Id, request.Skills);
                }

                await transaction.CommitAsync();
                return staff;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<StaffModel?> UpdateAsync(int id, UpdateStaffRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string sql = @"
                    UPDATE staff 
                    SET full_name = @FullName, email = @Email, phone = @Phone, 
                        position = @Position, specialization = @Specialization, 
                        department = @Department, salary = @Salary, hire_date = @HireDate,
                        is_active = @IsActive, bio = @Bio, profile_image_url = @ProfileImageUrl,
                        years_of_experience = @YearsOfExperience, license = @License,
                        manager_id = @ManagerId, updated_at = @UpdatedAt
                    WHERE id = @Id
                    RETURNING id, full_name, email, phone, position, specialization, 
                             department, salary, hire_date, is_active, bio, 
                             profile_image_url, years_of_experience, license, 
                             created_at, updated_at, manager_id";

                var staff = await connection.QueryFirstOrDefaultAsync<StaffModel>(sql, new
                {
                    Id = id,
                    request.FullName,
                    request.Email,
                    request.Phone,
                    request.Position,
                    request.Specialization,
                    request.Department,
                    request.Salary,
                    request.HireDate,
                    request.IsActive,
                    request.Bio,
                    request.ProfileImageUrl,
                    request.YearsOfExperience,
                    request.License,
                    request.ManagerId,
                    UpdatedAt = DateTime.UtcNow
                }, transaction);

                if (staff != null)
                {
                    // Update certifications
                    await DeleteCertificationsAsync(connection, transaction, id);
                    if (request.Certifications.Any())
                    {
                        await InsertCertificationsAsync(connection, transaction, id, request.Certifications);
                    }

                    // Update skills
                    await DeleteSkillsAsync(connection, transaction, id);
                    if (request.Skills.Any())
                    {
                        await InsertSkillsAsync(connection, transaction, id, request.Skills);
                    }
                }

                await transaction.CommitAsync();
                return staff;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Delete related data first
                await DeleteCertificationsAsync(connection, transaction, id);
                await DeleteSkillsAsync(connection, transaction, id);

                const string sql = "DELETE FROM staff WHERE id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id }, transaction);
                
                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<StaffModel>> GetByDepartmentAsync(string department)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, full_name, email, phone, position, specialization, 
                       department, salary, hire_date, is_active, bio, 
                       profile_image_url, years_of_experience, license, 
                       created_at, updated_at, manager_id
                FROM staff 
                WHERE department = @Department AND is_active = true
                ORDER BY full_name";

            return await connection.QueryAsync<StaffModel>(sql, new { Department = department });
        }

        public async Task<IEnumerable<StaffModel>> GetByPositionAsync(string position)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, full_name, email, phone, position, specialization, 
                       department, salary, hire_date, is_active, bio, 
                       profile_image_url, years_of_experience, license, 
                       created_at, updated_at, manager_id
                FROM staff 
                WHERE position = @Position AND is_active = true
                ORDER BY full_name";

            return await connection.QueryAsync<StaffModel>(sql, new { Position = position });
        }

        public async Task<IEnumerable<StaffModel>> SearchAsync(string searchTerm)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, full_name, email, phone, position, specialization, 
                       department, salary, hire_date, is_active, bio, 
                       profile_image_url, years_of_experience, license, 
                       created_at, updated_at, manager_id
                FROM staff 
                WHERE (
                    full_name ILIKE @SearchTerm OR 
                    email ILIKE @SearchTerm OR 
                    position ILIKE @SearchTerm OR 
                    specialization ILIKE @SearchTerm OR 
                    department ILIKE @SearchTerm
                ) AND is_active = true
                ORDER BY full_name";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<StaffModel>(sql, new { SearchTerm = searchPattern });
        }

        public async Task<StaffStatsResponse> GetStatsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string statsSql = @"
                SELECT 
                    COUNT(*) as TotalStaff,
                    COUNT(CASE WHEN is_active = true THEN 1 END) as ActiveStaff,
                    COUNT(CASE WHEN is_active = false THEN 1 END) as InactiveStaff,
                    COALESCE(AVG(salary), 0) as AverageSalary,
                    COALESCE(AVG(years_of_experience), 0) as AverageExperience
                FROM staff";

            const string departmentSql = @"
                SELECT 
                    department as Department,
                    COUNT(*) as Count,
                    COALESCE(AVG(salary), 0) as AverageSalary
                FROM staff 
                WHERE is_active = true
                GROUP BY department
                ORDER BY department";

            const string positionSql = @"
                SELECT 
                    position as Position,
                    COUNT(*) as Count,
                    COALESCE(AVG(salary), 0) as AverageSalary
                FROM staff 
                WHERE is_active = true
                GROUP BY position
                ORDER BY position";

            var stats = await connection.QueryFirstAsync<StaffStatsResponse>(statsSql);
            var departmentStats = await connection.QueryAsync<DepartmentStats>(departmentSql);
            var positionStats = await connection.QueryAsync<PositionStats>(positionSql);
            
            stats.DepartmentBreakdown = departmentStats.ToList();
            stats.PositionBreakdown = positionStats.ToList();
            
            return stats;
        }

        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT DISTINCT department 
                FROM staff 
                WHERE is_active = true AND department IS NOT NULL AND department != ''
                ORDER BY department";

            return await connection.QueryAsync<string>(sql);
        }

        public async Task<IEnumerable<string>> GetPositionsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT DISTINCT position 
                FROM staff 
                WHERE is_active = true AND position IS NOT NULL AND position != ''
                ORDER BY position";

            return await connection.QueryAsync<string>(sql);
        }

        public async Task<IEnumerable<StaffModel>> GetTeamMembersAsync(int managerId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, full_name, email, phone, position, specialization, 
                       department, salary, hire_date, is_active, bio, 
                       profile_image_url, years_of_experience, license, 
                       created_at, updated_at, manager_id
                FROM staff 
                WHERE manager_id = @ManagerId AND is_active = true
                ORDER BY full_name";

            return await connection.QueryAsync<StaffModel>(sql, new { ManagerId = managerId });
        }

        // Helper methods
        private async Task<List<string>> GetCertificationsAsync(int staffId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "SELECT certification FROM staff_certifications WHERE staff_id = @StaffId";
            var certifications = await connection.QueryAsync<string>(sql, new { StaffId = staffId });
            return certifications.ToList();
        }

        private async Task<List<string>> GetSkillsAsync(int staffId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "SELECT skill FROM staff_skills WHERE staff_id = @StaffId";
            var skills = await connection.QueryAsync<string>(sql, new { StaffId = staffId });
            return skills.ToList();
        }

        private async Task InsertCertificationsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int staffId, List<string> certifications)
        {
            const string sql = "INSERT INTO staff_certifications (staff_id, certification) VALUES (@StaffId, @Certification)";
            foreach (var certification in certifications)
            {
                await connection.ExecuteAsync(sql, new { StaffId = staffId, Certification = certification }, transaction);
            }
        }

        private async Task InsertSkillsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int staffId, List<string> skills)
        {
            const string sql = "INSERT INTO staff_skills (staff_id, skill) VALUES (@StaffId, @Skill)";
            foreach (var skill in skills)
            {
                await connection.ExecuteAsync(sql, new { StaffId = staffId, Skill = skill }, transaction);
            }
        }

        private async Task DeleteCertificationsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int staffId)
        {
            const string sql = "DELETE FROM staff_certifications WHERE staff_id = @StaffId";
            await connection.ExecuteAsync(sql, new { StaffId = staffId }, transaction);
        }

        private async Task DeleteSkillsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int staffId)
        {
            const string sql = "DELETE FROM staff_skills WHERE staff_id = @StaffId";
            await connection.ExecuteAsync(sql, new { StaffId = staffId }, transaction);
        }
    }
}