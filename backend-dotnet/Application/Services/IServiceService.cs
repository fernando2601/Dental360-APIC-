using ClinicApi.Models;

namespace ClinicApi.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByIdAsync(int id);
        Task<Service> CreateServiceAsync(CreateServiceDto serviceDto);
        Task<Service?> UpdateServiceAsync(int id, CreateServiceDto serviceDto);
        Task<bool> DeleteServiceAsync(int id);
        Task<IEnumerable<Service>> SearchServicesAsync(string searchTerm);
        Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category);
    }
}