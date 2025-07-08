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

        public async Task<bool> DeleteInventoryAsync(int id)
        {
            try
            {
                return await _inventoryRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar item do inventário: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Inventory>> SearchByNameAsync(string searchTerm)
        {
            try
            {
                return await _inventoryRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar itens por nome: {SearchTerm}", searchTerm);
                throw;
            }
        }

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

        private InventoryResponse MapToResponse(Inventory inventory)
        {
            // Implemente a lógica para mapear um objeto Inventory para um objeto InventoryResponse
            throw new NotImplementedException();
        }
    }
}