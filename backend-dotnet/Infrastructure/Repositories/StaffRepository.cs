using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class StaffRepository : BaseRepository<Staff>, IStaffRepository
    {
        public StaffRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.User)
                .OrderBy(s => s.User!.FullName)
                .ToListAsync();
        }

        public override async Task<Staff> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id) ?? throw new KeyNotFoundException($"Staff with id {id} not found");
        }

        public async Task<IEnumerable<Staff>> GetActiveStaffAsync()
        {
            return await _dbSet
                .Include(s => s.User)
                .Where(s => s.IsActive)
                .OrderBy(s => s.User!.FullName)
                .ToListAsync();
        }

        public async Task<Staff?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}