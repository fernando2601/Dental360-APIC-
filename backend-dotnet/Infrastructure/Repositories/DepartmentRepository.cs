using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly string _connectionString;
        public DepartmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            var departments = new List<Department>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name FROM Department", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                departments.Add(new Department
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                });
            }
            return departments;
        }
        public async Task<Department?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name FROM Department WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Department
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                };
            }
            return null;
        }
        public async Task<Department> CreateAsync(Department department)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("INSERT INTO Department (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();", connection);
            command.Parameters.AddWithValue("@Name", department.Name);
            var id = (int)(decimal)await command.ExecuteScalarAsync();
            department.Id = id;
            return department;
        }
        public async Task<Department?> UpdateAsync(int id, Department department)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("UPDATE Department SET Name = @Name WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", department.Name);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0 ? department : null;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Department WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
} 