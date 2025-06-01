using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class BeforeAfterRepository : BaseRepository<BeforeAfter>, IBeforeAfterRepository
    {
        public BeforeAfterRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<BeforeAfter>> GetAllAsync()
        {
            return await _dbSet
                .Include(ba => ba.Client)
                .Include(ba => ba.Service)
                .OrderByDescending(ba => ba.TreatmentDate)
                .ToListAsync();
        }

        public override async Task<BeforeAfter> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(ba => ba.Client)
                .Include(ba => ba.Service)
                .FirstOrDefaultAsync(ba => ba.Id == id) ?? throw new KeyNotFoundException($"BeforeAfter with id {id} not found");
        }

        public async Task<IEnumerable<BeforeAfter>> GetByClientIdAsync(int clientId)
        {
            return await _dbSet
                .Include(ba => ba.Client)
                .Include(ba => ba.Service)
                .Where(ba => ba.ClientId == clientId)
                .OrderByDescending(ba => ba.TreatmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BeforeAfter>> GetPublicGalleryAsync()
        {
            return await _dbSet
                .Include(ba => ba.Client)
                .Include(ba => ba.Service)
                .Where(ba => ba.IsPublic)
                .OrderByDescending(ba => ba.TreatmentDate)
                .ToListAsync();
        }
    }

    public class LearningAreaRepository : BaseRepository<LearningArea>, ILearningAreaRepository
    {
        public LearningAreaRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LearningArea>> GetByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(la => la.Category == category)
                .OrderByDescending(la => la.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<LearningArea>> GetPublishedAsync()
        {
            return await _dbSet
                .Where(la => la.IsPublished)
                .OrderByDescending(la => la.CreatedAt)
                .ToListAsync();
        }
    }

    public class ClinicInfoRepository : BaseRepository<ClinicInfo>, IClinicInfoRepository
    {
        public ClinicInfoRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public async Task<ClinicInfo?> GetClinicInfoAsync()
        {
            return await _dbSet.FirstOrDefaultAsync();
        }
    }
}