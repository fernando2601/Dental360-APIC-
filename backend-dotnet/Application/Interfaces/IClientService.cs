using DentalSpa.Domain.Entities;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientResponse>> GetAllAsync();
        Task<ClientResponse?> GetByIdAsync(int id);
        Task<ClientResponse> CreateAsync(ClientCreateRequest request);
        Task<ClientResponse?> UpdateAsync(int id, ClientCreateRequest request);
        Task<bool> DeleteAsync(int id);
        Task DeleteClientAsync(int id);
    }
}