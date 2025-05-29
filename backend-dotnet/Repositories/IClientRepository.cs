using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<Client> CreateAsync(CreateClientDto client);
        Task<Client?> UpdateAsync(int id, CreateClientDto client);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Client>> SearchAsync(string searchTerm);
    }
}