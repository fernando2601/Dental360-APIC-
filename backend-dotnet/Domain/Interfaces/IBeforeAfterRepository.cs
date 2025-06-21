using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IBeforeAfterRepository
    {
        Task<IEnumerable<BeforeAfter>> GetAllAsync();
        Task<BeforeAfter?> GetByIdAsync(int id);
        Task<BeforeAfter> CreateAsync(BeforeAfter beforeAfter);
        Task<BeforeAfter?> UpdateAsync(int id, BeforeAfter beforeAfter);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<BeforeAfter>> GetPublicAsync();
        Task<IEnumerable<BeforeAfter>> GetByServiceAsync(int serviceId);
        Task<IEnumerable<BeforeAfter>> SearchAsync(string searchTerm);
        Task<bool> ExistsAsync(int id);
    }
}