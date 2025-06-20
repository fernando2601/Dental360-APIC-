using DentalSpa.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Application.Interfaces
{
    public interface IOrcamentoService
    {
        Task<OrcamentoDto> CreateOrcamentoAsync(CreateOrcamentoDto dto);
        Task<OrcamentoDto?> GetOrcamentoByIdAsync(int id);
        Task<IEnumerable<OrcamentoDto>> GetOrcamentosByPacienteAsync(int pacienteId);
        Task<IEnumerable<OrcamentoDto>> GetAllOrcamentosAsync();
        Task<OrcamentoDto?> UpdateOrcamentoAsync(int id, UpdateOrcamentoDto dto);
        Task<bool> DeleteOrcamentoAsync(int id);
        Task<bool> AprovarOrcamentoAsync(int id);
        Task<bool> RecusarOrcamentoAsync(int id);
        Task<bool> ConverterOrcamentoEmTratamentoAsync(int id);
    }
} 