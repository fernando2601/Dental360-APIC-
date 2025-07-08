using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSpa.Application.DTOs;
using System.Linq;

namespace DentalSpa.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return await _clientRepository.CreateAsync(client);
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task<Client?> UpdateClientAsync(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return await _clientRepository.UpdateAsync(client.Id, client);
        }

        public async Task DeleteClientAsync(int id) => await _clientRepository.DeleteAsync(id);

        public async Task<IEnumerable<ClientResponse>> SearchClientsAsync(string searchTerm)
        {
            var clients = await _clientRepository.SearchAsync(searchTerm);
            return clients.Select(MapToResponse);
        }

        public async Task<ClientResponse> CreateAsync(ClientCreateRequest request)
        {
            var client = new Client
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Birthday = request.Birthday,
                Notes = request.Notes,
                CreatedAt = request.CreatedAt,
                ClinicId = request.ClinicId
            };
            var created = await _clientRepository.CreateAsync(client);
            return MapToResponse(created);
        }

        public async Task<ClientResponse?> UpdateAsync(int id, ClientCreateRequest request)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null) return null;
            client.FullName = request.FullName;
            client.Email = request.Email;
            client.Phone = request.Phone;
            client.Address = request.Address;
            client.Birthday = request.Birthday;
            client.Notes = request.Notes;
            client.CreatedAt = request.CreatedAt;
            client.ClinicId = request.ClinicId;
            var updated = await _clientRepository.UpdateAsync(id, client);
            return MapToResponse(updated);
        }

        public async Task<IEnumerable<ClientResponse>> GetAllAsync()
        {
            var clients = await _clientRepository.GetAllAsync();
            return clients.Select(MapToResponse);
        }

        public async Task<ClientResponse?> GetByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return client == null ? null : MapToResponse(client);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _clientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ClientResponse>> SearchAsync(string searchTerm)
        {
            var clients = await _clientRepository.SearchAsync(searchTerm);
            return clients.Select(MapToResponse);
        }

        private ClientResponse MapToResponse(Client client)
        {
            return new ClientResponse
            {
                FullName = client.FullName,
                Email = client.Email,
                Phone = client.Phone,
                Address = client.Address,
                Birthday = client.Birthday,
                Notes = client.Notes,
                CreatedAt = client.CreatedAt,
                ClinicId = client.ClinicId
            };
        }
    }
}