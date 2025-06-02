using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class LearningAreaRepository : ILearningAreaRepository
    {
        private readonly DentalSpaDbContext _context;

        public LearningAreaRepository(DentalSpaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LearningArea>> GetAllAsync()
        {
            return await _context.LearningAreas.ToListAsync();
        }

        public async Task<LearningArea?> GetByIdAsync(int id)
        {
            return await _context.LearningAreas.FindAsync(id);
        }

        public async Task<LearningArea> CreateAsync(LearningArea learningArea)
        {
            _context.LearningAreas.Add(learningArea);
            await _context.SaveChangesAsync();
            return learningArea;
        }

        public async Task<LearningArea> UpdateAsync(LearningArea learningArea)
        {
            _context.Entry(learningArea).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return learningArea;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var learningArea = await _context.LearningAreas.FindAsync(id);
            if (learningArea == null) return false;

            _context.LearningAreas.Remove(learningArea);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LearningArea>> GetByCategoryAsync(string category)
        {
            return await _context.LearningAreas
                .Where(la => la.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<LearningArea>> GetActiveAreasAsync()
        {
            return await _context.LearningAreas
                .Where(la => la.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<LearningArea>> SearchByTitleAsync(string searchTerm)
        {
            return await _context.LearningAreas
                .Where(la => la.Title.Contains(searchTerm) || la.Description.Contains(searchTerm))
                .ToListAsync();
        }
    }
}