using DentalSpa.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Application.Interfaces
{
    public interface IProfissionalSaudeService
    {
        Task<ProfissionalSaudeResponse?> GetByIdAsync(int id);
        Task<ProfissionalSaudeResponse?> GetByStaffIdAsync(int staffId);
        Task<IEnumerable<ProfissionalSaudeResponse>> GetAllAsync();
        Task<ProfissionalSaudeResponse> CreateAsync(ProfissionalSaudeCreateRequest request);
        Task<ProfissionalSaudeResponse?> UpdateAsync(int id, ProfissionalSaudeCreateRequest request);
        Task<bool> DeleteAsync(int id);
    }
} 