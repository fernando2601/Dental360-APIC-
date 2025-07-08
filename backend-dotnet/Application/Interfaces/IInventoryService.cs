using DentalSpa.Domain.Entities;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryResponse>> GetAllAsync();
        Task<InventoryResponse?> GetByIdAsync(int id);
        Task<InventoryResponse> CreateAsync(InventoryCreateRequest request);
        Task<InventoryResponse?> UpdateAsync(int id, InventoryCreateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<InventoryResponse>> SearchAsync(string searchTerm);
        Task<IEnumerable<Inventory>> GetByStatusAsync(string status);
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10);
        Task<bool> UpdateStockAsync(int id, int newQuantity);
    }
}