using DentalSpa.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Domain.Interfaces
{
    public interface IProfissionalSaudeRepository
    {
        Task<ProfissionalSaude?> GetByIdAsync(int id);
        Task<ProfissionalSaude?> GetByStaffIdAsync(int staffId);
        Task<IEnumerable<ProfissionalSaude>> GetAllAsync();
        Task<ProfissionalSaude> CreateAsync(ProfissionalSaude profissional);
        Task<ProfissionalSaude?> UpdateAsync(int id, ProfissionalSaude profissional);
        Task<bool> DeleteAsync(int id);
    }
} 