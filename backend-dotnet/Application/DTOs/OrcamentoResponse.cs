using System;
using System.Collections.Generic;

namespace DentalSpa.Application.DTOs
{
    public class OrcamentoResponse
    {
        public int PacienteId { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pendente";
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public List<OrcamentoItemResponse> Itens { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public class OrcamentoItemResponse
    {
        public int OrcamentoId { get; set; }
        public int ServicoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
} 