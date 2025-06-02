using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public interface ILearningRepository
    {
        Task<IEnumerable<LearningContentDetailedModel>> GetAllContentAsync(int? userId = null);
        Task<LearningContentDetailedModel?> GetContentByIdAsync(int id, int? userId = null);
        Task<LearningContentModel> CreateContentAsync(CreateLearningContentRequest request, int createdBy);
        Task<LearningContentModel?> UpdateContentAsync(int id, UpdateLearningContentRequest request);
        Task<bool> DeleteContentAsync(int id);
        Task<IEnumerable<LearningContentModel>> GetContentByCategoryAsync(string category);
        Task<IEnumerable<LearningContentModel>> SearchContentAsync(string searchTerm);
        Task<LearningStatsResponse> GetStatsAsync();
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<LearningProgressModel?> UpdateProgressAsync(int contentId, int userId, UpdateProgressRequest request);
        Task<LearningCommentModel> AddCommentAsync(int contentId, int userId, AddCommentRequest request);
        Task<IEnumerable<LearningProgressModel>> GetUserProgressAsync(int userId);
        Task<IEnumerable<LearningContentModel>> GetRecommendedContentAsync(int userId);
    }

    public class LearningRepository : ILearningRepository
    {
        private readonly string _connectionString;

        public LearningRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<LearningContentDetailedModel>> GetAllContentAsync(int? userId = null)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT 
                    lc.id, lc.title, lc.description, lc.content_type, lc.category, 
                    lc.difficulty, lc.content_url, lc.thumbnail_url, lc.duration_minutes, 
                    lc.is_active, lc.created_by, lc.created_at, lc.updated_at,
                    u.full_name as CreatedByName,
                    COALESCE(AVG(lcr.rating), 0) as Rating,
                    COUNT(lcr.rating) as TotalRatings,
                    COALESCE(lcs.view_count, 0) as ViewCount,
                    COALESCE(lcs.completion_count, 0) as CompletionCount,
                    CASE WHEN @UserId IS NOT NULL THEN
                        COALESCE(lp.is_completed, false)
                    ELSE false END as IsCompleted,
                    CASE WHEN @UserId IS NOT NULL THEN
                        lp.completed_at
                    ELSE NULL END as CompletedAt,
                    CASE WHEN @UserId IS NOT NULL THEN
                        COALESCE(lcr_user.rating, 0)
                    ELSE 0 END as UserRating
                FROM learning_content lc
                LEFT JOIN users u ON lc.created_by = u.id
                LEFT JOIN learning_content_stats lcs ON lc.id = lcs.content_id
                LEFT JOIN learning_comments lcr ON lc.id = lcr.content_id
                LEFT JOIN learning_progress lp ON lc.id = lp.content_id AND lp.user_id = @UserId
                LEFT JOIN learning_comments lcr_user ON lc.id = lcr_user.content_id AND lcr_user.user_id = @UserId
                WHERE lc.is_active = true
                GROUP BY lc.id, lc.title, lc.description, lc.content_type, lc.category, 
                         lc.difficulty, lc.content_url, lc.thumbnail_url, lc.duration_minutes, 
                         lc.is_active, lc.created_by, lc.created_at, lc.updated_at,
                         u.full_name, lcs.view_count, lcs.completion_count, lp.is_completed,
                         lp.completed_at, lcr_user.rating
                ORDER BY lc.created_at DESC";

            var content = await connection.QueryAsync<LearningContentDetailedModel>(sql, new { UserId = userId });
            
            // Load tags for each content
            foreach (var item in content)
            {
                item.Tags = await GetContentTagsAsync(item.Id);
            }

            return content;
        }

        public async Task<LearningContentDetailedModel?> GetContentByIdAsync(int id, int? userId = null)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Update view count
            await UpdateViewCountAsync(connection, id);

            const string sql = @"
                SELECT 
                    lc.id, lc.title, lc.description, lc.content_type, lc.category, 
                    lc.difficulty, lc.content_url, lc.thumbnail_url, lc.duration_minutes, 
                    lc.is_active, lc.created_by, lc.created_at, lc.updated_at,
                    u.full_name as CreatedByName,
                    COALESCE(AVG(lcr.rating), 0) as Rating,
                    COUNT(lcr.rating) as TotalRatings,
                    COALESCE(lcs.view_count, 0) as ViewCount,
                    COALESCE(lcs.completion_count, 0) as CompletionCount,
                    CASE WHEN @UserId IS NOT NULL THEN
                        COALESCE(lp.is_completed, false)
                    ELSE false END as IsCompleted,
                    CASE WHEN @UserId IS NOT NULL THEN
                        lp.completed_at
                    ELSE NULL END as CompletedAt,
                    CASE WHEN @UserId IS NOT NULL THEN
                        COALESCE(lcr_user.rating, 0)
                    ELSE 0 END as UserRating
                FROM learning_content lc
                LEFT JOIN users u ON lc.created_by = u.id
                LEFT JOIN learning_content_stats lcs ON lc.id = lcs.content_id
                LEFT JOIN learning_comments lcr ON lc.id = lcr.content_id
                LEFT JOIN learning_progress lp ON lc.id = lp.content_id AND lp.user_id = @UserId
                LEFT JOIN learning_comments lcr_user ON lc.id = lcr_user.content_id AND lcr_user.user_id = @UserId
                WHERE lc.id = @Id
                GROUP BY lc.id, lc.title, lc.description, lc.content_type, lc.category, 
                         lc.difficulty, lc.content_url, lc.thumbnail_url, lc.duration_minutes, 
                         lc.is_active, lc.created_by, lc.created_at, lc.updated_at,
                         u.full_name, lcs.view_count, lcs.completion_count, lp.is_completed,
                         lp.completed_at, lcr_user.rating";

            var content = await connection.QueryFirstOrDefaultAsync<LearningContentDetailedModel>(sql, new { Id = id, UserId = userId });
            
            if (content != null)
            {
                content.Tags = await GetContentTagsAsync(content.Id);
                content.Comments = (await GetContentCommentsAsync(content.Id)).ToList();
            }

            return content;
        }

        public async Task<LearningContentModel> CreateContentAsync(CreateLearningContentRequest request, int createdBy)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string sql = @"
                    INSERT INTO learning_content (
                        title, description, content_type, category, difficulty, 
                        content_url, thumbnail_url, duration_minutes, is_active, 
                        created_by, created_at
                    )
                    VALUES (
                        @Title, @Description, @ContentType, @Category, @Difficulty,
                        @ContentUrl, @ThumbnailUrl, @DurationMinutes, @IsActive,
                        @CreatedBy, @CreatedAt
                    )
                    RETURNING id, title, description, content_type, category, difficulty, 
                             content_url, thumbnail_url, duration_minutes, is_active, 
                             created_by, created_at, updated_at";

                var content = await connection.QueryFirstAsync<LearningContentModel>(sql, new
                {
                    request.Title,
                    request.Description,
                    request.ContentType,
                    request.Category,
                    request.Difficulty,
                    request.ContentUrl,
                    request.ThumbnailUrl,
                    request.DurationMinutes,
                    request.IsActive,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                }, transaction);

                // Insert tags
                if (request.Tags.Any())
                {
                    await InsertContentTagsAsync(connection, transaction, content.Id, request.Tags);
                }

                // Initialize stats
                await InitializeContentStatsAsync(connection, transaction, content.Id);

                await transaction.CommitAsync();
                return content;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<LearningContentModel?> UpdateContentAsync(int id, UpdateLearningContentRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string sql = @"
                    UPDATE learning_content 
                    SET title = @Title, description = @Description, content_type = @ContentType,
                        category = @Category, difficulty = @Difficulty, content_url = @ContentUrl,
                        thumbnail_url = @ThumbnailUrl, duration_minutes = @DurationMinutes,
                        is_active = @IsActive, updated_at = @UpdatedAt
                    WHERE id = @Id
                    RETURNING id, title, description, content_type, category, difficulty, 
                             content_url, thumbnail_url, duration_minutes, is_active, 
                             created_by, created_at, updated_at";

                var content = await connection.QueryFirstOrDefaultAsync<LearningContentModel>(sql, new
                {
                    Id = id,
                    request.Title,
                    request.Description,
                    request.ContentType,
                    request.Category,
                    request.Difficulty,
                    request.ContentUrl,
                    request.ThumbnailUrl,
                    request.DurationMinutes,
                    request.IsActive,
                    UpdatedAt = DateTime.UtcNow
                }, transaction);

                if (content != null)
                {
                    // Update tags
                    await DeleteContentTagsAsync(connection, transaction, id);
                    if (request.Tags.Any())
                    {
                        await InsertContentTagsAsync(connection, transaction, id, request.Tags);
                    }
                }

                await transaction.CommitAsync();
                return content;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteContentAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Delete related data first
                await DeleteContentTagsAsync(connection, transaction, id);
                await connection.ExecuteAsync("DELETE FROM learning_comments WHERE content_id = @Id", new { Id = id }, transaction);
                await connection.ExecuteAsync("DELETE FROM learning_progress WHERE content_id = @Id", new { Id = id }, transaction);
                await connection.ExecuteAsync("DELETE FROM learning_content_stats WHERE content_id = @Id", new { Id = id }, transaction);

                const string sql = "DELETE FROM learning_content WHERE id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id }, transaction);
                
                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<LearningContentModel>> GetContentByCategoryAsync(string category)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT id, title, description, content_type, category, difficulty, 
                       content_url, thumbnail_url, duration_minutes, is_active, 
                       created_by, created_at, updated_at
                FROM learning_content 
                WHERE category = @Category AND is_active = true
                ORDER BY created_at DESC";

            return await connection.QueryAsync<LearningContentModel>(sql, new { Category = category });
        }

        public async Task<IEnumerable<LearningContentModel>> SearchContentAsync(string searchTerm)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT DISTINCT lc.id, lc.title, lc.description, lc.content_type, lc.category, 
                       lc.difficulty, lc.content_url, lc.thumbnail_url, lc.duration_minutes, 
                       lc.is_active, lc.created_by, lc.created_at, lc.updated_at
                FROM learning_content lc
                LEFT JOIN learning_content_tags lct ON lc.id = lct.content_id
                WHERE (
                    lc.title ILIKE @SearchTerm OR 
                    lc.description ILIKE @SearchTerm OR 
                    lc.category ILIKE @SearchTerm OR
                    lct.tag ILIKE @SearchTerm
                ) AND lc.is_active = true
                ORDER BY lc.created_at DESC";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<LearningContentModel>(sql, new { SearchTerm = searchPattern });
        }

        public async Task<LearningStatsResponse> GetStatsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string mainStatsSql = @"
                SELECT 
                    COUNT(*) as TotalContent,
                    COUNT(CASE WHEN is_active = true THEN 1 END) as ActiveContent,
                    (SELECT COUNT(DISTINCT user_id) FROM learning_progress) as TotalUsers,
                    (SELECT COUNT(DISTINCT user_id) FROM learning_progress WHERE last_accessed_at > CURRENT_DATE - INTERVAL '30 days') as ActiveUsers,
                    COALESCE(AVG(
                        CASE WHEN lcs.view_count > 0 
                        THEN (lcs.completion_count::decimal / lcs.view_count) * 100 
                        ELSE 0 END
                    ), 0) as AverageCompletionRate,
                    COALESCE(SUM(lp.time_spent_minutes), 0) as TotalViewTime
                FROM learning_content lc
                LEFT JOIN learning_content_stats lcs ON lc.id = lcs.content_id
                LEFT JOIN learning_progress lp ON lc.id = lp.content_id";

            const string categorySql = @"
                SELECT 
                    lc.category as Category,
                    COUNT(lc.id) as ContentCount,
                    COALESCE(SUM(lcs.view_count), 0) as ViewCount,
                    COALESCE(AVG(
                        CASE WHEN lcs.view_count > 0 
                        THEN (lcs.completion_count::decimal / lcs.view_count) * 100 
                        ELSE 0 END
                    ), 0) as CompletionRate
                FROM learning_content lc
                LEFT JOIN learning_content_stats lcs ON lc.id = lcs.content_id
                WHERE lc.is_active = true
                GROUP BY lc.category
                ORDER BY ContentCount DESC";

            const string popularSql = @"
                SELECT 
                    lc.id, lc.title,
                    COALESCE(lcs.view_count, 0) as ViewCount,
                    COALESCE(AVG(lcr.rating), 0) as Rating
                FROM learning_content lc
                LEFT JOIN learning_content_stats lcs ON lc.id = lcs.content_id
                LEFT JOIN learning_comments lcr ON lc.id = lcr.content_id
                WHERE lc.is_active = true
                GROUP BY lc.id, lc.title, lcs.view_count
                ORDER BY lcs.view_count DESC, Rating DESC
                LIMIT 5";

            var stats = await connection.QueryFirstAsync<LearningStatsResponse>(mainStatsSql);
            var categoryStats = await connection.QueryAsync<CategoryStats>(categorySql);
            var popularContent = await connection.QueryAsync<PopularContent>(popularSql);
            
            stats.CategoryBreakdown = categoryStats.ToList();
            stats.MostPopular = popularContent.ToList();
            
            return stats;
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT DISTINCT category 
                FROM learning_content 
                WHERE is_active = true AND category IS NOT NULL AND category != ''
                ORDER BY category";

            return await connection.QueryAsync<string>(sql);
        }

        public async Task<LearningProgressModel?> UpdateProgressAsync(int contentId, int userId, UpdateProgressRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string upsertSql = @"
                    INSERT INTO learning_progress (content_id, user_id, progress_percentage, is_completed, time_spent_minutes, last_accessed_at, completed_at)
                    VALUES (@ContentId, @UserId, @ProgressPercentage, @IsCompleted, @TimeSpentMinutes, @LastAccessedAt, @CompletedAt)
                    ON CONFLICT (content_id, user_id)
                    DO UPDATE SET 
                        progress_percentage = @ProgressPercentage,
                        is_completed = @IsCompleted,
                        time_spent_minutes = GREATEST(learning_progress.time_spent_minutes, @TimeSpentMinutes),
                        last_accessed_at = @LastAccessedAt,
                        completed_at = CASE WHEN @IsCompleted THEN COALESCE(learning_progress.completed_at, @CompletedAt) ELSE NULL END
                    RETURNING id, content_id, user_id, progress_percentage, is_completed, completed_at, time_spent_minutes, last_accessed_at";

                var now = DateTime.UtcNow;
                var progress = await connection.QueryFirstOrDefaultAsync<LearningProgressModel>(upsertSql, new
                {
                    ContentId = contentId,
                    UserId = userId,
                    request.ProgressPercentage,
                    request.IsCompleted,
                    request.TimeSpentMinutes,
                    LastAccessedAt = now,
                    CompletedAt = request.IsCompleted ? now : (DateTime?)null
                }, transaction);

                // Update completion count if newly completed
                if (request.IsCompleted)
                {
                    await UpdateCompletionCountAsync(connection, transaction, contentId);
                }

                await transaction.CommitAsync();
                return progress;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<LearningCommentModel> AddCommentAsync(int contentId, int userId, AddCommentRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO learning_comments (content_id, user_id, comment, rating, created_at)
                VALUES (@ContentId, @UserId, @Comment, @Rating, @CreatedAt)
                RETURNING id, content_id, user_id, comment, rating, created_at";

            return await connection.QueryFirstAsync<LearningCommentModel>(sql, new
            {
                ContentId = contentId,
                UserId = userId,
                request.Comment,
                request.Rating,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<LearningProgressModel>> GetUserProgressAsync(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT 
                    lp.id, lp.content_id, lp.user_id, lp.progress_percentage, 
                    lp.is_completed, lp.completed_at, lp.time_spent_minutes, lp.last_accessed_at,
                    lc.title as ContentTitle
                FROM learning_progress lp
                INNER JOIN learning_content lc ON lp.content_id = lc.id
                WHERE lp.user_id = @UserId
                ORDER BY lp.last_accessed_at DESC";

            return await connection.QueryAsync<LearningProgressModel>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<LearningContentModel>> GetRecommendedContentAsync(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT DISTINCT lc.id, lc.title, lc.description, lc.content_type, lc.category, 
                       lc.difficulty, lc.content_url, lc.thumbnail_url, lc.duration_minutes, 
                       lc.is_active, lc.created_by, lc.created_at, lc.updated_at
                FROM learning_content lc
                LEFT JOIN learning_progress lp ON lc.id = lp.content_id AND lp.user_id = @UserId
                LEFT JOIN learning_content_stats lcs ON lc.id = lcs.content_id
                WHERE lc.is_active = true 
                AND lp.content_id IS NULL  -- Not yet started by user
                ORDER BY lcs.view_count DESC, lcs.completion_count DESC
                LIMIT 10";

            return await connection.QueryAsync<LearningContentModel>(sql, new { UserId = userId });
        }

        // Helper methods
        private async Task<List<string>> GetContentTagsAsync(int contentId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "SELECT tag FROM learning_content_tags WHERE content_id = @ContentId";
            var tags = await connection.QueryAsync<string>(sql, new { ContentId = contentId });
            return tags.ToList();
        }

        private async Task<IEnumerable<LearningCommentModel>> GetContentCommentsAsync(int contentId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT lc.id, lc.content_id, lc.user_id, lc.comment, lc.rating, lc.created_at,
                       u.full_name as UserName
                FROM learning_comments lc
                INNER JOIN users u ON lc.user_id = u.id
                WHERE lc.content_id = @ContentId
                ORDER BY lc.created_at DESC";

            return await connection.QueryAsync<LearningCommentModel>(sql, new { ContentId = contentId });
        }

        private async Task InsertContentTagsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int contentId, List<string> tags)
        {
            const string sql = "INSERT INTO learning_content_tags (content_id, tag) VALUES (@ContentId, @Tag)";
            foreach (var tag in tags)
            {
                await connection.ExecuteAsync(sql, new { ContentId = contentId, Tag = tag }, transaction);
            }
        }

        private async Task DeleteContentTagsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int contentId)
        {
            const string sql = "DELETE FROM learning_content_tags WHERE content_id = @ContentId";
            await connection.ExecuteAsync(sql, new { ContentId = contentId }, transaction);
        }

        private async Task InitializeContentStatsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int contentId)
        {
            const string sql = @"
                INSERT INTO learning_content_stats (content_id, view_count, completion_count)
                VALUES (@ContentId, 0, 0)";
            
            await connection.ExecuteAsync(sql, new { ContentId = contentId }, transaction);
        }

        private async Task UpdateViewCountAsync(NpgsqlConnection connection, int contentId)
        {
            const string sql = @"
                UPDATE learning_content_stats 
                SET view_count = view_count + 1
                WHERE content_id = @ContentId";
            
            await connection.ExecuteAsync(sql, new { ContentId = contentId });
        }

        private async Task UpdateCompletionCountAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int contentId)
        {
            const string sql = @"
                UPDATE learning_content_stats 
                SET completion_count = completion_count + 1
                WHERE content_id = @ContentId";
            
            await connection.ExecuteAsync(sql, new { ContentId = contentId }, transaction);
        }
    }
}