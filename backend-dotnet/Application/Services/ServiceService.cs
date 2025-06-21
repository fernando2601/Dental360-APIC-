using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
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
            if (id <= 0)
                return null;

            return await _serviceRepository.GetByIdAsync(id);
        }

        public async Task<Service> CreateServiceAsync(Service service)
        {
            ValidateService(service);
            return await _serviceRepository.CreateAsync(service);
        }

        public async Task<Service?> UpdateServiceAsync(Service service)
        {
            if (service.Id <= 0)
                return null;

            ValidateService(service);
            return await _serviceRepository.UpdateAsync(service.Id, service);
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            if (id <= 0)
                return false;

            var existingService = await _serviceRepository.GetByIdAsync(id);
            if (existingService == null)
                return false;

            return await _serviceRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return Enumerable.Empty<Service>();

            // Implementação básica - retorna todos os serviços
            return await _serviceRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Service>> SearchServicesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Service>();

            return await _serviceRepository.SearchAsync(searchTerm);
        }

        public async Task<object> GetServiceStatsAsync()
        {
            // Implementação básica
            var services = await _serviceRepository.GetAllAsync();
            return new
            {
                totalServices = services.Count(),
                averagePrice = services.Any() ? services.Average(s => s.Price) : 0,
                totalCategories = services.Select(s => s.Category).Distinct().Count()
            };
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            // Implementação básica
            var services = await _serviceRepository.GetAllAsync();
            return services.Select(s => s.Category).Distinct();
        }

        private static void ValidateService(Service service)
        {
            if (string.IsNullOrWhiteSpace(service.Name))
                throw new ArgumentException("Nome do serviço é obrigatório");

            if (service.Price <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (service.Duration <= 0)
                throw new ArgumentException("Duração deve ser maior que zero");
        }
    }
}