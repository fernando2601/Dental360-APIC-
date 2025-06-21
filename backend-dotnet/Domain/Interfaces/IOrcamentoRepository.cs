using DentalSpa.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Domain.Interfaces
{
    public interface IOrcamentoRepository
    {
        Task<IEnumerable<Orcamento>> GetAllAsync();
        Task<Orcamento?> GetByIdAsync(int id);
        Task<Orcamento> CreateAsync(Orcamento orcamento);
        Task<Orcamento?> UpdateAsync(int id, Orcamento orcamento);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Orcamento>> SearchAsync(string searchTerm);
    }
} 