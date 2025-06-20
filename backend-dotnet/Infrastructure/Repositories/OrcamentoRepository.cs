using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using Npgsql;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentalSpa.Infrastructure.Repositories
{
    public class OrcamentoRepository : IOrcamentoRepository
    {
        private readonly string _connectionString;
        public OrcamentoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<Orcamento> CreateOrcamentoAsync(Orcamento orcamento)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();
            var orcamentoId = await connection.QuerySingleAsync<int>(@"
                INSERT INTO orcamentos (paciente_id, data_criacao, status, valor_total, observacoes)
                VALUES (@PacienteId, @DataCriacao, @Status, @ValorTotal, @Observacoes)
                RETURNING id;",
                orcamento, tx);
            foreach (var item in orcamento.Itens)
            {
                item.OrcamentoId = orcamentoId;
                await connection.ExecuteAsync(@"
                    INSERT INTO orcamento_itens (orcamento_id, servico_id, descricao, quantidade, valor_unitario, valor_total)
                    VALUES (@OrcamentoId, @ServicoId, @Descricao, @Quantidade, @ValorUnitario, @ValorTotal);",
                    item, tx);
            }
            await tx.CommitAsync();
            return await GetOrcamentoByIdAsync(orcamentoId) ?? throw new System.Exception("Erro ao criar orçamento");
        }

        public async Task<Orcamento?> GetOrcamentoByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var orcamento = await connection.QueryFirstOrDefaultAsync<Orcamento>(
                "SELECT * FROM orcamentos WHERE id = @id", new { id });
            if (orcamento == null) return null;
            var itens = (await connection.QueryAsync<OrcamentoItem>(
                "SELECT * FROM orcamento_itens WHERE orcamento_id = @id", new { id })).ToList();
            orcamento.Itens = itens;
            return orcamento;
        }

        public async Task<IEnumerable<Orcamento>> GetOrcamentosByPacienteAsync(int pacienteId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var orcamentos = (await connection.QueryAsync<Orcamento>(
                "SELECT * FROM orcamentos WHERE paciente_id = @pacienteId ORDER BY data_criacao DESC", new { pacienteId })).ToList();
            foreach (var o in orcamentos)
                o.Itens = (await connection.QueryAsync<OrcamentoItem>(
                    "SELECT * FROM orcamento_itens WHERE orcamento_id = @id", new { id = o.Id })).ToList();
            return orcamentos;
        }

        public async Task<IEnumerable<Orcamento>> GetAllOrcamentosAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var orcamentos = (await connection.QueryAsync<Orcamento>(
                "SELECT * FROM orcamentos ORDER BY data_criacao DESC")).ToList();
            foreach (var o in orcamentos)
                o.Itens = (await connection.QueryAsync<OrcamentoItem>(
                    "SELECT * FROM orcamento_itens WHERE orcamento_id = @id", new { id = o.Id })).ToList();
            return orcamentos;
        }

        public async Task<Orcamento> UpdateOrcamentoAsync(Orcamento orcamento)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();
            await connection.ExecuteAsync(@"
                UPDATE orcamentos SET observacoes = @Observacoes, status = @Status, valor_total = @ValorTotal
                WHERE id = @Id;",
                orcamento, tx);
            await connection.ExecuteAsync("DELETE FROM orcamento_itens WHERE orcamento_id = @Id", orcamento, tx);
            foreach (var item in orcamento.Itens)
            {
                item.OrcamentoId = orcamento.Id;
                await connection.ExecuteAsync(@"
                    INSERT INTO orcamento_itens (orcamento_id, servico_id, descricao, quantidade, valor_unitario, valor_total)
                    VALUES (@OrcamentoId, @ServicoId, @Descricao, @Quantidade, @ValorUnitario, @ValorTotal);",
                    item, tx);
            }
            await tx.CommitAsync();
            return await GetOrcamentoByIdAsync(orcamento.Id) ?? throw new System.Exception("Erro ao atualizar orçamento");
        }

        public async Task<bool> DeleteOrcamentoAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync("DELETE FROM orcamento_itens WHERE orcamento_id = @id", new { id });
            var rows = await connection.ExecuteAsync("DELETE FROM orcamentos WHERE id = @id", new { id });
            return rows > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var rows = await connection.ExecuteAsync("UPDATE orcamentos SET status = @status WHERE id = @id", new { id, status });
            return rows > 0;
        }
    }
} 