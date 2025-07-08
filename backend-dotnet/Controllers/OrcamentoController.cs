using Microsoft.AspNetCore.Mvc;
using DentalSpa.Application.Interfaces;
using DentalSpa.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using DentalSpa.Application.DTOs;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrcamentoController : ControllerBase
    {
        private readonly IOrcamentoService _service;
        public OrcamentoController(IOrcamentoService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<OrcamentoResponse>> Create([FromBody] Orcamento orcamento)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newOrcamento = await _service.CreateOrcamentoAsync(orcamento);
            var response = new OrcamentoResponse
            {
                PacienteId = newOrcamento.PacienteId,
                DataCriacao = newOrcamento.DataCriacao,
                Status = newOrcamento.Status,
                ValorTotal = newOrcamento.ValorTotal,
                Observacoes = newOrcamento.Observacoes,
                Itens = newOrcamento.Itens.Select(i => new OrcamentoItemResponse
                {
                    OrcamentoId = i.OrcamentoId,
                    ServicoId = i.ServicoId,
                    Descricao = i.Descricao,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    ValorTotal = i.ValorTotal
                }).ToList(),
                CreatedAt = newOrcamento.CreatedAt,
                UpdatedAt = newOrcamento.UpdatedAt
            };
            return CreatedAtAction(nameof(GetById), null, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrcamentoResponse>> GetById(int id)
        {
            var orcamento = await _service.GetOrcamentoByIdAsync(id);
            if (orcamento == null)
                return NotFound();
            return Ok(orcamento);
        }

        [HttpGet("paciente/{pacienteId}")]
        public async Task<ActionResult<IEnumerable<Orcamento>>> GetByPaciente(int pacienteId)
        {
            var orcamentos = await _service.GetOrcamentosByPacienteAsync(pacienteId);
            return Ok(orcamentos);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrcamentoResponse>>> GetAll()
        {
            var orcamentos = await _service.GetAllOrcamentosAsync();
            return Ok(orcamentos);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Orcamento>> Update(int id, [FromBody] Orcamento orcamento)
        {
            if (id != orcamento.Id)
            {
                return BadRequest("O ID do orçamento não corresponde.");
            }
            var result = await _service.UpdateOrcamentoAsync(id, orcamento);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteOrcamentoAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/aprovar")]
        public async Task<IActionResult> Aprovar(int id)
        {
            var ok = await _service.AprovarOrcamentoAsync(id);
            if (!ok) return NotFound();
            return Ok(new { message = "Orçamento aprovado" });
        }

        [HttpPost("{id}/recusar")]
        public async Task<IActionResult> Recusar(int id)
        {
            var ok = await _service.RecusarOrcamentoAsync(id);
            if (!ok) return NotFound();
            return Ok(new { message = "Orçamento recusado" });
        }

        [HttpPost("{id}/converter")]
        public async Task<IActionResult> Converter(int id)
        {
            var ok = await _service.ConverterOrcamentoEmTratamentoAsync(id);
            if (!ok) return NotFound();
            return Ok(new { message = "Orçamento convertido em tratamento" });
        }
    }
} 