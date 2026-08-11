using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IControleMensalService
{
    Task<ControleMensalDto> ObterAsync(
        FiltroControleMensalDto filtro,
        CancellationToken cancellationToken = default);
}
