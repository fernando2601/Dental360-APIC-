using DentalSpa.Application.DTOs;
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

        public async Task<OrcamentoDto> CreateOrcamentoAsync(CreateOrcamentoDto dto)
        {
            var orcamento = new Orcamento
            {
                PacienteId = dto.PacienteId,
                Observacoes = dto.Observacoes,
                Status = "Pendente",
                DataCriacao = DateTime.UtcNow,
                Itens = dto.Itens.Select(i => new OrcamentoItem
                {
                    ServicoId = i.ServicoId,
                    Descricao = i.Descricao,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    ValorTotal = i.Quantidade * i.ValorUnitario
                }).ToList()
            };
            orcamento.ValorTotal = orcamento.Itens.Sum(x => x.ValorTotal);
            var created = await _repo.CreateOrcamentoAsync(orcamento);
            return MapToDto(created);
        }

        public async Task<OrcamentoDto?> GetOrcamentoByIdAsync(int id)
        {
            var orcamento = await _repo.GetOrcamentoByIdAsync(id);
            return orcamento == null ? null : MapToDto(orcamento);
        }

        public async Task<IEnumerable<OrcamentoDto>> GetOrcamentosByPacienteAsync(int pacienteId)
        {
            var orcamentos = await _repo.GetOrcamentosByPacienteAsync(pacienteId);
            return orcamentos.Select(MapToDto);
        }

        public async Task<IEnumerable<OrcamentoDto>> GetAllOrcamentosAsync()
        {
            var orcamentos = await _repo.GetAllOrcamentosAsync();
            return orcamentos.Select(MapToDto);
        }

        public async Task<OrcamentoDto?> UpdateOrcamentoAsync(int id, UpdateOrcamentoDto dto)
        {
            var orcamento = await _repo.GetOrcamentoByIdAsync(id);
            if (orcamento == null) return null;
            orcamento.Observacoes = dto.Observacoes ?? orcamento.Observacoes;
            if (dto.Itens.Any())
            {
                orcamento.Itens = dto.Itens.Select(i => new OrcamentoItem
                {
                    ServicoId = i.ServicoId,
                    Descricao = i.Descricao,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    ValorTotal = i.Quantidade * i.ValorUnitario
                }).ToList();
                orcamento.ValorTotal = orcamento.Itens.Sum(x => x.ValorTotal);
            }
            if (!string.IsNullOrEmpty(dto.Status))
                orcamento.Status = dto.Status;
            var updated = await _repo.UpdateOrcamentoAsync(orcamento);
            return MapToDto(updated);
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

        private static OrcamentoDto MapToDto(Orcamento o) => new OrcamentoDto
        {
            Id = o.Id,
            PacienteId = o.PacienteId,
            DataCriacao = o.DataCriacao,
            Status = o.Status,
            ValorTotal = o.ValorTotal,
            Observacoes = o.Observacoes,
            Itens = o.Itens.Select(i => new OrcamentoItemDto
            {
                Id = i.Id,
                ServicoId = i.ServicoId,
                Descricao = i.Descricao,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                ValorTotal = i.ValorTotal
            }).ToList()
        };
    }
} 