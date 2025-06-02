using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;

namespace DentalSpa.Application.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceResponse>> GetAllServicesAsync();
        Task<ServiceResponse?> GetServiceByIdAsync(int id);
        Task<ServiceResponse> CreateServiceAsync(CreateServiceRequest request);
        Task<ServiceResponse?> UpdateServiceAsync(int id, UpdateServiceRequest request);
        Task<bool> DeleteServiceAsync(int id);
        Task<IEnumerable<ServiceResponse>> GetServicesByCategoryAsync(string category);
        Task<ServiceStatsResponse> GetServiceStatsAsync();
        Task<IEnumerable<string>> GetCategoriesAsync();
    }

    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<ServiceResponse>> GetAllServicesAsync()
        {
            var services = await _serviceRepository.GetAllAsync();
            return services.Select(MapToResponse);
        }

        public async Task<ServiceResponse?> GetServiceByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var service = await _serviceRepository.GetByIdAsync(id);
            return service != null ? MapToResponse(service) : null;
        }

        public async Task<ServiceResponse> CreateServiceAsync(CreateServiceRequest request)
        {
            ValidateServiceRequest(request);

            var service = await _serviceRepository.CreateAsync(request);
            return MapToResponse(service);
        }

        public async Task<ServiceResponse?> UpdateServiceAsync(int id, UpdateServiceRequest request)
        {
            if (id <= 0)
                return null;

            ValidateServiceRequest(request);

            var existingService = await _serviceRepository.GetByIdAsync(id);
            if (existingService == null)
                return null;

            var updatedService = await _serviceRepository.UpdateAsync(id, request);
            return updatedService != null ? MapToResponse(updatedService) : null;
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

        public async Task<IEnumerable<ServiceResponse>> GetServicesByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return Enumerable.Empty<ServiceResponse>();

            var services = await _serviceRepository.GetByCategoryAsync(category);
            return services.Select(MapToResponse);
        }

        public async Task<ServiceStatsResponse> GetServiceStatsAsync()
        {
            return await _serviceRepository.GetStatsAsync();
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _serviceRepository.GetCategoriesAsync();
        }

        private static ServiceResponse MapToResponse(ServiceModel service)
        {
            return new ServiceResponse
            {
                Id = service.Id,
                Name = service.Name,
                Category = service.Category,
                Description = service.Description,
                Price = service.Price,
                DurationMinutes = service.DurationMinutes,
                IsActive = service.IsActive,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt
            };
        }

        private static void ValidateServiceRequest(CreateServiceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Nome do serviço é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Category))
                throw new ArgumentException("Categoria é obrigatória");

            if (request.Price <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (request.DurationMinutes <= 0)
                throw new ArgumentException("Duração deve ser maior que zero");

            if (request.DurationMinutes > 480)
                throw new ArgumentException("Duração não pode ser maior que 8 horas");
        }

        private static void ValidateServiceRequest(UpdateServiceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Nome do serviço é obrigatório");

            if (string.IsNullOrWhiteSpace(request.Category))
                throw new ArgumentException("Categoria é obrigatória");

            if (request.Price <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (request.DurationMinutes <= 0)
                throw new ArgumentException("Duração deve ser maior que zero");

            if (request.DurationMinutes > 480)
                throw new ArgumentException("Duração não pode ser maior que 8 horas");
        }
    }
}