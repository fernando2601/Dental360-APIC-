using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Services
{
    public class OrcamentoService : IOrcamentoService
    {
        private readonly IOrcamentoRepository _orcamentoRepository;

        public OrcamentoService(IOrcamentoRepository orcamentoRepository)
        {
            _orcamentoRepository = orcamentoRepository;
        }

        public async Task<IEnumerable<OrcamentoResponse>> GetAllAsync()
        {
            var orcamentos = await _orcamentoRepository.GetAllAsync();
            return orcamentos.Select(MapToResponse);
        }

        public async Task<OrcamentoResponse?> GetByIdAsync(int id)
        {
            var orcamento = await _orcamentoRepository.GetByIdAsync(id);
            return orcamento == null ? null : MapToResponse(orcamento);
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

        public async Task<IEnumerable<OrcamentoResponse>> SearchAsync(string searchTerm)
        {
            var orcamentos = await _orcamentoRepository.SearchAsync(searchTerm);
            return orcamentos.Select(MapToResponse);
        }

        public async Task<IEnumerable<Orcamento>> GetOrcamentosByPacienteAsync(int pacienteId)
        {
            return await _orcamentoRepository.GetOrcamentosByPacienteAsync(pacienteId);
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            return await _orcamentoRepository.UpdateStatusAsync(id, status);
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
            return await _orcamentoRepository.UpdateStatusAsync(id, "Convertido em Tratamento");
        }

        public async Task<Orcamento?> CreateOrcamentoAsync(Orcamento orcamento)
        {
            return await _orcamentoRepository.CreateAsync(orcamento);
        }

        public async Task<Orcamento?> GetOrcamentoByIdAsync(int id)
        {
            return await _orcamentoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Orcamento>> GetAllOrcamentosAsync()
        {
            return await _orcamentoRepository.GetAllAsync();
        }

        public async Task<Orcamento?> UpdateOrcamentoAsync(int id, Orcamento orcamento)
        {
            return await _orcamentoRepository.UpdateAsync(id, orcamento);
        }

        public async Task<bool> DeleteOrcamentoAsync(int id)
        {
            return await _orcamentoRepository.DeleteAsync(id);
        }

        private OrcamentoResponse MapToResponse(Orcamento o)
        {
            return new OrcamentoResponse
            {
                PacienteId = o.PacienteId,
                DataCriacao = o.DataCriacao,
                Status = o.Status,
                ValorTotal = o.ValorTotal,
                Observacoes = o.Observacoes,
                Itens = o.Itens.Select(i => new OrcamentoItemResponse
                {
                    OrcamentoId = i.OrcamentoId,
                    ServicoId = i.ServicoId,
                    Descricao = i.Descricao,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    ValorTotal = i.ValorTotal
                }).ToList(),
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };
        }
    }
} 