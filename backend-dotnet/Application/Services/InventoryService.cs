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
        private readonly IProductRepository _productRepository;

        public InventoryService(IInventoryRepository inventoryRepository, ILogger<InventoryService> logger, IProductRepository productRepository)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
            _productRepository = productRepository;
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
            var inventory = new Inventory
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                MinQuantity = request.MinQuantity,
                Status = request.Status,
                ClinicId = request.ClinicId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var created = await _inventoryRepository.CreateAsync(inventory);
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            return new InventoryResponse
            {
                Id = created.Id,
                ProductId = created.ProductId,
                ProductName = product?.Name ?? string.Empty,
                ProductCategory = product?.Category ?? string.Empty,
                Quantity = created.Quantity,
                MinQuantity = created.MinQuantity,
                Status = created.Status,
                ClinicId = created.ClinicId,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            };
        }

        public async Task<InventoryResponse?> UpdateAsync(int id, InventoryCreateRequest request)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null) return null;
            item.ProductId = request.ProductId;
            item.Quantity = request.Quantity;
            item.MinQuantity = request.MinQuantity;
            item.Status = request.Status;
            item.ClinicId = request.ClinicId;
            item.UpdatedAt = DateTime.UtcNow;
            var updated = await _inventoryRepository.UpdateAsync(id, item);
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            return updated == null ? null : new InventoryResponse
            {
                Id = updated.Id,
                ProductId = updated.ProductId,
                ProductName = product?.Name ?? string.Empty,
                ProductCategory = product?.Category ?? string.Empty,
                Quantity = updated.Quantity,
                MinQuantity = updated.MinQuantity,
                Status = updated.Status,
                ClinicId = updated.ClinicId,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt
            };
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

        public async Task<IEnumerable<InventoryResponse>> GetByProductIdAsync(int productId)
        {
            var inventories = await _inventoryRepository.GetByProductIdAsync(productId);
            var product = await _productRepository.GetByIdAsync(productId);
            return inventories.Select(i => new InventoryResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = product?.Name ?? string.Empty,
                ProductCategory = product?.Category ?? string.Empty,
                Quantity = i.Quantity,
                MinQuantity = i.MinQuantity,
                Status = i.Status,
                ClinicId = i.ClinicId,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            });
        }

        public async Task<IEnumerable<InventoryResponse>> GetByClinicIdAsync(int clinicId)
        {
            var inventories = await _inventoryRepository.GetByClinicIdAsync(clinicId);
            var products = new Dictionary<int, (string Name, string? Category)>();
            foreach (var inv in inventories)
            {
                if (!products.ContainsKey(inv.ProductId))
                {
                    var prod = await _productRepository.GetByIdAsync(inv.ProductId);
                    products[inv.ProductId] = (prod?.Name ?? string.Empty, prod?.Category);
                }
            }
            return inventories.Select(i => new InventoryResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = products[i.ProductId].Name,
                ProductCategory = products[i.ProductId].Category ?? string.Empty,
                Quantity = i.Quantity,
                MinQuantity = i.MinQuantity,
                Status = i.Status,
                ClinicId = i.ClinicId,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            });
        }

        private InventoryResponse MapToResponse(Inventory inventory)
        {
            var product = _productRepository.GetByIdAsync(inventory.ProductId).Result;
            return new InventoryResponse
            {
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                ProductName = product?.Name ?? string.Empty,
                ProductCategory = product?.Category ?? string.Empty,
                Quantity = inventory.Quantity,
                MinQuantity = inventory.MinQuantity,
                Status = inventory.Status,
                ClinicId = inventory.ClinicId,
                CreatedAt = inventory.CreatedAt,
                UpdatedAt = inventory.UpdatedAt
            };
        }
    }
}