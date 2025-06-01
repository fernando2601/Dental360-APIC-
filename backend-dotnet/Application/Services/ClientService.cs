using DentalSpa.Application.DTOs;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;

namespace DentalSpa.Application.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDTO>> GetAllClientsAsync();
        Task<ClientDTO> GetClientByIdAsync(int id);
        Task<ClientDTO> CreateClientAsync(ClientCreateRequest request);
        Task<ClientDTO> UpdateClientAsync(int id, ClientUpdateRequest request);
        Task DeleteClientAsync(int id);
        Task<IEnumerable<ClientDTO>> SearchClientsAsync(string searchTerm);
    }

    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<ClientDTO>> GetAllClientsAsync()
        {
            var clients = await _clientRepository.GetAllAsync();
            return clients.Select(MapToClientDTO);
        }

        public async Task<ClientDTO> GetClientByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return MapToClientDTO(client);
        }

        public async Task<ClientDTO> CreateClientAsync(ClientCreateRequest request)
        {
            var existingClient = await _clientRepository.GetByEmailAsync(request.Email);
            if (existingClient != null)
            {
                throw new InvalidOperationException("Email já está em uso");
            }

            var client = new Client
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Birthday = request.Birthday,
                Notes = request.Notes
            };

            await _clientRepository.AddAsync(client);
            return MapToClientDTO(client);
        }

        public async Task<ClientDTO> UpdateClientAsync(int id, ClientUpdateRequest request)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            
            var existingClient = await _clientRepository.GetByEmailAsync(request.Email);
            if (existingClient != null && existingClient.Id != id)
            {
                throw new InvalidOperationException("Email já está em uso");
            }

            client.FullName = request.FullName;
            client.Email = request.Email;
            client.Phone = request.Phone;
            client.Address = request.Address;
            client.Birthday = request.Birthday;
            client.Notes = request.Notes;

            await _clientRepository.UpdateAsync(client);
            return MapToClientDTO(client);
        }

        public async Task DeleteClientAsync(int id)
        {
            await _clientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ClientDTO>> SearchClientsAsync(string searchTerm)
        {
            var clients = await _clientRepository.SearchAsync(searchTerm);
            return clients.Select(MapToClientDTO);
        }

        private static ClientDTO MapToClientDTO(Client client)
        {
            return new ClientDTO
            {
                Id = client.Id,
                FullName = client.FullName,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address,
                Birthday = client.Birthday,
                Notes = client.Notes,
                CreatedAt = client.CreatedAt,
                Age = client.GetAge(),
                IsMinor = client.IsMinor()
            };
        }
    }
}