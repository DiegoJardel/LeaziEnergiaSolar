using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface ILocalidadeRepository
{
    Task<IReadOnlyList<Estado>> ListarEstadosAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Municipio>> ListarMunicipiosAsync(string codigoIbgeEstado, CancellationToken cancellationToken = default);

    Task<Estado?> ObterEstadoPorCodigoIbgeAsync(string codigoIbge, CancellationToken cancellationToken = default);

    Task<Municipio?> ObterMunicipioPorCodigoIbgeAsync(string codigoIbge, CancellationToken cancellationToken = default);

    Task SalvarEstadosAsync(IEnumerable<Estado> estados, CancellationToken cancellationToken = default);

    Task SalvarMunicipiosAsync(IEnumerable<Municipio> municipios, CancellationToken cancellationToken = default);
}
