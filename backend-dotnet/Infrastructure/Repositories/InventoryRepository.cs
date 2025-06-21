using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DentalSpaDbContext _context;

        public InventoryRepository(DentalSpaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Inventories.ToListAsync();
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _context.Inventories.FindAsync(id);
        }

        public async Task<Inventory> CreateAsync(Inventory inventory)
        {
            inventory.CreatedAt = DateTime.UtcNow;
            inventory.UpdatedAt = DateTime.UtcNow;
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<Inventory?> UpdateAsync(int id, Inventory inventory)
        {
            var existingInventory = await _context.Inventories.FindAsync(id);
            if (existingInventory == null) return null;

            existingInventory.Name = inventory.Name;
            existingInventory.Description = inventory.Description;
            existingInventory.Category = inventory.Category;
            existingInventory.Quantity = inventory.Quantity;
            existingInventory.Unit = inventory.Unit;
            existingInventory.MinStock = inventory.MinStock;
            existingInventory.UnitPrice = inventory.UnitPrice;
            existingInventory.Supplier = inventory.Supplier;
            existingInventory.Location = inventory.Location;
            existingInventory.BatchNumber = inventory.BatchNumber;
            existingInventory.ExpirationDate = inventory.ExpirationDate;
            existingInventory.Status = inventory.Status;
            existingInventory.IsActive = inventory.IsActive;
            existingInventory.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingInventory;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return false;

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Inventory>> SearchAsync(string searchTerm)
        {
            return await _context.Inventories
                .Where(i => i.Name.Contains(searchTerm) || 
                           i.Description.Contains(searchTerm) || 
                           i.Category.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetByCategoryAsync(string category)
        {
            return await _context.Inventories
                .Where(i => i.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetLowStockAsync()
        {
            return await _context.Inventories
                .Where(i => i.Quantity <= i.MinStock)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetExpiredAsync()
        {
            return await _context.Inventories
                .Where(i => i.ExpirationDate.HasValue && i.ExpirationDate <= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Inventories.CountAsync();
        }

        public async Task<int> GetCountByCategoryAsync(string category)
        {
            return await _context.Inventories
                .Where(i => i.Category == category)
                .CountAsync();
        }

        public async Task<int> GetLowStockCountAsync()
        {
            return await _context.Inventories
                .Where(i => i.Quantity <= i.MinStock)
                .CountAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Inventories.AnyAsync(i => i.Id == id);
        }
    }
}