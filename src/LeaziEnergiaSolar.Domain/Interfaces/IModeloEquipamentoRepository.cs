using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface IModeloEquipamentoRepository
{
    Task<IReadOnlyList<ModeloEquipamento>> ListarAsync(
        int marcaId,
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default);

    Task<ModeloEquipamento?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteNomeAsync(
        int marcaId,
        string nome,
        int? ignorarId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteAlgumPorMarcaAsync(
        int marcaId,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        ModeloEquipamento modelo,
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        ModeloEquipamento modelo,
        CancellationToken cancellationToken = default);

    Task ExcluirAsync(
        ModeloEquipamento modelo,
        CancellationToken cancellationToken = default);
}