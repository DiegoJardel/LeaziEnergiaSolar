using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface IUnidadeMedidaRepository
{
    Task<IReadOnlyList<UnidadeMedida>> ListarAsync(string? pesquisa, bool? ativo, CancellationToken cancellationToken = default);
    Task<UnidadeMedida?> ObterAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteSiglaAsync(string sigla, int? ignorarId = null, CancellationToken cancellationToken = default);
    Task<bool> PossuiEquipamentosAsync(int id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(UnidadeMedida entidade, CancellationToken cancellationToken = default);
    Task AtualizarAsync(UnidadeMedida entidade, CancellationToken cancellationToken = default);
}
