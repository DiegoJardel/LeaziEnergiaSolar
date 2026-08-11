using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> ObterAsync(
        int ano,
        int? mes = null,
        CancellationToken cancellationToken = default);
}
