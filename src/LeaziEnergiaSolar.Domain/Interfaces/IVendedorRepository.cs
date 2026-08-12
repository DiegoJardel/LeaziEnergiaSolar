using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface IVendedorRepository
{
    Task<IReadOnlyList<Vendedor>> ListarAsync(
        string? pesquisa = null,
        CancellationToken cancellationToken = default);

    Task<Vendedor?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteDocumentoAsync(
        string cpfCnpj,
        int? ignorarId = null,
        CancellationToken cancellationToken = default);

    Task<bool> PossuiLancamentosAsync(
        int vendedorId,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Vendedor vendedor,
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        Vendedor vendedor,
        CancellationToken cancellationToken = default);

    Task ExcluirAsync(
        Vendedor vendedor,
        CancellationToken cancellationToken = default);
}