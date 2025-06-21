using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class LearningService : ILearningService
    {
        private readonly ILearningAreaRepository _learningRepository;
        private readonly ILogger<LearningService> _logger;

        public LearningService(ILearningAreaRepository learningRepository, ILogger<LearningService> logger)
        {
            _learningRepository = learningRepository;
            _logger = logger;
        }

        // Métodos genéricos (usados pelo controller)
        public async Task<IEnumerable<LearningArea>> GetAllAsync()
        {
            return await GetAllLearningAreasAsync();
        }

        public async Task<LearningArea?> GetByIdAsync(int id)
        {
            return await GetLearningAreaByIdAsync(id);
        }

        public async Task<LearningArea> CreateAsync(LearningArea learningArea)
        {
            return await CreateLearningAreaAsync(learningArea);
        }

        public async Task<LearningArea?> UpdateAsync(int id, LearningArea learningArea)
        {
            return await _learningRepository.UpdateAsync(id, learningArea);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await DeleteLearningAreaAsync(id);
        }

        // Métodos específicos
        public async Task<IEnumerable<LearningArea>> GetAllLearningAreasAsync()
        {
            try
            {
                return await _learningRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as áreas de aprendizado");
                throw;
            }
        }

        public async Task<LearningArea?> GetLearningAreaByIdAsync(int id)
        {
            try
            {
                return await _learningRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar área de aprendizado por ID: {Id}", id);
                throw;
            }
        }

        public async Task<LearningArea> CreateLearningAreaAsync(LearningArea learningArea)
        {
            try
            {
                learningArea.CreatedAt = DateTime.UtcNow;
                learningArea.UpdatedAt = DateTime.UtcNow;
                return await _learningRepository.CreateAsync(learningArea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar área de aprendizado");
                throw;
            }
        }

        public async Task<bool> DeleteLearningAreaAsync(int id)
        {
            try
            {
                return await _learningRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar área de aprendizado: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<LearningArea>> GetLearningAreasByCategoryAsync(string category)
        {
            try
            {
                return await _learningRepository.GetByCategoryAsync(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar áreas de aprendizado por categoria: {Category}", category);
                throw;
            }
        }

        public async Task<IEnumerable<LearningArea>> GetActiveLearningAreasAsync()
        {
            try
            {
                return await _learningRepository.GetActiveAreasAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar áreas de aprendizado ativas");
                throw;
            }
        }

        public async Task<IEnumerable<LearningArea>> SearchLearningAreasByTitleAsync(string searchTerm)
        {
            try
            {
                return await _learningRepository.SearchByTitleAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar áreas de aprendizado por título: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<LearningArea> UpdateLearningAreaAsync(LearningArea learningArea)
        {
            try
            {
                learningArea.UpdatedAt = DateTime.UtcNow;
                return await _learningRepository.UpdateAsync(learningArea.Id, learningArea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar área de aprendizado: {Id}", learningArea.Id);
                throw;
            }
        }
    }
}