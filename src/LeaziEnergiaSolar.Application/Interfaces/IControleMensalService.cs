using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IControleMensalService
{
    Task<IReadOnlyList<int>> ListarAnosDisponiveisAsync(
        CancellationToken cancellationToken = default);

    Task<ControleMensalDto> ObterAsync(
        FiltroControleMensalDto filtro,
        CancellationToken cancellationToken = default);
}