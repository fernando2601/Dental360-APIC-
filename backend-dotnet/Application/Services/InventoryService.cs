using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

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

        public async Task<Inventory> CreateInventoryAsync(Inventory inventory)
        {
            try
            {
                inventory.CreatedAt = DateTime.UtcNow;
                inventory.UpdatedAt = DateTime.UtcNow;
                return await _inventoryRepository.CreateAsync(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar item do inventário");
                throw;
            }
        }

        public async Task<Inventory> UpdateInventoryAsync(Inventory inventory)
        {
            try
            {
                inventory.UpdatedAt = DateTime.UtcNow;
                return await _inventoryRepository.UpdateAsync(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item do inventário: {Id}", inventory.Id);
                throw;
            }
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

        public async Task<IEnumerable<Inventory>> GetByStatusAsync(string status)
        {
            try
            {
                return await _inventoryRepository.GetByStatusAsync(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens por status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10)
        {
            try
            {
                return await _inventoryRepository.GetLowStockItemsAsync(threshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens com estoque baixo");
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(int id, int newQuantity)
        {
            try
            {
                return await _inventoryRepository.UpdateStockAsync(id, newQuantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar estoque do item: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Inventory>> SearchByNameAsync(string searchTerm)
        {
            try
            {
                return await _inventoryRepository.SearchByNameAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar itens por nome: {SearchTerm}", searchTerm);
                throw;
            }
        }
    }
}