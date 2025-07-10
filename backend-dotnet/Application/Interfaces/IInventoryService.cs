using DentalSpa.Domain.Entities;
using DentalSpa.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryResponse?> GetByIdAsync(int id);
        Task<IEnumerable<InventoryResponse>> GetAllAsync();
        Task<InventoryResponse> CreateAsync(InventoryCreateRequest request);
        Task<InventoryResponse?> UpdateAsync(int id, InventoryCreateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<InventoryResponse>> GetByProductIdAsync(int productId);
        Task<IEnumerable<InventoryResponse>> GetByClinicIdAsync(int clinicId);
        Task<IEnumerable<Inventory>> GetByStatusAsync(string status);
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10);
        Task<bool> UpdateStockAsync(int id, int newQuantity);
        Task<bool> DeleteInventoryAsync(int id);
    }
}