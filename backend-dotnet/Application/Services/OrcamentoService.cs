using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentalSpa.Application.Services
{
    public class OrcamentoService : IOrcamentoService
    {
        private readonly IOrcamentoRepository _orcamentoRepository;

        public OrcamentoService(IOrcamentoRepository orcamentoRepository)
        {
            _orcamentoRepository = orcamentoRepository;
        }

        public async Task<IEnumerable<Orcamento>> GetAllAsync()
        {
            return await _orcamentoRepository.GetAllAsync();
        }

        public async Task<Orcamento?> GetByIdAsync(int id)
        {
            return await _orcamentoRepository.GetByIdAsync(id);
        }

        public async Task<Orcamento> CreateAsync(Orcamento orcamento)
        {
            orcamento.CreatedAt = DateTime.UtcNow;
            orcamento.UpdatedAt = DateTime.UtcNow;
            return await _orcamentoRepository.CreateAsync(orcamento);
        }

        public async Task<Orcamento?> UpdateAsync(int id, Orcamento orcamento)
        {
            orcamento.UpdatedAt = DateTime.UtcNow;
            return await _orcamentoRepository.UpdateAsync(id, orcamento);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _orcamentoRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Orcamento>> SearchAsync(string searchTerm)
        {
            return await _orcamentoRepository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<Orcamento>> GetOrcamentosByPacienteAsync(int pacienteId)
        {
            return await _orcamentoRepository.GetOrcamentosByPacienteAsync(pacienteId);
        }

        public async Task<bool> AprovarOrcamentoAsync(int id)
        {
            return await _orcamentoRepository.UpdateStatusAsync(id, "Aprovado");
        }

        public async Task<bool> RecusarOrcamentoAsync(int id)
        {
            return await _orcamentoRepository.UpdateStatusAsync(id, "Recusado");
        }

        public async Task<bool> ConverterOrcamentoEmTratamentoAsync(int id)
        {
            return await _orcamentoRepository.UpdateStatusAsync(id, "Convertido");
        }
    }
} 