using ClinicApi.Models;
using ClinicApi.Repositories;

namespace ClinicApi.Services
{
    public class BeforeAfterService : IBeforeAfterService
    {
        private readonly IBeforeAfterRepository _beforeAfterRepository;
        private readonly ILogger<BeforeAfterService> _logger;

        public BeforeAfterService(
            IBeforeAfterRepository beforeAfterRepository,
            ILogger<BeforeAfterService> logger)
        {
            _beforeAfterRepository = beforeAfterRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<BeforeAfterResponse>> GetAllBeforeAfterAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all before/after cases");
                return await _beforeAfterRepository.GetAllBeforeAfterAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after cases");
                throw new ApplicationException("Erro ao buscar casos antes e depois", ex);
            }
        }

        public async Task<BeforeAfterModel?> GetBeforeAfterByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving before/after case with ID: {CaseId}", id);
                
                if (id <= 0)
                {
                    throw new ArgumentException("ID do caso deve ser maior que zero", nameof(id));
                }

                return await _beforeAfterRepository.GetBeforeAfterByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after case with ID: {CaseId}", id);
                throw new ApplicationException($"Erro ao buscar caso com ID {id}", ex);
            }
        }

        public async Task<BeforeAfterResponse> CreateBeforeAfterAsync(CreateBeforeAfterRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new before/after case: {Title}", request.Title);

                var (isValid, errorMessage) = await ValidateBeforeAfterAsync(request);
                if (!isValid)
                {
                    throw new ArgumentException(errorMessage);
                }

                return await _beforeAfterRepository.CreateBeforeAfterAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating before/after case: {Title}", request.Title);
                
                if (ex is ArgumentException)
                    throw;
                    
                throw new ApplicationException("Erro ao criar caso antes e depois", ex);
            }
        }

        public async Task<BeforeAfterResponse?> UpdateBeforeAfterAsync(int id, UpdateBeforeAfterRequest request)
        {
            try
            {
                _logger.LogInformation("Updating before/after case with ID: {CaseId}", id);

                if (id <= 0)
                {
                    throw new ArgumentException("ID do caso deve ser maior que zero", nameof(id));
                }

                var existingCase = await _beforeAfterRepository.GetBeforeAfterByIdAsync(id);
                if (existingCase == null)
                {
                    throw new KeyNotFoundException($"Caso com ID {id} não encontrado");
                }

                var (isValid, errorMessage) = await ValidateBeforeAfterAsync(request);
                if (!isValid)
                {
                    throw new ArgumentException(errorMessage);
                }

                return await _beforeAfterRepository.UpdateBeforeAfterAsync(id, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating before/after case with ID: {CaseId}", id);
                
                if (ex is ArgumentException || ex is KeyNotFoundException)
                    throw;
                    
                throw new ApplicationException($"Erro ao atualizar caso com ID {id}", ex);
            }
        }

        public async Task<bool> DeleteBeforeAfterAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting before/after case with ID: {CaseId}", id);

                if (id <= 0)
                {
                    throw new ArgumentException("ID do caso deve ser maior que zero", nameof(id));
                }

                var existingCase = await _beforeAfterRepository.GetBeforeAfterByIdAsync(id);
                if (existingCase == null)
                {
                    throw new KeyNotFoundException($"Caso com ID {id} não encontrado");
                }

                return await _beforeAfterRepository.DeleteBeforeAfterAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting before/after case with ID: {CaseId}", id);
                
                if (ex is ArgumentException || ex is KeyNotFoundException)
                    throw;
                    
                throw new ApplicationException($"Erro ao excluir caso com ID {id}", ex);
            }
        }

        public async Task<BeforeAfterStatsResponse> GetBeforeAfterStatsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving before/after statistics");
                return await _beforeAfterRepository.GetBeforeAfterStatsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after statistics");
                throw new ApplicationException("Erro ao buscar estatísticas", ex);
            }
        }

        public async Task<IEnumerable<BeforeAfterResponse>> GetPublicBeforeAfterAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving public before/after cases");
                return await _beforeAfterRepository.GetPublicBeforeAfterAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving public before/after cases");
                throw new ApplicationException("Erro ao buscar casos públicos", ex);
            }
        }

        public async Task<IEnumerable<BeforeAfterResponse>> GetBeforeAfterByTreatmentTypeAsync(string treatmentType)
        {
            try
            {
                _logger.LogInformation("Retrieving before/after cases by treatment type: {TreatmentType}", treatmentType);

                if (string.IsNullOrWhiteSpace(treatmentType))
                {
                    throw new ArgumentException("Tipo de tratamento não pode ser vazio", nameof(treatmentType));
                }

                return await _beforeAfterRepository.GetBeforeAfterByTreatmentTypeAsync(treatmentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving before/after cases by treatment type: {TreatmentType}", treatmentType);
                
                if (ex is ArgumentException)
                    throw;
                    
                throw new ApplicationException($"Erro ao buscar casos do tipo {treatmentType}", ex);
            }
        }

        public async Task<IEnumerable<BeforeAfterResponse>> SearchBeforeAfterAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching before/after cases with term: {SearchTerm}", searchTerm);

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllBeforeAfterAsync();
                }

                return await _beforeAfterRepository.SearchBeforeAfterAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching before/after cases with term: {SearchTerm}", searchTerm);
                throw new ApplicationException($"Erro ao buscar casos com termo '{searchTerm}'", ex);
            }
        }

        public async Task<IEnumerable<string>> GetTreatmentTypesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving treatment types");
                return await _beforeAfterRepository.GetTreatmentTypesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving treatment types");
                throw new ApplicationException("Erro ao buscar tipos de tratamento", ex);
            }
        }

        public async Task<bool> IncrementViewCountAsync(int id)
        {
            try
            {
                _logger.LogInformation("Incrementing view count for case: {CaseId}", id);

                if (id <= 0)
                {
                    throw new ArgumentException("ID do caso deve ser maior que zero", nameof(id));
                }

                return await _beforeAfterRepository.IncrementViewCountAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing view count for case: {CaseId}", id);
                
                if (ex is ArgumentException)
                    throw;
                    
                throw new ApplicationException("Erro ao incrementar visualizações", ex);
            }
        }

        public async Task<bool> AddRatingAsync(int beforeAfterId, CreateRatingRequest request)
        {
            try
            {
                _logger.LogInformation("Adding rating for case: {CaseId}", beforeAfterId);

                if (beforeAfterId <= 0)
                {
                    throw new ArgumentException("ID do caso deve ser maior que zero", nameof(beforeAfterId));
                }

                var caseExists = await _beforeAfterRepository.BeforeAfterExistsAsync(beforeAfterId);
                if (!caseExists)
                {
                    throw new KeyNotFoundException($"Caso com ID {beforeAfterId} não encontrado");
                }

                return await _beforeAfterRepository.AddRatingAsync(beforeAfterId, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding rating for case: {CaseId}", beforeAfterId);
                
                if (ex is ArgumentException || ex is KeyNotFoundException)
                    throw;
                    
                throw new ApplicationException("Erro ao adicionar avaliação", ex);
            }
        }

        public async Task<IEnumerable<BeforeAfterRatingModel>> GetRatingsAsync(int beforeAfterId)
        {
            try
            {
                _logger.LogInformation("Retrieving ratings for case: {CaseId}", beforeAfterId);

                if (beforeAfterId <= 0)
                {
                    throw new ArgumentException("ID do caso deve ser maior que zero", nameof(beforeAfterId));
                }

                return await _beforeAfterRepository.GetRatingsAsync(beforeAfterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for case: {CaseId}", beforeAfterId);
                
                if (ex is ArgumentException)
                    throw;
                    
                throw new ApplicationException("Erro ao buscar avaliações", ex);
            }
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateBeforeAfterAsync(CreateBeforeAfterRequest request)
        {
            // Validate image URLs
            if (!IsValidImageUrl(request.BeforeImageUrl))
            {
                return (false, "URL da imagem 'antes' inválida ou formato não suportado");
            }

            if (!IsValidImageUrl(request.AfterImageUrl))
            {
                return (false, "URL da imagem 'depois' inválida ou formato não suportado");
            }

            // Validate treatment date
            if (request.TreatmentDate > DateTime.UtcNow)
            {
                return (false, "Data do tratamento não pode ser no futuro");
            }

            if (request.TreatmentDate < DateTime.UtcNow.AddYears(-10))
            {
                return (false, "Data do tratamento não pode ser anterior a 10 anos");
            }

            // Validate patient age if provided
            if (!string.IsNullOrEmpty(request.PatientAge) && !IsValidAge(request.PatientAge))
            {
                return (false, "Idade do paciente inválida. Use formato: '25 anos' ou '25'");
            }

            // Validate patient gender if provided
            if (!string.IsNullOrEmpty(request.PatientGender) && !IsValidGender(request.PatientGender))
            {
                return (false, "Gênero deve ser: Masculino, Feminino ou Não informado");
            }

            return (true, string.Empty);
        }

        private static bool IsValidImageUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var result))
                return false;

            if (result.Scheme != Uri.UriSchemeHttp && result.Scheme != Uri.UriSchemeHttps)
                return false;

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(result.AbsolutePath).ToLowerInvariant();
            
            return validExtensions.Contains(extension);
        }

        private static bool IsValidAge(string age)
        {
            // Remove common suffixes and extract number
            var cleanAge = age.Replace("anos", "").Replace("ano", "").Trim();
            
            if (int.TryParse(cleanAge, out var ageNumber))
            {
                return ageNumber >= 0 && ageNumber <= 120;
            }

            return false;
        }

        private static bool IsValidGender(string gender)
        {
            var validGenders = new[] { "Masculino", "Feminino", "Não informado", "M", "F" };
            return validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
        }
    }
}