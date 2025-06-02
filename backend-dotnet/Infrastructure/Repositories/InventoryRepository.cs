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
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<Inventory> UpdateAsync(Inventory inventory)
        {
            _context.Entry(inventory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return false;

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Inventory>> GetByStatusAsync(string status)
        {
            return await _context.Inventories
                .Where(i => i.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10)
        {
            return await _context.Inventories
                .Where(i => i.Quantity <= threshold)
                .ToListAsync();
        }

        public async Task<bool> UpdateStockAsync(int id, int newQuantity)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return false;

            inventory.Quantity = newQuantity;
            inventory.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Inventory>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Inventories
                .Where(i => i.Name.Contains(searchTerm) || i.Description.Contains(searchTerm))
                .ToListAsync();
        }
    }
}