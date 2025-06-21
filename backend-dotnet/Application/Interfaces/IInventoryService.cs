using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetAllInventoryAsync();
        Task<Inventory?> GetInventoryByIdAsync(int id);
        Task<Inventory> CreateInventoryAsync(Inventory inventory);
        Task<Inventory> UpdateInventoryAsync(Inventory inventory);
        Task<bool> DeleteInventoryAsync(int id);
        Task<IEnumerable<Inventory>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Inventory>> GetByStatusAsync(string status);
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10);
        Task<bool> UpdateStockAsync(int id, int newQuantity);
    }
}