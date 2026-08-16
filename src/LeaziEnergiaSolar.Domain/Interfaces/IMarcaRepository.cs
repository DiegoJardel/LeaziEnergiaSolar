using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface IMarcaRepository
{
    Task<IReadOnlyList<Marca>> ListarAsync(string? pesquisa, bool? ativo, CancellationToken cancellationToken = default);
    Task<Marca?> ObterAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken cancellationToken = default);
    Task<bool> PossuiEquipamentosAsync(int id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Marca entidade, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Marca entidade, CancellationToken cancellationToken = default);
}
