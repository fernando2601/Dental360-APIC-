using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface ILearningService
    {
        Task<IEnumerable<LearningArea>> GetAllLearningAreasAsync();
        Task<LearningArea?> GetLearningAreaByIdAsync(int id);
        Task<LearningArea> CreateLearningAreaAsync(LearningArea learningArea);
        Task<LearningArea> UpdateLearningAreaAsync(LearningArea learningArea);
        Task<bool> DeleteLearningAreaAsync(int id);
        Task<IEnumerable<LearningArea>> GetLearningAreasByCategoryAsync(string category);
        Task<IEnumerable<LearningArea>> GetActiveLearningAreasAsync();
        Task<IEnumerable<LearningArea>> SearchLearningAreasByTitleAsync(string searchTerm);
    }
}