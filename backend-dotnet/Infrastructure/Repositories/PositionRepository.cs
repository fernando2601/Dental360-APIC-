using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly string _connectionString;
        public PositionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<Position>> GetAllAsync()
        {
            var positions = new List<Position>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name FROM Position", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                positions.Add(new Position
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                });
            }
            return positions;
        }
        public async Task<Position?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name FROM Position WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Position
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                };
            }
            return null;
        }
        public async Task<Position> CreateAsync(Position position)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("INSERT INTO Position (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();", connection);
            command.Parameters.AddWithValue("@Name", position.Name);
            var id = (int)(decimal)await command.ExecuteScalarAsync();
            position.Id = id;
            return position;
        }
        public async Task<Position?> UpdateAsync(int id, Position position)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("UPDATE Position SET Name = @Name WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", position.Name);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0 ? position : null;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Position WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
} 