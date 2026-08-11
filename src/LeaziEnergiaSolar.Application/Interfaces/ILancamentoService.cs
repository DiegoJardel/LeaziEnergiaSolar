using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface ILancamentoService
{
    Task<IReadOnlyList<LancamentoDto>> ListarAsync(
        FiltroLancamentoDto filtro,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarLancamentoDto lancamento,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        StatusLancamento status,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> ExcluirAsync(
        int id,
        CancellationToken cancellationToken = default);

    decimal CalcularComissao(
        decimal valorVenda,
        decimal percentualComissao);
}
