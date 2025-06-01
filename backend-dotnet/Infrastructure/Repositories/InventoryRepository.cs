using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync()
        {
            return await _dbSet
                .Where(i => i.Quantity <= i.MinQuantity)
                .OrderBy(i => i.Quantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetExpiringSoonItemsAsync()
        {
            var thirtyDaysFromNow = DateTime.Now.AddDays(30);
            return await _dbSet
                .Where(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value <= thirtyDaysFromNow && i.ExpiryDate.Value >= DateTime.Now)
                .OrderBy(i => i.ExpiryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(i => i.Category == category)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }
    }
}