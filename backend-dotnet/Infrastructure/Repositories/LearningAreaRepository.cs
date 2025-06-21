using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class LearningAreaRepository : ILearningAreaRepository
    {
        private readonly IDbConnection _connection;

        public LearningAreaRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<LearningArea>> GetAllAsync()
        {
            var learningAreas = new List<LearningArea>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, title, description, content, category, difficulty_level, duration_minutes, is_active, created_at, updated_at FROM learning_areas WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        learningAreas.Add(new LearningArea
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Content = reader.IsDBNull(reader.GetOrdinal("content")) ? null : reader.GetString(reader.GetOrdinal("content")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            DifficultyLevel = reader.GetString(reader.GetOrdinal("difficulty_level")),
                            DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(learningAreas);
        }

        public async Task<LearningArea?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, title, description, content, category, difficulty_level, duration_minutes, is_active, created_at, updated_at FROM learning_areas WHERE id = @Id AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new LearningArea
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Content = reader.IsDBNull(reader.GetOrdinal("content")) ? null : reader.GetString(reader.GetOrdinal("content")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            DifficultyLevel = reader.GetString(reader.GetOrdinal("difficulty_level")),
                            DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<LearningArea?>(null);
        }

        public async Task<LearningArea> CreateAsync(LearningArea learningArea)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO learning_areas (title, description, content, category, difficulty_level, duration_minutes, is_active, created_at, updated_at) 
                                   VALUES (@Title, @Description, @Content, @Category, @DifficultyLevel, @DurationMinutes, @IsActive, @CreatedAt, @UpdatedAt);
                                   SELECT CAST(SCOPE_IDENTITY() as int)";
                
                cmd.Parameters.Add(CreateParameter("@Title", learningArea.Title));
                cmd.Parameters.Add(CreateParameter("@Description", learningArea.Description));
                cmd.Parameters.Add(CreateParameter("@Content", learningArea.Content));
                cmd.Parameters.Add(CreateParameter("@Category", learningArea.Category));
                cmd.Parameters.Add(CreateParameter("@DifficultyLevel", learningArea.DifficultyLevel));
                cmd.Parameters.Add(CreateParameter("@DurationMinutes", learningArea.DurationMinutes));
                cmd.Parameters.Add(CreateParameter("@IsActive", true));
                cmd.Parameters.Add(CreateParameter("@CreatedAt", DateTime.Now));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                learningArea.Id = id;
                learningArea.CreatedAt = DateTime.Now;
                learningArea.UpdatedAt = DateTime.Now;
                return await Task.FromResult(learningArea);
            }
        }

        public async Task<LearningArea?> UpdateAsync(int id, LearningArea learningArea)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE learning_areas SET title = @Title, description = @Description, content = @Content, category = @Category, difficulty_level = @DifficultyLevel, duration_minutes = @DurationMinutes, is_active = @IsActive, updated_at = @UpdatedAt WHERE id = @Id AND is_active = 1";
                
                cmd.Parameters.Add(CreateParameter("@Id", id));
                cmd.Parameters.Add(CreateParameter("@Title", learningArea.Title));
                cmd.Parameters.Add(CreateParameter("@Description", learningArea.Description));
                cmd.Parameters.Add(CreateParameter("@Content", learningArea.Content));
                cmd.Parameters.Add(CreateParameter("@Category", learningArea.Category));
                cmd.Parameters.Add(CreateParameter("@DifficultyLevel", learningArea.DifficultyLevel));
                cmd.Parameters.Add(CreateParameter("@DurationMinutes", learningArea.DurationMinutes));
                cmd.Parameters.Add(CreateParameter("@IsActive", learningArea.IsActive));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? learningArea : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE learning_areas SET is_active = 0 WHERE id = @Id";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<LearningArea>> GetByCategoryAsync(string category)
        {
            var learningAreas = new List<LearningArea>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, title, description, content, category, difficulty_level, duration_minutes, is_active, created_at, updated_at FROM learning_areas WHERE category = @Category AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Category", category));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        learningAreas.Add(new LearningArea
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Content = reader.IsDBNull(reader.GetOrdinal("content")) ? null : reader.GetString(reader.GetOrdinal("content")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            DifficultyLevel = reader.GetString(reader.GetOrdinal("difficulty_level")),
                            DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(learningAreas);
        }

        public async Task<IEnumerable<LearningArea>> GetActiveAreasAsync()
        {
            var learningAreas = new List<LearningArea>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, title, description, content, category, difficulty_level, duration_minutes, is_active, created_at, updated_at FROM learning_areas WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        learningAreas.Add(new LearningArea
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Content = reader.IsDBNull(reader.GetOrdinal("content")) ? null : reader.GetString(reader.GetOrdinal("content")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            DifficultyLevel = reader.GetString(reader.GetOrdinal("difficulty_level")),
                            DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(learningAreas);
        }

        public async Task<IEnumerable<LearningArea>> SearchByTitleAsync(string searchTerm)
        {
            var learningAreas = new List<LearningArea>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, title, description, content, category, difficulty_level, duration_minutes, is_active, created_at, updated_at FROM learning_areas WHERE is_active = 1 AND (title LIKE @SearchTerm OR description LIKE @SearchTerm)";
                cmd.Parameters.Add(CreateParameter("@SearchTerm", $"%{searchTerm}%"));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        learningAreas.Add(new LearningArea
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Content = reader.IsDBNull(reader.GetOrdinal("content")) ? null : reader.GetString(reader.GetOrdinal("content")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            DifficultyLevel = reader.GetString(reader.GetOrdinal("difficulty_level")),
                            DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(learningAreas);
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