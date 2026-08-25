using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IControleAnualService
{
    Task<IReadOnlyList<int>> ListarAnosDisponiveisAsync(
        CancellationToken cancellationToken = default);

    Task<ControleAnualDto> ObterAsync(
        FiltroControleAnualDto filtro,
        CancellationToken cancellationToken = default);
}