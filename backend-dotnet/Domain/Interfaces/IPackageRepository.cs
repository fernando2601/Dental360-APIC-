using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IPackageRepository
    {
        // CRUD Básico
        Task<IEnumerable<Package>> GetAllAsync();
        Task<Package?> GetByIdAsync(int id);
        Task<Package> CreateAsync(Package package);
        Task<Package?> UpdateAsync(int id, Package package);
        Task<bool> DeleteAsync(int id);

        // Busca e Filtros
        Task<IEnumerable<Package>> SearchAsync(string searchTerm);
        Task<IEnumerable<Package>> GetByCategoryAsync(string category);
        Task<IEnumerable<string>> GetCategoriesAsync();

        // Contagem
        Task<int> GetCountAsync();
        Task<int> GetCountByCategoryAsync(string category);

        // Verificação
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}