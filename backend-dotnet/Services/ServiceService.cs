using ClinicApi.Models;
using ClinicApi.Repositories;

namespace ClinicApi.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _serviceRepository.GetAllAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _serviceRepository.GetByIdAsync(id);
        }

        public async Task<Service> CreateServiceAsync(CreateServiceDto serviceDto)
        {
            return await _serviceRepository.CreateAsync(serviceDto);
        }

        public async Task<Service?> UpdateServiceAsync(int id, CreateServiceDto serviceDto)
        {
            return await _serviceRepository.UpdateAsync(id, serviceDto);
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            return await _serviceRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Service>> SearchServicesAsync(string searchTerm)
        {
            return await _serviceRepository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category)
        {
            return await _serviceRepository.GetByCategoryAsync(category);
        }
    }
}