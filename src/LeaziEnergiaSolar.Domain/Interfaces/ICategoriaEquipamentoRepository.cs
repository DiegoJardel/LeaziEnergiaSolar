using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface ICategoriaEquipamentoRepository
{
    Task<IReadOnlyList<CategoriaEquipamento>> ListarAsync(string? pesquisa, bool? ativo, CancellationToken cancellationToken = default);
    Task<CategoriaEquipamento?> ObterAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteDescricaoAsync(string descricao, int? ignorarId = null, CancellationToken cancellationToken = default);
    Task<bool> PossuiEquipamentosAsync(int id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(CategoriaEquipamento entidade, CancellationToken cancellationToken = default);
    Task AtualizarAsync(CategoriaEquipamento entidade, CancellationToken cancellationToken = default);
}
