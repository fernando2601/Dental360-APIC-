using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreateOrcamentoDto
    {
        [Required]
        public int PacienteId { get; set; }
        [Required]
        public List<CreateOrcamentoItemDto> Itens { get; set; } = new();
        public string? Observacoes { get; set; }
    }

    public class CreateOrcamentoItemDto
    {
        [Required]
        public int ServicoId { get; set; }
        [Required]
        public string Descricao { get; set; } = string.Empty;
        [Required]
        public int Quantidade { get; set; }
        [Required]
        public decimal ValorUnitario { get; set; }
    }

    public class UpdateOrcamentoDto
    {
        [Required]
        public int Id { get; set; }
        public List<CreateOrcamentoItemDto> Itens { get; set; } = new();
        public string? Observacoes { get; set; }
        public string? Status { get; set; }
    }

    public class OrcamentoDto
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public List<OrcamentoItemDto> Itens { get; set; } = new();
    }

    public class OrcamentoItemDto
    {
        public int Id { get; set; }
        public int ServicoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
} 