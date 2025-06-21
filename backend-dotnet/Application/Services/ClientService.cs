using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            return await _clientRepository.CreateAsync(client);
        }

        public async Task<Client?> UpdateClientAsync(Client client)
        {
            return await _clientRepository.UpdateAsync(client.Id, client);
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            return await _clientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm)
        {
            return await _clientRepository.SearchAsync(searchTerm);
        }
    }
}