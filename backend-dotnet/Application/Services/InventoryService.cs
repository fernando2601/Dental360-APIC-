using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(IInventoryRepository inventoryRepository, ILogger<InventoryService> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Inventory>> GetAllInventoryAsync()
        {
            try
            {
                return await _inventoryRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os itens do inventário");
                throw;
            }
        }

        public async Task<Inventory?> GetInventoryByIdAsync(int id)
        {
            try
            {
                return await _inventoryRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar item do inventário por ID: {Id}", id);
                throw;
            }
        }

        public async Task<InventoryResponse> CreateAsync(InventoryCreateRequest request)
        {
            var item = new Inventory
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Quantity = request.Quantity,
                Unit = request.Unit,
                MinStock = request.MinStock,
                UnitPrice = request.UnitPrice,
                Supplier = request.Supplier,
                Location = request.Location,
                BatchNumber = request.BatchNumber,
                ExpirationDate = request.ExpirationDate,
                Status = request.Status,
                IsActive = request.IsActive,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                ClinicId = request.ClinicId
            };
            var created = await _inventoryRepository.CreateAsync(item);
            return MapToResponse(created);
        }

        public async Task<InventoryResponse?> UpdateAsync(int id, InventoryCreateRequest request)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null) return null;
            item.Name = request.Name;
            item.Description = request.Description;
            item.Category = request.Category;
            item.Quantity = request.Quantity;
            item.Unit = request.Unit;
            item.MinStock = request.MinStock;
            item.UnitPrice = request.UnitPrice;
            item.Supplier = request.Supplier;
            item.Location = request.Location;
            item.BatchNumber = request.BatchNumber;
            item.ExpirationDate = request.ExpirationDate;
            item.Status = request.Status;
            item.IsActive = request.IsActive;
            item.CreatedAt = request.CreatedAt;
            item.UpdatedAt = request.UpdatedAt;
            item.ClinicId = request.ClinicId;
            var updated = await _inventoryRepository.UpdateAsync(id, item);
            return MapToResponse(updated);
        }

        public async Task<bool> DeleteInventoryAsync(int id) => await _inventoryRepository.DeleteAsync(id);

        public async Task<IEnumerable<Inventory>> GetByStatusAsync(string status)
        {
            // Implementação fictícia, pois não há método correspondente no repositório
            return new List<Inventory>();
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10)
        {
            // Implementação fictícia, pois não há método correspondente no repositório
            return new List<Inventory>();
        }

        public async Task<bool> UpdateStockAsync(int id, int newQuantity)
        {
            // Implementação fictícia, pois não há método correspondente no repositório
            return false;
        }

        public async Task<IEnumerable<InventoryResponse>> GetAllAsync()
        {
            var items = await _inventoryRepository.GetAllAsync();
            return items.Select(MapToResponse);
        }

        public async Task<InventoryResponse?> GetByIdAsync(int id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            return item == null ? null : MapToResponse(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _inventoryRepository.DeleteAsync(id);
        }

        private InventoryResponse MapToResponse(Inventory inventory)
        {
            return new InventoryResponse
            {
                Name = inventory.Name,
                Description = inventory.Description,
                Category = inventory.Category,
                Quantity = inventory.Quantity,
                Unit = inventory.Unit,
                MinStock = inventory.MinStock,
                UnitPrice = inventory.UnitPrice,
                Supplier = inventory.Supplier,
                Location = inventory.Location,
                BatchNumber = inventory.BatchNumber,
                ExpirationDate = inventory.ExpirationDate,
                Status = inventory.Status,
                IsActive = inventory.IsActive,
                CreatedAt = inventory.CreatedAt,
                UpdatedAt = inventory.UpdatedAt,
                ClinicId = inventory.ClinicId
            };
        }
    }
}