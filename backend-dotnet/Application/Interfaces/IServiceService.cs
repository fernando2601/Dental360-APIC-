using DentalSpa.Domain.Entities;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Interfaces
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceResponse>> GetAllAsync();
        Task<ServiceResponse?> GetByIdAsync(int id);
        Task<ServiceResponse> CreateAsync(ServiceCreateRequest request);
        Task<ServiceResponse?> UpdateAsync(int id, ServiceCreateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ServiceResponse>> SearchAsync(string searchTerm);
        Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category);
        Task<object> GetServiceStatsAsync();
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<bool> DeleteServiceAsync(int id);
        Task SetServiceStaffAsync(int serviceId, List<int> staffIds);
    }
}