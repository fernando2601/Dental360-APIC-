using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IBeforeAfterRepository
    {
        Task<IEnumerable<BeforeAfterResponse>> GetAllBeforeAfterAsync();
        Task<BeforeAfterModel?> GetBeforeAfterByIdAsync(int id);
        Task<BeforeAfterResponse> CreateBeforeAfterAsync(CreateBeforeAfterRequest request);
        Task<BeforeAfterResponse?> UpdateBeforeAfterAsync(int id, UpdateBeforeAfterRequest request);
        Task<bool> DeleteBeforeAfterAsync(int id);
        Task<BeforeAfterStatsResponse> GetBeforeAfterStatsAsync();
        Task<IEnumerable<BeforeAfterResponse>> GetPublicBeforeAfterAsync();
        Task<IEnumerable<BeforeAfterResponse>> GetBeforeAfterByTreatmentTypeAsync(string treatmentType);
        Task<IEnumerable<BeforeAfterResponse>> SearchBeforeAfterAsync(string searchTerm);
        Task<IEnumerable<string>> GetTreatmentTypesAsync();
        Task<bool> IncrementViewCountAsync(int id);
        Task<bool> AddRatingAsync(int beforeAfterId, CreateRatingRequest request);
        Task<IEnumerable<BeforeAfterRatingModel>> GetRatingsAsync(int beforeAfterId);
        Task<bool> BeforeAfterExistsAsync(int id);
    }
}