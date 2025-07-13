using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class UserClinicRepository : IUserClinicRepository
    {
        private readonly string _connectionString;
        public UserClinicRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<UserClinic>> GetAllAsync()
        {
            var list = new List<UserClinic>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT UserId, ClinicId, CreatedAt, UpdatedAt, CreatedByUserId, UpdatedByUserId FROM UserClinic", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new UserClinic
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    ClinicId = reader.GetInt32(reader.GetOrdinal("ClinicId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("CreatedByUserId")) ? null : reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                    UpdatedByUserId = reader.IsDBNull(reader.GetOrdinal("UpdatedByUserId")) ? null : reader.GetInt32(reader.GetOrdinal("UpdatedByUserId")),
                });
            }
            return list;
        }
        public async Task<UserClinic?> GetByIdsAsync(int userId, int clinicId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT UserId, ClinicId, CreatedAt, UpdatedAt, CreatedByUserId, UpdatedByUserId FROM UserClinic WHERE UserId = @UserId AND ClinicId = @ClinicId", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@ClinicId", clinicId);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserClinic
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    ClinicId = reader.GetInt32(reader.GetOrdinal("ClinicId")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("CreatedByUserId")) ? null : reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                    UpdatedByUserId = reader.IsDBNull(reader.GetOrdinal("UpdatedByUserId")) ? null : reader.GetInt32(reader.GetOrdinal("UpdatedByUserId")),
                };
            }
            return null;
        }
        public async Task<UserClinic> CreateAsync(UserClinic userClinic)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("INSERT INTO UserClinic (UserId, ClinicId, CreatedAt, UpdatedAt, CreatedByUserId, UpdatedByUserId) VALUES (@UserId, @ClinicId, @CreatedAt, @UpdatedAt, @CreatedByUserId, @UpdatedByUserId)", connection);
            command.Parameters.AddWithValue("@UserId", userClinic.UserId);
            command.Parameters.AddWithValue("@ClinicId", userClinic.ClinicId);
            command.Parameters.AddWithValue("@CreatedAt", userClinic.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)userClinic.UpdatedAt ?? System.DBNull.Value);
            command.Parameters.AddWithValue("@CreatedByUserId", (object?)userClinic.CreatedByUserId ?? System.DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedByUserId", (object?)userClinic.UpdatedByUserId ?? System.DBNull.Value);
            await command.ExecuteNonQueryAsync();
            return userClinic;
        }
        public async Task<UserClinic?> UpdateAsync(int userId, int clinicId, UserClinic userClinic)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("UPDATE UserClinic SET UpdatedAt = @UpdatedAt, UpdatedByUserId = @UpdatedByUserId WHERE UserId = @UserId AND ClinicId = @ClinicId", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@ClinicId", clinicId);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)userClinic.UpdatedAt ?? System.DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedByUserId", (object?)userClinic.UpdatedByUserId ?? System.DBNull.Value);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0 ? userClinic : null;
        }
        public async Task<bool> DeleteAsync(int userId, int clinicId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM UserClinic WHERE UserId = @UserId AND ClinicId = @ClinicId", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@ClinicId", clinicId);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
} 