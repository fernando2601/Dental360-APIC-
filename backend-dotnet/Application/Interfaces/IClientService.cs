using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(int id);
        Task<Client> CreateClientAsync(Client client);
        Task<Client?> UpdateClientAsync(Client client);
        Task<bool> DeleteClientAsync(int id);
        Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm);
    }
}