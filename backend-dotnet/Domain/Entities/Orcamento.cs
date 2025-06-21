using System;
using System.Collections.Generic;

namespace DentalSpa.Domain.Entities
{
    public class Orcamento
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pendente"; // Pendente, Aprovado, Recusado, Convertido
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public List<OrcamentoItem> Itens { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OrcamentoItem
    {
        public int Id { get; set; }
        public int OrcamentoId { get; set; }
        public int ServicoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
} 