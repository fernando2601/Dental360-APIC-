using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class FinancialTransactionRepository : BaseRepository<FinancialTransaction>, IFinancialTransactionRepository
    {
        public FinancialTransactionRepository(DentalSpaDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<FinancialTransaction>> GetAllAsync()
        {
            return await _dbSet
                .Include(f => f.Client)
                .Include(f => f.Appointment)
                    .ThenInclude(a => a!.Service)
                .OrderByDescending(f => f.Date)
                .ToListAsync();
        }

        public override async Task<FinancialTransaction> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(f => f.Client)
                .Include(f => f.Appointment)
                    .ThenInclude(a => a!.Service)
                .FirstOrDefaultAsync(f => f.Id == id) ?? throw new KeyNotFoundException($"FinancialTransaction with id {id} not found");
        }

        public async Task<IEnumerable<FinancialTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(f => f.Client)
                .Include(f => f.Appointment)
                    .ThenInclude(a => a!.Service)
                .Where(f => f.Date >= startDate && f.Date <= endDate)
                .OrderByDescending(f => f.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialTransaction>> GetByTypeAsync(string type)
        {
            return await _dbSet
                .Include(f => f.Client)
                .Include(f => f.Appointment)
                    .ThenInclude(a => a!.Service)
                .Where(f => f.Type == type)
                .OrderByDescending(f => f.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalIncomeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(f => f.Type == "income" && f.Date >= startDate && f.Date <= endDate)
                .SumAsync(f => f.Amount);
        }

        public async Task<decimal> GetTotalExpenseAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(f => f.Type == "expense" && f.Date >= startDate && f.Date <= endDate)
                .SumAsync(f => f.Amount);
        }
    }
}