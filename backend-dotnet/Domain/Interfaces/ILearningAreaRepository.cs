using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface ILearningAreaRepository
    {
        Task<IEnumerable<LearningArea>> GetAllAsync();
        Task<LearningArea?> GetByIdAsync(int id);
        Task<LearningArea> CreateAsync(LearningArea learningArea);
        Task<LearningArea> UpdateAsync(LearningArea learningArea);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<LearningArea>> GetByCategoryAsync(string category);
        Task<IEnumerable<LearningArea>> GetActiveAreasAsync();
        Task<IEnumerable<LearningArea>> SearchByTitleAsync(string searchTerm);
    }
}