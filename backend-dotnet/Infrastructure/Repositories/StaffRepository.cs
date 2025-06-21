using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public interface IStaffRepository
    {
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<Staff?> GetByIdAsync(int id);
        Task<Staff> CreateAsync(Staff staff);
        Task<Staff?> UpdateAsync(int id, Staff staff);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Staff>> SearchAsync(string searchTerm);
        Task<IEnumerable<Staff>> GetBySpecializationAsync(string specialization);
    }

    public class StaffRepository : IStaffRepository
    {
        private readonly string _connectionString;

        public StaffRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "SELECT * FROM staff";
            var staff = await connection.QueryAsync<Staff>(sql);
            return staff;
        }

        public async Task<Staff?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "SELECT * FROM staff WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Staff>(sql, new { Id = id });
        }

        public async Task<Staff> CreateAsync(Staff staff)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = @"INSERT INTO staff (full_name, email, phone, position, specialization, department, salary, hire_date, is_active, bio, profile_image_url, years_of_experience, license, manager_id, created_at) VALUES (@FullName, @Email, @Phone, @Position, @Specialization, @Department, @Salary, @HireDate, @IsActive, @Bio, @ProfileImageUrl, @YearsOfExperience, @License, @ManagerId, @CreatedAt) RETURNING *;";
            var createdStaff = await connection.QueryFirstAsync<Staff>(sql, new
            {
                staff.FullName,
                staff.Email,
                staff.Phone,
                staff.Position,
                staff.Specialization,
                staff.Department,
                staff.Salary,
                staff.HireDate,
                staff.IsActive,
                staff.Bio,
                staff.ProfileImageUrl,
                staff.YearsOfExperience,
                staff.License,
                staff.ManagerId,
                CreatedAt = DateTime.UtcNow
            });
            return createdStaff;
        }

        public async Task<Staff?> UpdateAsync(int id, Staff staff)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = @"UPDATE staff SET full_name = @FullName, email = @Email, phone = @Phone, position = @Position, specialization = @Specialization, department = @Department, salary = @Salary, hire_date = @HireDate, is_active = @IsActive, bio = @Bio, profile_image_url = @ProfileImageUrl, years_of_experience = @YearsOfExperience, license = @License, manager_id = @ManagerId, updated_at = @UpdatedAt WHERE id = @Id RETURNING *;";
            var updatedStaff = await connection.QueryFirstOrDefaultAsync<Staff>(sql, new
            {
                Id = id,
                staff.FullName,
                staff.Email,
                staff.Phone,
                staff.Position,
                staff.Specialization,
                staff.Department,
                staff.Salary,
                staff.HireDate,
                staff.IsActive,
                staff.Bio,
                staff.ProfileImageUrl,
                staff.YearsOfExperience,
                staff.License,
                staff.ManagerId,
                UpdatedAt = DateTime.UtcNow
            });
            return updatedStaff;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "DELETE FROM staff WHERE id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<Staff>> SearchAsync(string searchTerm)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "SELECT * FROM staff WHERE full_name ILIKE @Search OR email ILIKE @Search OR phone ILIKE @Search";
            return await connection.QueryAsync<Staff>(sql, new { Search = $"%{searchTerm}%" });
        }

        public async Task<IEnumerable<Staff>> GetBySpecializationAsync(string specialization)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "SELECT * FROM staff WHERE specialization = @Specialization";
            return await connection.QueryAsync<Staff>(sql, new { Specialization = specialization });
        }
    }
}