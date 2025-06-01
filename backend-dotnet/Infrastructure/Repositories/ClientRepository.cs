using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        public ClientRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Client>> SearchAsync(string searchTerm)
        {
            return await _dbSet
                .Where(c => c.FullName.Contains(searchTerm) || 
                           c.Email.Contains(searchTerm) || 
                           c.Phone.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<Client?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}