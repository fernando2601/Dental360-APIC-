using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<ServiceResponse>> GetAllAsync()
        {
            var services = await _serviceRepository.GetAllAsync();
            return services.Select((Service s) => MapToResponse(s));
        }

        public async Task<ServiceResponse?> GetByIdAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            return service == null ? null : MapToResponse(service);
        }

        public async Task<ServiceResponse> CreateAsync(ServiceCreateRequest request)
        {
            var service = new Service
            {
                Name = request.Name,
                Category = request.Category,
                Description = request.Description,
                Price = request.Price,
                Duration = request.Duration,
                IsActive = request.IsActive,
                ClinicId = request.ClinicId
            };
            var created = await _serviceRepository.CreateAsync(service);
            // Salvar relação N:N com Staff
            await _serviceRepository.SetServiceStaffAsync(created.Id, request.StaffIds);
            return MapToResponse(created, request.StaffIds);
        }

        public async Task<ServiceResponse?> UpdateAsync(int id, ServiceCreateRequest request)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return null;
            service.Name = request.Name;
            service.Category = request.Category;
            service.Description = request.Description;
            service.Price = request.Price;
            service.Duration = request.Duration;
            service.IsActive = request.IsActive;
            service.ClinicId = request.ClinicId;
            var updated = await _serviceRepository.UpdateAsync(id, service);
            await _serviceRepository.SetServiceStaffAsync(id, request.StaffIds);
            return MapToResponse(updated, request.StaffIds);
        }

        public async Task<bool> DeleteAsync(int id)
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
            return services.Select((Service s) => s.Category).Distinct();
        }

        public async Task<bool> DeleteServiceAsync(int id) => await DeleteAsync(id);
        public async Task SetServiceStaffAsync(int serviceId, List<int> staffIds) => await _serviceRepository.SetServiceStaffAsync(serviceId, staffIds);

        private static void ValidateService(Service service)
        {
            if (string.IsNullOrWhiteSpace(service.Name))
                throw new ArgumentException("Nome do serviço é obrigatório");

            if (service.Price <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (service.Duration <= 0)
                throw new ArgumentException("Duração deve ser maior que zero");
        }

        private ServiceResponse MapToResponse(Service s, List<int>? staffIds = null)
        {
            return new ServiceResponse
            {
                Name = s.Name,
                Category = s.Category,
                Description = s.Description,
                Price = s.Price,
                Duration = s.Duration,
                IsActive = s.IsActive,
                ClinicId = s.ClinicId,
                StaffIds = staffIds ?? s.StaffServices.Select(ss => ss.StaffId).ToList(),
                CreatedAt = s.CreatedAt
            };
        }
    }
}