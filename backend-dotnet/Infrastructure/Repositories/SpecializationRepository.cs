using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly string _connectionString;
        public SpecializationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<Specialization>> GetAllAsync()
        {
            var specializations = new List<Specialization>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name FROM Specialization", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                specializations.Add(new Specialization
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                });
            }
            return specializations;
        }
        public async Task<Specialization?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name FROM Specialization WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Specialization
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                };
            }
            return null;
        }
        public async Task<Specialization> CreateAsync(Specialization specialization)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("INSERT INTO Specialization (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();", connection);
            command.Parameters.AddWithValue("@Name", specialization.Name);
            var id = (int)(decimal)await command.ExecuteScalarAsync();
            specialization.Id = id;
            return specialization;
        }
        public async Task<Specialization?> UpdateAsync(int id, Specialization specialization)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("UPDATE Specialization SET Name = @Name WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", specialization.Name);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0 ? specialization : null;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Specialization WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
} 