using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;
        public RoomRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            var rooms = new List<Room>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name, ClinicId, IsActive FROM Room", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rooms.Add(new Room
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    ClinicId = reader.GetInt32(reader.GetOrdinal("ClinicId")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                });
            }
            return rooms;
        }
        public async Task<Room?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name, ClinicId, IsActive FROM Room WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Room
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    ClinicId = reader.GetInt32(reader.GetOrdinal("ClinicId")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };
            }
            return null;
        }
        public async Task<Room> CreateAsync(Room room)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("INSERT INTO Room (Name, ClinicId, IsActive) VALUES (@Name, @ClinicId, @IsActive); SELECT SCOPE_IDENTITY();", connection);
            command.Parameters.AddWithValue("@Name", room.Name);
            command.Parameters.AddWithValue("@ClinicId", room.ClinicId);
            command.Parameters.AddWithValue("@IsActive", room.IsActive);
            var id = (int)(decimal)await command.ExecuteScalarAsync();
            room.Id = id;
            return room;
        }
        public async Task<Room?> UpdateAsync(int id, Room room)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("UPDATE Room SET Name = @Name, ClinicId = @ClinicId, IsActive = @IsActive WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", room.Name);
            command.Parameters.AddWithValue("@ClinicId", room.ClinicId);
            command.Parameters.AddWithValue("@IsActive", room.IsActive);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0 ? room : null;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Room WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
} 