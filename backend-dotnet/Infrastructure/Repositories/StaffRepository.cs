using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly IDbConnection _connection;

        public StaffRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            var list = new List<Staff>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, full_name, email, phone, position, specialization, department, salary, hire_date, is_active, bio, profile_image_url, years_of_experience, license, manager_id, created_at FROM staff WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Staff
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetString(reader.GetOrdinal("phone")),
                            Position = reader.GetString(reader.GetOrdinal("position")),
                            Specialization = reader.GetString(reader.GetOrdinal("specialization")),
                            Department = reader.GetString(reader.GetOrdinal("department")),
                            Salary = reader.GetDecimal(reader.GetOrdinal("salary")),
                            HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
                            ProfileImageUrl = reader.IsDBNull(reader.GetOrdinal("profile_image_url")) ? null : reader.GetString(reader.GetOrdinal("profile_image_url")),
                            YearsOfExperience = reader.GetInt32(reader.GetOrdinal("years_of_experience")),
                            License = reader.IsDBNull(reader.GetOrdinal("license")) ? null : reader.GetString(reader.GetOrdinal("license")),
                            ManagerId = reader.IsDBNull(reader.GetOrdinal("manager_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("manager_id")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<Staff?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, full_name, email, phone, position, specialization, department, salary, hire_date, is_active, bio, profile_image_url, years_of_experience, license, manager_id, created_at FROM staff WHERE id = @Id AND is_active = 1";
                var param = cmd.CreateParameter(); param.ParameterName = "@Id"; param.Value = id; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Staff
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetString(reader.GetOrdinal("phone")),
                            Position = reader.GetString(reader.GetOrdinal("position")),
                            Specialization = reader.GetString(reader.GetOrdinal("specialization")),
                            Department = reader.GetString(reader.GetOrdinal("department")),
                            Salary = reader.GetDecimal(reader.GetOrdinal("salary")),
                            HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
                            ProfileImageUrl = reader.IsDBNull(reader.GetOrdinal("profile_image_url")) ? null : reader.GetString(reader.GetOrdinal("profile_image_url")),
                            YearsOfExperience = reader.GetInt32(reader.GetOrdinal("years_of_experience")),
                            License = reader.IsDBNull(reader.GetOrdinal("license")) ? null : reader.GetString(reader.GetOrdinal("license")),
                            ManagerId = reader.IsDBNull(reader.GetOrdinal("manager_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("manager_id")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<Staff?>(null);
        }

        public async Task<Staff> CreateAsync(Staff staff)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO staff (full_name, email, phone, position, specialization, department, salary, hire_date, is_active, bio, profile_image_url, years_of_experience, license, manager_id, created_at) VALUES (@FullName, @Email, @Phone, @Position, @Specialization, @Department, @Salary, @HireDate, @IsActive, @Bio, @ProfileImageUrl, @YearsOfExperience, @License, @ManagerId, @CreatedAt); SELECT LASTVAL();";
                var p1 = cmd.CreateParameter(); p1.ParameterName = "@FullName"; p1.Value = staff.FullName; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@Email"; p2.Value = staff.Email; cmd.Parameters.Add(p2);
                var p3 = cmd.CreateParameter(); p3.ParameterName = "@Phone"; p3.Value = staff.Phone; cmd.Parameters.Add(p3);
                var p4 = cmd.CreateParameter(); p4.ParameterName = "@Position"; p4.Value = staff.Position; cmd.Parameters.Add(p4);
                var p5 = cmd.CreateParameter(); p5.ParameterName = "@Specialization"; p5.Value = staff.Specialization; cmd.Parameters.Add(p5);
                var p6 = cmd.CreateParameter(); p6.ParameterName = "@Department"; p6.Value = staff.Department; cmd.Parameters.Add(p6);
                var p7 = cmd.CreateParameter(); p7.ParameterName = "@Salary"; p7.Value = staff.Salary; cmd.Parameters.Add(p7);
                var p8 = cmd.CreateParameter(); p8.ParameterName = "@HireDate"; p8.Value = staff.HireDate; cmd.Parameters.Add(p8);
                var p9 = cmd.CreateParameter(); p9.ParameterName = "@IsActive"; p9.Value = staff.IsActive; cmd.Parameters.Add(p9);
                var p10 = cmd.CreateParameter(); p10.ParameterName = "@Bio"; p10.Value = (object?)staff.Bio ?? DBNull.Value; cmd.Parameters.Add(p10);
                var p11 = cmd.CreateParameter(); p11.ParameterName = "@ProfileImageUrl"; p11.Value = (object?)staff.ProfileImageUrl ?? DBNull.Value; cmd.Parameters.Add(p11);
                var p12 = cmd.CreateParameter(); p12.ParameterName = "@YearsOfExperience"; p12.Value = staff.YearsOfExperience; cmd.Parameters.Add(p12);
                var p13 = cmd.CreateParameter(); p13.ParameterName = "@License"; p13.Value = (object?)staff.License ?? DBNull.Value; cmd.Parameters.Add(p13);
                var p14 = cmd.CreateParameter(); p14.ParameterName = "@ManagerId"; p14.Value = (object?)staff.ManagerId ?? DBNull.Value; cmd.Parameters.Add(p14);
                var p15 = cmd.CreateParameter(); p15.ParameterName = "@CreatedAt"; p15.Value = staff.CreatedAt; cmd.Parameters.Add(p15);
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                staff.Id = id;
                return await Task.FromResult(staff);
            }
        }

        public async Task<Staff?> UpdateAsync(int id, Staff staff)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE staff SET full_name = @FullName, email = @Email, phone = @Phone, position = @Position, specialization = @Specialization, department = @Department, salary = @Salary, hire_date = @HireDate, is_active = @IsActive, bio = @Bio, profile_image_url = @ProfileImageUrl, years_of_experience = @YearsOfExperience, license = @License, manager_id = @ManagerId WHERE id = @Id";
                var p0 = cmd.CreateParameter(); p0.ParameterName = "@Id"; p0.Value = id; cmd.Parameters.Add(p0);
                var p1 = cmd.CreateParameter(); p1.ParameterName = "@FullName"; p1.Value = staff.FullName; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@Email"; p2.Value = staff.Email; cmd.Parameters.Add(p2);
                var p3 = cmd.CreateParameter(); p3.ParameterName = "@Phone"; p3.Value = staff.Phone; cmd.Parameters.Add(p3);
                var p4 = cmd.CreateParameter(); p4.ParameterName = "@Position"; p4.Value = staff.Position; cmd.Parameters.Add(p4);
                var p5 = cmd.CreateParameter(); p5.ParameterName = "@Specialization"; p5.Value = staff.Specialization; cmd.Parameters.Add(p5);
                var p6 = cmd.CreateParameter(); p6.ParameterName = "@Department"; p6.Value = staff.Department; cmd.Parameters.Add(p6);
                var p7 = cmd.CreateParameter(); p7.ParameterName = "@Salary"; p7.Value = staff.Salary; cmd.Parameters.Add(p7);
                var p8 = cmd.CreateParameter(); p8.ParameterName = "@HireDate"; p8.Value = staff.HireDate; cmd.Parameters.Add(p8);
                var p9 = cmd.CreateParameter(); p9.ParameterName = "@IsActive"; p9.Value = staff.IsActive; cmd.Parameters.Add(p9);
                var p10 = cmd.CreateParameter(); p10.ParameterName = "@Bio"; p10.Value = (object?)staff.Bio ?? DBNull.Value; cmd.Parameters.Add(p10);
                var p11 = cmd.CreateParameter(); p11.ParameterName = "@ProfileImageUrl"; p11.Value = (object?)staff.ProfileImageUrl ?? DBNull.Value; cmd.Parameters.Add(p11);
                var p12 = cmd.CreateParameter(); p12.ParameterName = "@YearsOfExperience"; p12.Value = staff.YearsOfExperience; cmd.Parameters.Add(p12);
                var p13 = cmd.CreateParameter(); p13.ParameterName = "@License"; p13.Value = (object?)staff.License ?? DBNull.Value; cmd.Parameters.Add(p13);
                var p14 = cmd.CreateParameter(); p14.ParameterName = "@ManagerId"; p14.Value = (object?)staff.ManagerId ?? DBNull.Value; cmd.Parameters.Add(p14);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? staff : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE staff SET is_active = 0 WHERE id = @Id";
                var param = cmd.CreateParameter(); param.ParameterName = "@Id"; param.Value = id; cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<Staff>> SearchAsync(string searchTerm)
        {
            var list = new List<Staff>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, full_name, email, phone, position, specialization, department, salary, hire_date, is_active, bio, profile_image_url, years_of_experience, license, manager_id, created_at FROM staff WHERE (full_name ILIKE @Search OR email ILIKE @Search OR phone ILIKE @Search) AND is_active = 1";
                var param = cmd.CreateParameter(); param.ParameterName = "@Search"; param.Value = $"%{searchTerm}%"; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Staff
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetString(reader.GetOrdinal("phone")),
                            Position = reader.GetString(reader.GetOrdinal("position")),
                            Specialization = reader.GetString(reader.GetOrdinal("specialization")),
                            Department = reader.GetString(reader.GetOrdinal("department")),
                            Salary = reader.GetDecimal(reader.GetOrdinal("salary")),
                            HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
                            ProfileImageUrl = reader.IsDBNull(reader.GetOrdinal("profile_image_url")) ? null : reader.GetString(reader.GetOrdinal("profile_image_url")),
                            YearsOfExperience = reader.GetInt32(reader.GetOrdinal("years_of_experience")),
                            License = reader.IsDBNull(reader.GetOrdinal("license")) ? null : reader.GetString(reader.GetOrdinal("license")),
                            ManagerId = reader.IsDBNull(reader.GetOrdinal("manager_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("manager_id")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<IEnumerable<Staff>> GetBySpecializationAsync(string specialization)
        {
            var list = new List<Staff>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, full_name, email, phone, position, specialization, department, salary, hire_date, is_active, bio, profile_image_url, years_of_experience, license, manager_id, created_at FROM staff WHERE specialization = @Specialization AND is_active = 1";
                var param = cmd.CreateParameter(); param.ParameterName = "@Specialization"; param.Value = specialization; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Staff
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetString(reader.GetOrdinal("phone")),
                            Position = reader.GetString(reader.GetOrdinal("position")),
                            Specialization = reader.GetString(reader.GetOrdinal("specialization")),
                            Department = reader.GetString(reader.GetOrdinal("department")),
                            Salary = reader.GetDecimal(reader.GetOrdinal("salary")),
                            HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
                            ProfileImageUrl = reader.IsDBNull(reader.GetOrdinal("profile_image_url")) ? null : reader.GetString(reader.GetOrdinal("profile_image_url")),
                            YearsOfExperience = reader.GetInt32(reader.GetOrdinal("years_of_experience")),
                            License = reader.IsDBNull(reader.GetOrdinal("license")) ? null : reader.GetString(reader.GetOrdinal("license")),
                            ManagerId = reader.IsDBNull(reader.GetOrdinal("manager_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("manager_id")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task SetStaffServicesAsync(int staffId, List<int> serviceIds)
        {
            // Remove existing relations
            using (var deleteCmd = _connection.CreateCommand())
            {
                deleteCmd.CommandText = "DELETE FROM staff_service WHERE staff_id = @StaffId";
                var param = deleteCmd.CreateParameter(); param.ParameterName = "@StaffId"; param.Value = staffId; deleteCmd.Parameters.Add(param);
                deleteCmd.ExecuteNonQuery();
            }
            // Add new relations
            foreach (var serviceId in serviceIds)
            {
                using (var insertCmd = _connection.CreateCommand())
                {
                    insertCmd.CommandText = "INSERT INTO staff_service (staff_id, service_id) VALUES (@StaffId, @ServiceId)";
                    var p1 = insertCmd.CreateParameter(); p1.ParameterName = "@StaffId"; p1.Value = staffId; insertCmd.Parameters.Add(p1);
                    var p2 = insertCmd.CreateParameter(); p2.ParameterName = "@ServiceId"; p2.Value = serviceId; insertCmd.Parameters.Add(p2);
                    insertCmd.ExecuteNonQuery();
                }
            }
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Staff>> GetStaffByDepartmentAsync(string department)
        {
            var list = new List<Staff>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, full_name, email, phone, position, specialization, department, salary, hire_date, is_active, bio, profile_image_url, years_of_experience, license, manager_id, created_at FROM staff WHERE department = @Department AND is_active = 1";
                var param = cmd.CreateParameter(); param.ParameterName = "@Department"; param.Value = department; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Staff
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetString(reader.GetOrdinal("phone")),
                            Position = reader.GetString(reader.GetOrdinal("position")),
                            Specialization = reader.GetString(reader.GetOrdinal("specialization")),
                            Department = reader.GetString(reader.GetOrdinal("department")),
                            Salary = reader.GetDecimal(reader.GetOrdinal("salary")),
                            HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
                            ProfileImageUrl = reader.IsDBNull(reader.GetOrdinal("profile_image_url")) ? null : reader.GetString(reader.GetOrdinal("profile_image_url")),
                            YearsOfExperience = reader.GetInt32(reader.GetOrdinal("years_of_experience")),
                            License = reader.IsDBNull(reader.GetOrdinal("license")) ? null : reader.GetString(reader.GetOrdinal("license")),
                            ManagerId = reader.IsDBNull(reader.GetOrdinal("manager_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("manager_id")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<IEnumerable<Staff>> GetStaffByPositionAsync(string position)
        {
            var list = new List<Staff>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, full_name, email, phone, position, specialization, department, salary, hire_date, is_active, bio, profile_image_url, years_of_experience, license, manager_id, created_at FROM staff WHERE position = @Position AND is_active = 1";
                var param = cmd.CreateParameter(); param.ParameterName = "@Position"; param.Value = position; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Staff
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetString(reader.GetOrdinal("phone")),
                            Position = reader.GetString(reader.GetOrdinal("position")),
                            Specialization = reader.GetString(reader.GetOrdinal("specialization")),
                            Department = reader.GetString(reader.GetOrdinal("department")),
                            Salary = reader.GetDecimal(reader.GetOrdinal("salary")),
                            HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
                            ProfileImageUrl = reader.IsDBNull(reader.GetOrdinal("profile_image_url")) ? null : reader.GetString(reader.GetOrdinal("profile_image_url")),
                            YearsOfExperience = reader.GetInt32(reader.GetOrdinal("years_of_experience")),
                            License = reader.IsDBNull(reader.GetOrdinal("license")) ? null : reader.GetString(reader.GetOrdinal("license")),
                            ManagerId = reader.IsDBNull(reader.GetOrdinal("manager_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("manager_id")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<IEnumerable<Staff>> GetTeamMembersAsync(int managerId)
        {
            var list = new List<Staff>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, full_name, email, phone, position, specialization, department, salary, hire_date, is_active, bio, profile_image_url, years_of_experience, license, manager_id, created_at FROM staff WHERE manager_id = @ManagerId AND is_active = 1";
                var param = cmd.CreateParameter(); param.ParameterName = "@ManagerId"; param.Value = managerId; cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Staff
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FullName = reader.GetString(reader.GetOrdinal("full_name")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Phone = reader.GetString(reader.GetOrdinal("phone")),
                            Position = reader.GetString(reader.GetOrdinal("position")),
                            Specialization = reader.GetString(reader.GetOrdinal("specialization")),
                            Department = reader.GetString(reader.GetOrdinal("department")),
                            Salary = reader.GetDecimal(reader.GetOrdinal("salary")),
                            HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
                            ProfileImageUrl = reader.IsDBNull(reader.GetOrdinal("profile_image_url")) ? null : reader.GetString(reader.GetOrdinal("profile_image_url")),
                            YearsOfExperience = reader.GetInt32(reader.GetOrdinal("years_of_experience")),
                            License = reader.IsDBNull(reader.GetOrdinal("license")) ? null : reader.GetString(reader.GetOrdinal("license")),
                            ManagerId = reader.IsDBNull(reader.GetOrdinal("manager_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("manager_id")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<IEnumerable<Staff>> GetAllStaffAsync() => await GetAllAsync();
        public async Task<Staff?> GetStaffByIdAsync(int id) => await GetByIdAsync(id);
        public async Task<IEnumerable<Staff>> SearchStaffAsync(string searchTerm) => await SearchAsync(searchTerm);
    }
} 