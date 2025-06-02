using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;

namespace DentalSpa.Application.Services
{
    public interface ILearningService
    {
        Task<IEnumerable<LearningContentResponse>> GetAllContentAsync(int? userId = null);
        Task<LearningContentResponse?> GetContentByIdAsync(int id, int? userId = null);
        Task<LearningContentResponse> CreateContentAsync(CreateLearningContentRequest request, int createdBy);
        Task<LearningContentResponse?> UpdateContentAsync(int id, UpdateLearningContentRequest request);
        Task<bool> DeleteContentAsync(int id);
        Task<IEnumerable<LearningContentResponse>> GetContentByCategoryAsync(string category);
        Task<IEnumerable<LearningContentResponse>> SearchContentAsync(string searchTerm);
        Task<LearningStatsResponse> GetStatsAsync();
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<LearningProgressModel?> UpdateProgressAsync(int contentId, int userId, UpdateProgressRequest request);
        Task<LearningCommentModel> AddCommentAsync(int contentId, int userId, AddCommentRequest request);
        Task<IEnumerable<LearningProgressModel>> GetUserProgressAsync(int userId);
        Task<IEnumerable<LearningContentResponse>> GetRecommendedContentAsync(int userId);
    }

    public class LearningService : ILearningService
    {
        private readonly ILearningRepository _learningRepository;
        private readonly ILogger<LearningService> _logger;

        public LearningService(ILearningRepository learningRepository, ILogger<LearningService> logger)
        {
            _learningRepository = learningRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<LearningContentResponse>> GetAllContentAsync(int? userId = null)
        {
            try
            {
                var content = await _learningRepository.GetAllContentAsync(userId);
                return content.Select(MapToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo de aprendizado");
                throw;
            }
        }

        public async Task<LearningContentResponse?> GetContentByIdAsync(int id, int? userId = null)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido fornecido: {Id}", id);
                return null;
            }

            try
            {
                var content = await _learningRepository.GetContentByIdAsync(id, userId);
                return content != null ? MapToResponse(content) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo com ID {Id}", id);
                throw;
            }
        }

        public async Task<LearningContentResponse> CreateContentAsync(CreateLearningContentRequest request, int createdBy)
        {
            ValidateContentRequest(request);

            try
            {
                var content = await _learningRepository.CreateContentAsync(request, createdBy);
                var detailedContent = await _learningRepository.GetContentByIdAsync(content.Id);
                return MapToResponse(detailedContent!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conteúdo");
                throw;
            }
        }

        public async Task<LearningContentResponse?> UpdateContentAsync(int id, UpdateLearningContentRequest request)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido fornecido para atualização: {Id}", id);
                return null;
            }

            ValidateContentRequest(request);

            try
            {
                var existingContent = await _learningRepository.GetContentByIdAsync(id);
                if (existingContent == null)
                {
                    _logger.LogWarning("Conteúdo com ID {Id} não encontrado para atualização", id);
                    return null;
                }

                var updatedContent = await _learningRepository.UpdateContentAsync(id, request);
                if (updatedContent == null)
                {
                    return null;
                }

                var detailedContent = await _learningRepository.GetContentByIdAsync(updatedContent.Id);
                return MapToResponse(detailedContent!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar conteúdo com ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteContentAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido fornecido para exclusão: {Id}", id);
                return false;
            }

            try
            {
                var existingContent = await _learningRepository.GetContentByIdAsync(id);
                if (existingContent == null)
                {
                    _logger.LogWarning("Conteúdo com ID {Id} não encontrado para exclusão", id);
                    return false;
                }

                return await _learningRepository.DeleteContentAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir conteúdo com ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<LearningContentResponse>> GetContentByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                _logger.LogWarning("Categoria vazia fornecida");
                return Enumerable.Empty<LearningContentResponse>();
            }

            try
            {
                var content = await _learningRepository.GetContentByCategoryAsync(category);
                return content.Select(MapToBasicResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo da categoria {Category}", category);
                throw;
            }
        }

        public async Task<IEnumerable<LearningContentResponse>> SearchContentAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.LogWarning("Termo de busca vazio fornecido");
                return Enumerable.Empty<LearningContentResponse>();
            }

            try
            {
                var content = await _learningRepository.SearchContentAsync(searchTerm);
                return content.Select(MapToBasicResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo com termo {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<LearningStatsResponse> GetStatsAsync()
        {
            try
            {
                return await _learningRepository.GetStatsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas de aprendizado");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            try
            {
                return await _learningRepository.GetCategoriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias");
                throw;
            }
        }

        public async Task<LearningProgressModel?> UpdateProgressAsync(int contentId, int userId, UpdateProgressRequest request)
        {
            if (contentId <= 0 || userId <= 0)
            {
                _logger.LogWarning("IDs inválidos fornecidos - ContentId: {ContentId}, UserId: {UserId}", contentId, userId);
                return null;
            }

            ValidateProgressRequest(request);

            try
            {
                return await _learningRepository.UpdateProgressAsync(contentId, userId, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar progresso do usuário {UserId} no conteúdo {ContentId}", userId, contentId);
                throw;
            }
        }

        public async Task<LearningCommentModel> AddCommentAsync(int contentId, int userId, AddCommentRequest request)
        {
            if (contentId <= 0 || userId <= 0)
            {
                throw new ArgumentException("IDs de conteúdo e usuário devem ser válidos");
            }

            ValidateCommentRequest(request);

            try
            {
                return await _learningRepository.AddCommentAsync(contentId, userId, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar comentário do usuário {UserId} no conteúdo {ContentId}", userId, contentId);
                throw;
            }
        }

        public async Task<IEnumerable<LearningProgressModel>> GetUserProgressAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("ID de usuário inválido: {UserId}", userId);
                return Enumerable.Empty<LearningProgressModel>();
            }

            try
            {
                return await _learningRepository.GetUserProgressAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar progresso do usuário {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<LearningContentResponse>> GetRecommendedContentAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("ID de usuário inválido: {UserId}", userId);
                return Enumerable.Empty<LearningContentResponse>();
            }

            try
            {
                var content = await _learningRepository.GetRecommendedContentAsync(userId);
                return content.Select(MapToBasicResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar conteúdo recomendado para usuário {UserId}", userId);
                throw;
            }
        }

        // Mapping methods
        private static LearningContentResponse MapToResponse(LearningContentDetailedModel content)
        {
            return new LearningContentResponse
            {
                Id = content.Id,
                Title = content.Title,
                Description = content.Description,
                ContentType = content.ContentType,
                Category = content.Category,
                Difficulty = content.Difficulty,
                ContentUrl = content.ContentUrl,
                ThumbnailUrl = content.ThumbnailUrl,
                DurationMinutes = content.DurationMinutes,
                IsActive = content.IsActive,
                CreatedBy = content.CreatedBy,
                CreatedByName = content.CreatedByName,
                CreatedAt = content.CreatedAt,
                UpdatedAt = content.UpdatedAt,
                Tags = content.Tags,
                Rating = content.Rating,
                TotalRatings = content.TotalRatings,
                ViewCount = content.ViewCount,
                CompletionCount = content.CompletionCount
            };
        }

        private static LearningContentResponse MapToBasicResponse(LearningContentModel content)
        {
            return new LearningContentResponse
            {
                Id = content.Id,
                Title = content.Title,
                Description = content.Description,
                ContentType = content.ContentType,
                Category = content.Category,
                Difficulty = content.Difficulty,
                ContentUrl = content.ContentUrl,
                ThumbnailUrl = content.ThumbnailUrl,
                DurationMinutes = content.DurationMinutes,
                IsActive = content.IsActive,
                CreatedBy = content.CreatedBy,
                CreatedAt = content.CreatedAt,
                UpdatedAt = content.UpdatedAt,
                Tags = content.Tags,
                Rating = content.Rating,
                TotalRatings = content.TotalRatings,
                ViewCount = content.ViewCount,
                CompletionCount = content.CompletionCount
            };
        }

        // Validation methods
        private static void ValidateContentRequest(CreateLearningContentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Título é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Descrição é obrigatória");

            if (string.IsNullOrWhiteSpace(request.ContentType))
                throw new ArgumentException("Tipo de conteúdo é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Category))
                throw new ArgumentException("Categoria é obrigatória");

            if (string.IsNullOrWhiteSpace(request.Difficulty))
                throw new ArgumentException("Dificuldade é obrigatória");

            if (string.IsNullOrWhiteSpace(request.ContentUrl))
                throw new ArgumentException("URL do conteúdo é obrigatória");

            if (request.DurationMinutes <= 0)
                throw new ArgumentException("Duração deve ser maior que zero");

            if (request.DurationMinutes > 600)
                throw new ArgumentException("Duração não pode ser maior que 10 horas");

            var validContentTypes = new[] { "video", "article", "course", "quiz", "document" };
            if (!validContentTypes.Contains(request.ContentType.ToLower()))
                throw new ArgumentException("Tipo de conteúdo inválido");

            var validDifficulties = new[] { "beginner", "intermediate", "advanced" };
            if (!validDifficulties.Contains(request.Difficulty.ToLower()))
                throw new ArgumentException("Nível de dificuldade inválido");

            if (!IsValidUrl(request.ContentUrl))
                throw new ArgumentException("URL do conteúdo inválida");

            if (!string.IsNullOrWhiteSpace(request.ThumbnailUrl) && !IsValidUrl(request.ThumbnailUrl))
                throw new ArgumentException("URL da thumbnail inválida");
        }

        private static void ValidateProgressRequest(UpdateProgressRequest request)
        {
            if (request.ProgressPercentage < 0 || request.ProgressPercentage > 100)
                throw new ArgumentException("Progresso deve estar entre 0 e 100");

            if (request.TimeSpentMinutes < 0)
                throw new ArgumentException("Tempo gasto deve ser maior ou igual a zero");
        }

        private static void ValidateCommentRequest(AddCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Comment))
                throw new ArgumentException("Comentário é obrigatório");

            if (request.Rating < 1 || request.Rating > 5)
                throw new ArgumentException("Avaliação deve estar entre 1 e 5");
        }

        private static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var validatedUri) &&
                   (validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}