using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                    .ThenInclude(s => s!.User)
                .ToListAsync();
        }

        public override async Task<Appointment> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                    .ThenInclude(s => s!.User)
                .FirstOrDefaultAsync(a => a.Id == id) ?? throw new KeyNotFoundException($"Appointment with id {id} not found");
        }

        public async Task<IEnumerable<Appointment>> GetByClientIdAsync(int clientId)
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                    .ThenInclude(s => s!.User)
                .Where(a => a.ClientId == clientId)
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                    .ThenInclude(s => s!.User)
                .Where(a => a.DateTime >= startDate && a.DateTime <= endDate)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByStaffIdAsync(int staffId)
        {
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                    .ThenInclude(s => s!.User)
                .Where(a => a.StaffId == staffId)
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            return await _dbSet
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                    .ThenInclude(s => s!.User)
                .Where(a => a.DateTime >= today && a.DateTime < tomorrow)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }
    }
}