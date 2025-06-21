using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class BeforeAfterRepository : IBeforeAfterRepository
    {
        private readonly IDbConnection _connection;

        public BeforeAfterRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<BeforeAfter>> GetAllAsync()
        {
            var list = new List<BeforeAfter>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, service_id, before_photo_url, after_photo_url, notes, treatment_date, is_public, created_at FROM before_after WHERE is_public = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BeforeAfter
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            ServiceId = reader.GetInt32(reader.GetOrdinal("service_id")),
                            BeforePhotoUrl = reader.GetString(reader.GetOrdinal("before_photo_url")),
                            AfterPhotoUrl = reader.GetString(reader.GetOrdinal("after_photo_url")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            TreatmentDate = reader.GetDateTime(reader.GetOrdinal("treatment_date")),
                            IsPublic = reader.GetBoolean(reader.GetOrdinal("is_public")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<BeforeAfter?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, service_id, before_photo_url, after_photo_url, notes, treatment_date, is_public, created_at FROM before_after WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BeforeAfter
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            ServiceId = reader.GetInt32(reader.GetOrdinal("service_id")),
                            BeforePhotoUrl = reader.GetString(reader.GetOrdinal("before_photo_url")),
                            AfterPhotoUrl = reader.GetString(reader.GetOrdinal("after_photo_url")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            TreatmentDate = reader.GetDateTime(reader.GetOrdinal("treatment_date")),
                            IsPublic = reader.GetBoolean(reader.GetOrdinal("is_public")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<BeforeAfter?>(null);
        }

        public async Task<BeforeAfter> CreateAsync(BeforeAfter beforeAfter)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO before_after (client_id, service_id, before_photo_url, after_photo_url, notes, treatment_date, is_public, created_at) VALUES (@ClientId, @ServiceId, @BeforePhotoUrl, @AfterPhotoUrl, @Notes, @TreatmentDate, @IsPublic, @CreatedAt); SELECT LASTVAL();";
                var p1 = cmd.CreateParameter(); p1.ParameterName = "@ClientId"; p1.Value = beforeAfter.ClientId; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@ServiceId"; p2.Value = beforeAfter.ServiceId; cmd.Parameters.Add(p2);
                var p3 = cmd.CreateParameter(); p3.ParameterName = "@BeforePhotoUrl"; p3.Value = beforeAfter.BeforePhotoUrl; cmd.Parameters.Add(p3);
                var p4 = cmd.CreateParameter(); p4.ParameterName = "@AfterPhotoUrl"; p4.Value = beforeAfter.AfterPhotoUrl; cmd.Parameters.Add(p4);
                var p5 = cmd.CreateParameter(); p5.ParameterName = "@Notes"; p5.Value = (object?)beforeAfter.Notes ?? DBNull.Value; cmd.Parameters.Add(p5);
                var p6 = cmd.CreateParameter(); p6.ParameterName = "@TreatmentDate"; p6.Value = beforeAfter.TreatmentDate; cmd.Parameters.Add(p6);
                var p7 = cmd.CreateParameter(); p7.ParameterName = "@IsPublic"; p7.Value = beforeAfter.IsPublic; cmd.Parameters.Add(p7);
                var p8 = cmd.CreateParameter(); p8.ParameterName = "@CreatedAt"; p8.Value = beforeAfter.CreatedAt; cmd.Parameters.Add(p8);
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                beforeAfter.Id = id;
                return await Task.FromResult(beforeAfter);
            }
        }

        public async Task<BeforeAfter?> UpdateAsync(int id, BeforeAfter beforeAfter)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE before_after SET client_id = @ClientId, service_id = @ServiceId, before_photo_url = @BeforePhotoUrl, after_photo_url = @AfterPhotoUrl, notes = @Notes, treatment_date = @TreatmentDate, is_public = @IsPublic WHERE id = @Id";
                var p0 = cmd.CreateParameter(); p0.ParameterName = "@Id"; p0.Value = id; cmd.Parameters.Add(p0);
                var p1 = cmd.CreateParameter(); p1.ParameterName = "@ClientId"; p1.Value = beforeAfter.ClientId; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@ServiceId"; p2.Value = beforeAfter.ServiceId; cmd.Parameters.Add(p2);
                var p3 = cmd.CreateParameter(); p3.ParameterName = "@BeforePhotoUrl"; p3.Value = beforeAfter.BeforePhotoUrl; cmd.Parameters.Add(p3);
                var p4 = cmd.CreateParameter(); p4.ParameterName = "@AfterPhotoUrl"; p4.Value = beforeAfter.AfterPhotoUrl; cmd.Parameters.Add(p4);
                var p5 = cmd.CreateParameter(); p5.ParameterName = "@Notes"; p5.Value = (object?)beforeAfter.Notes ?? DBNull.Value; cmd.Parameters.Add(p5);
                var p6 = cmd.CreateParameter(); p6.ParameterName = "@TreatmentDate"; p6.Value = beforeAfter.TreatmentDate; cmd.Parameters.Add(p6);
                var p7 = cmd.CreateParameter(); p7.ParameterName = "@IsPublic"; p7.Value = beforeAfter.IsPublic; cmd.Parameters.Add(p7);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? beforeAfter : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM before_after WHERE id = @Id";
                var param = cmd.CreateParameter(); param.ParameterName = "@Id"; param.Value = id; cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<BeforeAfter>> SearchAsync(string searchTerm)
        {
            var list = new List<BeforeAfter>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT ba.id, ba.client_id, ba.service_id, ba.before_photo_url, ba.after_photo_url, ba.notes, ba.treatment_date, ba.is_public, ba.created_at FROM before_after ba INNER JOIN clients c ON ba.client_id = c.id WHERE ba.is_public = 1 AND c.full_name ILIKE @SearchTerm";
                var param = cmd.CreateParameter(); param.ParameterName = "@SearchTerm"; param.Value = $"%{searchTerm}%"; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BeforeAfter
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            ServiceId = reader.GetInt32(reader.GetOrdinal("service_id")),
                            BeforePhotoUrl = reader.GetString(reader.GetOrdinal("before_photo_url")),
                            AfterPhotoUrl = reader.GetString(reader.GetOrdinal("after_photo_url")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            TreatmentDate = reader.GetDateTime(reader.GetOrdinal("treatment_date")),
                            IsPublic = reader.GetBoolean(reader.GetOrdinal("is_public")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<IEnumerable<BeforeAfter>> GetPublicAsync()
        {
            var list = new List<BeforeAfter>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, service_id, before_photo_url, after_photo_url, notes, treatment_date, is_public, created_at FROM before_after WHERE is_public = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BeforeAfter
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            ServiceId = reader.GetInt32(reader.GetOrdinal("service_id")),
                            BeforePhotoUrl = reader.GetString(reader.GetOrdinal("before_photo_url")),
                            AfterPhotoUrl = reader.GetString(reader.GetOrdinal("after_photo_url")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            TreatmentDate = reader.GetDateTime(reader.GetOrdinal("treatment_date")),
                            IsPublic = reader.GetBoolean(reader.GetOrdinal("is_public")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<IEnumerable<BeforeAfter>> GetByServiceAsync(int serviceId)
        {
            var list = new List<BeforeAfter>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, service_id, before_photo_url, after_photo_url, notes, treatment_date, is_public, created_at FROM before_after WHERE service_id = @ServiceId AND is_public = 1";
                var param = cmd.CreateParameter(); param.ParameterName = "@ServiceId"; param.Value = serviceId; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BeforeAfter
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            ServiceId = reader.GetInt32(reader.GetOrdinal("service_id")),
                            BeforePhotoUrl = reader.GetString(reader.GetOrdinal("before_photo_url")),
                            AfterPhotoUrl = reader.GetString(reader.GetOrdinal("after_photo_url")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            TreatmentDate = reader.GetDateTime(reader.GetOrdinal("treatment_date")),
                            IsPublic = reader.GetBoolean(reader.GetOrdinal("is_public")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT 1 FROM before_after WHERE id = @Id";
                var param = cmd.CreateParameter(); param.ParameterName = "@Id"; param.Value = id; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    return await Task.FromResult(reader.Read());
                }
            }
        }
    }
} 