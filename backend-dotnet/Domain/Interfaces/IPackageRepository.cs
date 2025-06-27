using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IProductRepository
    {
        // CRUD Básico
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);

        // Busca e Filtros
        Task<IEnumerable<Product>> SearchAsync(string searchTerm);

        // Contagem
        Task<int> GetCountAsync();
        Task<int> GetCountByCategoryAsync(string category);

        // Verificação
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}