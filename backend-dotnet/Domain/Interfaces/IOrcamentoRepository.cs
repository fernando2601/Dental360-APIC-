using DentalSpa.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Domain.Interfaces
{
    public interface IOrcamentoRepository
    {
        Task<Orcamento> CreateOrcamentoAsync(Orcamento orcamento);
        Task<Orcamento?> GetOrcamentoByIdAsync(int id);
        Task<IEnumerable<Orcamento>> GetOrcamentosByPacienteAsync(int pacienteId);
        Task<IEnumerable<Orcamento>> GetAllOrcamentosAsync();
        Task<Orcamento> UpdateOrcamentoAsync(Orcamento orcamento);
        Task<bool> DeleteOrcamentoAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
    }
} 