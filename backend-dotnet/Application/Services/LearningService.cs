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

        public async Task<LearningArea> UpdateLearningAreaAsync(LearningArea learningArea)
        {
            try
            {
                learningArea.UpdatedAt = DateTime.UtcNow;
                return await _learningRepository.UpdateAsync(learningArea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar área de aprendizado: {Id}", learningArea.Id);
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
    }
}