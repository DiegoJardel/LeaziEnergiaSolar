using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface ILancamentoRepository
{
    Task<IReadOnlyList<Lancamento>> ListarAsync(
        string? pesquisa = null,
        DateTime? dataInicial = null,
        DateTime? dataFinal = null,
        int? vendedorId = null,
        StatusLancamento? status = null,
        CancellationToken cancellationToken = default);

    Task<Lancamento?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<int> ContarPorClienteAsync(
        int clienteId,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default);

    Task ExcluirAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default);
}