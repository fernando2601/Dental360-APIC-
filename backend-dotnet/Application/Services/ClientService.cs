using AutoMapper;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.DTOs;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task<Client> CreateClientAsync(CreateClientDto clientDto)
        {
            // ANTES (sem AutoMapper): Conversão manual
            // var client = new Client 
            // {
            //     Name = clientDto.Name,
            //     Email = clientDto.Email,
            //     Phone = clientDto.Phone,
            //     // ... mais 10+ propriedades manuais
            // };

            // AGORA (com AutoMapper): Conversão automática
            var client = _mapper.Map<Client>(clientDto);
            return await _clientRepository.CreateAsync(client);
        }

        // Exemplo de método que retorna DTOs automaticamente
        public async Task<IEnumerable<ClientDto>> GetAllClientsAsDtoAsync()
        {
            var clients = await _clientRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
        }

        public async Task<ClientDto?> GetClientDtoByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return client != null ? _mapper.Map<ClientDto>(client) : null;
        }

        public async Task<Client?> UpdateClientAsync(int id, CreateClientDto clientDto)
        {
            return await _clientRepository.UpdateAsync(id, clientDto);
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