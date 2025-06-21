using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IServiceService
    {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByIdAsync(int id);
        Task<Service> CreateServiceAsync(Service service);
        Task<Service?> UpdateServiceAsync(int id, Service service);
        Task<bool> DeleteServiceAsync(int id);
        Task<IEnumerable<Service>> SearchServicesAsync(string searchTerm);
        Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category);
        Task<object> GetServiceStatsAsync();
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}