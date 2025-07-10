using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class ClinicInfoRepository : IClinicInfoRepository
    {
        private readonly IDbConnection _connection;

        public ClinicInfoRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ClinicInfo?> GetClinicInfoAsync()
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, address, phone, email, website, opening_time, closing_time, working_days, city, state, zip_code, whatsapp, instagram, facebook, logo, is_active, created_at, updated_at FROM clinic_info WHERE is_active = 1 LIMIT 1";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ClinicInfo
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))!,
                            Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address"))!,
                            Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone"))!,
                            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email"))!,
                            Website = reader.IsDBNull(reader.GetOrdinal("website")) ? null : reader.GetString(reader.GetOrdinal("website"))!,
                            OpeningTime = reader.IsDBNull(reader.GetOrdinal("opening_time")) ? null : reader.GetString(reader.GetOrdinal("opening_time"))!,
                            ClosingTime = reader.IsDBNull(reader.GetOrdinal("closing_time")) ? null : reader.GetString(reader.GetOrdinal("closing_time"))!,
                            WorkingDays = reader.IsDBNull(reader.GetOrdinal("working_days")) ? null : reader.GetString(reader.GetOrdinal("working_days"))!,
                            City = reader.IsDBNull(reader.GetOrdinal("city")) ? null : reader.GetString(reader.GetOrdinal("city"))!,
                            State = reader.IsDBNull(reader.GetOrdinal("state")) ? null : reader.GetString(reader.GetOrdinal("state"))!,
                            ZipCode = reader.IsDBNull(reader.GetOrdinal("zip_code")) ? null : reader.GetString(reader.GetOrdinal("zip_code"))!,
                            WhatsApp = reader.IsDBNull(reader.GetOrdinal("whatsapp")) ? null : reader.GetString(reader.GetOrdinal("whatsapp"))!,
                            Instagram = reader.IsDBNull(reader.GetOrdinal("instagram")) ? null : reader.GetString(reader.GetOrdinal("instagram"))!,
                            Facebook = reader.IsDBNull(reader.GetOrdinal("facebook")) ? null : reader.GetString(reader.GetOrdinal("facebook"))!,
                            Logo = reader.IsDBNull(reader.GetOrdinal("logo")) ? null : reader.GetString(reader.GetOrdinal("logo"))!,
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<ClinicInfo?>(null);
        }

        public async Task<ClinicInfo> CreateOrUpdateClinicInfoAsync(ClinicInfo clinicInfo)
        {
            var existing = await GetClinicInfoAsync();
            
            if (existing == null)
            {
                // Create new
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO clinic_info (name, description, address, phone, email, website, opening_time, closing_time, working_days, city, state, zip_code, whatsapp, instagram, facebook, logo, is_active, created_at, updated_at) 
                                       VALUES (@Name, @Description, @Address, @Phone, @Email, @Website, @OpeningTime, @ClosingTime, @WorkingDays, @City, @State, @ZipCode, @WhatsApp, @Instagram, @Facebook, @Logo, @IsActive, @CreatedAt, @UpdatedAt);
                                       SELECT CAST(SCOPE_IDENTITY() as int)";
                    
                    cmd.Parameters.Add(CreateParameter("@Name", clinicInfo.Name));
                    cmd.Parameters.Add(CreateParameter("@Description", clinicInfo.Description));
                    cmd.Parameters.Add(CreateParameter("@Address", clinicInfo.Address));
                    cmd.Parameters.Add(CreateParameter("@Phone", clinicInfo.Phone));
                    cmd.Parameters.Add(CreateParameter("@Email", clinicInfo.Email));
                    cmd.Parameters.Add(CreateParameter("@Website", clinicInfo.Website));
                    cmd.Parameters.Add(CreateParameter("@OpeningTime", clinicInfo.OpeningTime));
                    cmd.Parameters.Add(CreateParameter("@ClosingTime", clinicInfo.ClosingTime));
                    cmd.Parameters.Add(CreateParameter("@WorkingDays", clinicInfo.WorkingDays));
                    cmd.Parameters.Add(CreateParameter("@City", clinicInfo.City));
                    cmd.Parameters.Add(CreateParameter("@State", clinicInfo.State));
                    cmd.Parameters.Add(CreateParameter("@ZipCode", clinicInfo.ZipCode));
                    cmd.Parameters.Add(CreateParameter("@WhatsApp", clinicInfo.WhatsApp));
                    cmd.Parameters.Add(CreateParameter("@Instagram", clinicInfo.Instagram));
                    cmd.Parameters.Add(CreateParameter("@Facebook", clinicInfo.Facebook));
                    cmd.Parameters.Add(CreateParameter("@Logo", clinicInfo.Logo));
                    cmd.Parameters.Add(CreateParameter("@IsActive", true));
                    cmd.Parameters.Add(CreateParameter("@CreatedAt", DateTime.Now));
                    cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                    
                    var id = Convert.ToInt32(cmd.ExecuteScalar());
                    clinicInfo.Id = id;
                    clinicInfo.CreatedAt = DateTime.Now;
                    clinicInfo.UpdatedAt = DateTime.Now;
                }
            }
            else
            {
                // Update existing
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE clinic_info SET name = @Name, description = @Description, address = @Address, phone = @Phone, email = @Email, website = @Website, opening_time = @OpeningTime, closing_time = @ClosingTime, working_days = @WorkingDays, city = @City, state = @State, zip_code = @ZipCode, whatsapp = @WhatsApp, instagram = @Instagram, facebook = @Facebook, logo = @Logo, is_active = @IsActive, updated_at = @UpdatedAt WHERE id = @Id";
                    
                    cmd.Parameters.Add(CreateParameter("@Id", existing.Id));
                    cmd.Parameters.Add(CreateParameter("@Name", clinicInfo.Name));
                    cmd.Parameters.Add(CreateParameter("@Description", clinicInfo.Description));
                    cmd.Parameters.Add(CreateParameter("@Address", clinicInfo.Address));
                    cmd.Parameters.Add(CreateParameter("@Phone", clinicInfo.Phone));
                    cmd.Parameters.Add(CreateParameter("@Email", clinicInfo.Email));
                    cmd.Parameters.Add(CreateParameter("@Website", clinicInfo.Website));
                    cmd.Parameters.Add(CreateParameter("@OpeningTime", clinicInfo.OpeningTime));
                    cmd.Parameters.Add(CreateParameter("@ClosingTime", clinicInfo.ClosingTime));
                    cmd.Parameters.Add(CreateParameter("@WorkingDays", clinicInfo.WorkingDays));
                    cmd.Parameters.Add(CreateParameter("@City", clinicInfo.City));
                    cmd.Parameters.Add(CreateParameter("@State", clinicInfo.State));
                    cmd.Parameters.Add(CreateParameter("@ZipCode", clinicInfo.ZipCode));
                    cmd.Parameters.Add(CreateParameter("@WhatsApp", clinicInfo.WhatsApp));
                    cmd.Parameters.Add(CreateParameter("@Instagram", clinicInfo.Instagram));
                    cmd.Parameters.Add(CreateParameter("@Facebook", clinicInfo.Facebook));
                    cmd.Parameters.Add(CreateParameter("@Logo", clinicInfo.Logo));
                    cmd.Parameters.Add(CreateParameter("@IsActive", clinicInfo.IsActive));
                    cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                    
                    cmd.ExecuteNonQuery();
                    clinicInfo.Id = existing.Id;
                    clinicInfo.CreatedAt = existing.CreatedAt;
                    clinicInfo.UpdatedAt = DateTime.Now;
                }
            }

            return await Task.FromResult(clinicInfo);
        }

        public async Task<bool> DeleteClinicInfoAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE clinic_info SET is_active = 0 WHERE id = @Id";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        private IDbDataParameter CreateParameter(string name, object? value)
        {
            var param = _connection.CreateCommand().CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }
    }
}