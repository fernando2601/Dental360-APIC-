using Microsoft.AspNetCore.Mvc;
using DentalSpa.Application.DTOs;
using DentalSpa.Application.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public async Task<ActionResult<OrcamentoDto>> Create([FromBody] CreateOrcamentoDto dto)
        {
            var orcamento = await _service.CreateOrcamentoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = orcamento.Id }, orcamento);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrcamentoDto>> GetById(int id)
        {
            var orcamento = await _service.GetOrcamentoByIdAsync(id);
            if (orcamento == null) return NotFound();
            return Ok(orcamento);
        }

        [HttpGet("paciente/{pacienteId}")]
        public async Task<ActionResult<IEnumerable<OrcamentoDto>>> GetByPaciente(int pacienteId)
        {
            var orcamentos = await _service.GetOrcamentosByPacienteAsync(pacienteId);
            return Ok(orcamentos);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrcamentoDto>>> GetAll()
        {
            var orcamentos = await _service.GetAllOrcamentosAsync();
            return Ok(orcamentos);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrcamentoDto>> Update(int id, [FromBody] UpdateOrcamentoDto dto)
        {
            var orcamento = await _service.UpdateOrcamentoAsync(id, dto);
            if (orcamento == null) return NotFound();
            return Ok(orcamento);
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