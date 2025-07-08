using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        // CRUD Básico
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory?> GetByIdAsync(int id);
        Task<Inventory> CreateAsync(Inventory inventory);
        Task<Inventory?> UpdateAsync(int id, Inventory inventory);
        Task<bool> DeleteAsync(int id);

        // Busca e Filtros
        Task<IEnumerable<Inventory>> GetByCategoryAsync(string category);
        Task<IEnumerable<Inventory>> GetLowStockAsync();
        Task<IEnumerable<Inventory>> GetExpiredAsync();

        // Contagem
        Task<int> GetCountAsync();
        Task<int> GetCountByCategoryAsync(string category);
        Task<int> GetLowStockCountAsync();

        // Verificação
        Task<bool> ExistsAsync(int id);
    }
}