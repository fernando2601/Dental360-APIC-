using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class PackageRepository : BaseRepository<Package>, IPackageRepository
    {
        public PackageRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Package>> GetActivePackagesAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }

    public class ClientPackageRepository : BaseRepository<ClientPackage>, IClientPackageRepository
    {
        public ClientPackageRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<ClientPackage>> GetAllAsync()
        {
            return await _dbSet
                .Include(cp => cp.Client)
                .Include(cp => cp.Package)
                .OrderByDescending(cp => cp.PurchaseDate)
                .ToListAsync();
        }

        public override async Task<ClientPackage> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(cp => cp.Client)
                .Include(cp => cp.Package)
                .FirstOrDefaultAsync(cp => cp.Id == id) ?? throw new KeyNotFoundException($"ClientPackage with id {id} not found");
        }

        public async Task<IEnumerable<ClientPackage>> GetByClientIdAsync(int clientId)
        {
            return await _dbSet
                .Include(cp => cp.Client)
                .Include(cp => cp.Package)
                .Where(cp => cp.ClientId == clientId)
                .OrderByDescending(cp => cp.PurchaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClientPackage>> GetActivePackagesAsync()
        {
            return await _dbSet
                .Include(cp => cp.Client)
                .Include(cp => cp.Package)
                .Where(cp => cp.Status == "active")
                .OrderByDescending(cp => cp.PurchaseDate)
                .ToListAsync();
        }
    }
}