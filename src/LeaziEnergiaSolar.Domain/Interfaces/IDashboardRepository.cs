using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface IDashboardRepository
{
    Task<decimal> ObterTotalVendidoAsync(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default);

    Task<decimal> ObterTotalComissaoAsync(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default);

    Task<int> ContarLancamentosAsync(
        DateTime inicio,
        DateTime fim,
        StatusLancamento? status = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Lancamento>> ObterUltimosLancamentosAsync(
        DateTime inicio,
        DateTime fim,
        int quantidade,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Lancamento>> ObterLancamentosDoAnoAsync(
        int ano,
        CancellationToken cancellationToken = default);
}
