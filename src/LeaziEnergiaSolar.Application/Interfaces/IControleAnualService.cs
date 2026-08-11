using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IControleAnualService
{
    Task<ControleAnualDto> ObterAsync(
        FiltroControleAnualDto filtro,
        CancellationToken cancellationToken = default);
}
