using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface IEquipamentoRepository
{
    Task<IReadOnlyList<Equipamento>> ListarAsync(string? pesquisa, int? categoriaId, int? marcaId, bool? ativo, CancellationToken cancellationToken = default);
    Task<Equipamento?> ObterAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteDuplicadoAsync(string descricao, int? marcaId, string? modelo, int? ignorarId = null, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Equipamento equipamento, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Equipamento equipamento, CancellationToken cancellationToken = default);
}
