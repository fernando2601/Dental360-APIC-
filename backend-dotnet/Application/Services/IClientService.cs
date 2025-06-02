using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Services
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(int id);
        Task<Client> CreateClientAsync(CreateClientDto clientDto);
        Task<Client?> UpdateClientAsync(int id, CreateClientDto clientDto);
        Task<bool> DeleteClientAsync(int id);
        Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm);
    }
}