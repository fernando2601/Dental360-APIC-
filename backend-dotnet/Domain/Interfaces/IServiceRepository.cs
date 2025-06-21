using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllAsync();
        Task<Service?> GetByIdAsync(int id);
        Task<Service> CreateAsync(Service service);
        Task<Service?> UpdateAsync(int id, Service service);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Service>> SearchAsync(string searchTerm);
        Task<IEnumerable<Service>> GetByCategoryAsync(string category);
    }
}