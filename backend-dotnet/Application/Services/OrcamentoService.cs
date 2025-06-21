using DentalSpa.Application.Interfaces;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentalSpa.Application.Services
{
    public class OrcamentoService : IOrcamentoService
    {
        private readonly IOrcamentoRepository _repo;
        public OrcamentoService(IOrcamentoRepository repo)
        {
            _repo = repo;
        }

        public async Task<Orcamento> CreateOrcamentoAsync(Orcamento orcamento)
        {
            orcamento.Status = "Pendente";
            orcamento.DataCriacao = DateTime.UtcNow;
            
            // Calcular valor total dos itens
            foreach (var item in orcamento.Itens)
            {
                item.ValorTotal = item.Quantidade * item.ValorUnitario;
            }
            orcamento.ValorTotal = orcamento.Itens.Sum(x => x.ValorTotal);
            
            return await _repo.CreateOrcamentoAsync(orcamento);
        }

        public async Task<Orcamento?> GetOrcamentoByIdAsync(int id)
        {
            return await _repo.GetOrcamentoByIdAsync(id);
        }

        public async Task<IEnumerable<Orcamento>> GetOrcamentosByPacienteAsync(int pacienteId)
        {
            return await _repo.GetOrcamentosByPacienteAsync(pacienteId);
        }

        public async Task<IEnumerable<Orcamento>> GetAllOrcamentosAsync()
        {
            return await _repo.GetAllOrcamentosAsync();
        }

        public async Task<Orcamento?> UpdateOrcamentoAsync(Orcamento orcamento)
        {
            var existing = await _repo.GetOrcamentoByIdAsync(orcamento.Id);
            if (existing == null) return null;
            
            // Atualizar propriedades
            existing.Observacoes = orcamento.Observacoes ?? existing.Observacoes;
            existing.Status = orcamento.Status ?? existing.Status;
            
            // Atualizar itens se fornecidos
            if (orcamento.Itens != null && orcamento.Itens.Any())
            {
                existing.Itens = orcamento.Itens.Select(i => new OrcamentoItem
                {
                    ServicoId = i.ServicoId,
                    Descricao = i.Descricao,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    ValorTotal = i.Quantidade * i.ValorUnitario
                }).ToList();
                existing.ValorTotal = existing.Itens.Sum(x => x.ValorTotal);
            }
            
            return await _repo.UpdateOrcamentoAsync(existing);
        }

        public async Task<bool> DeleteOrcamentoAsync(int id)
        {
            return await _repo.DeleteOrcamentoAsync(id);
        }

        public async Task<bool> AprovarOrcamentoAsync(int id)
        {
            return await _repo.UpdateStatusAsync(id, "Aprovado");
        }

        public async Task<bool> RecusarOrcamentoAsync(int id)
        {
            return await _repo.UpdateStatusAsync(id, "Recusado");
        }

        public async Task<bool> ConverterOrcamentoEmTratamentoAsync(int id)
        {
            return await _repo.UpdateStatusAsync(id, "Convertido");
        }
    }
} 