using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IIbgeLocalidadeService
{
    Task<IReadOnlyList<EstadoDto>> ListarEstadosAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosAsync(
        string codigoIbgeEstado,
        CancellationToken cancellationToken = default);
}
