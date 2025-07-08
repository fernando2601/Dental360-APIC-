using DentalSpa.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Interfaces
{
    public interface IOrcamentoService
    {
        Task<IEnumerable<OrcamentoResponse>> GetAllAsync();
        Task<OrcamentoResponse?> GetByIdAsync(int id);
        Task<Orcamento> CreateAsync(Orcamento orcamento);
        Task<Orcamento?> UpdateAsync(int id, Orcamento orcamento);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<OrcamentoResponse>> SearchAsync(string searchTerm);
        Task<IEnumerable<Orcamento>> GetOrcamentosByPacienteAsync(int pacienteId);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> AprovarOrcamentoAsync(int id);
        Task<bool> RecusarOrcamentoAsync(int id);
        Task<bool> ConverterOrcamentoEmTratamentoAsync(int id);
        Task<Orcamento?> CreateOrcamentoAsync(Orcamento orcamento);
        Task<Orcamento?> GetOrcamentoByIdAsync(int id);
        Task<IEnumerable<Orcamento>> GetAllOrcamentosAsync();
        Task<Orcamento?> UpdateOrcamentoAsync(int id, Orcamento orcamento);
        Task<bool> DeleteOrcamentoAsync(int id);
    }
} 