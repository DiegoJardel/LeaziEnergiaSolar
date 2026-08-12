using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<IReadOnlyList<Usuario>> ListarAsync(
        string? pesquisa = null,
        CancellationToken cancellationToken = default);

    Task<Usuario?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Usuario?> ObterPorLoginAsync(
        string login,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteLoginAsync(
        string login,
        int? ignorarId = null,
        CancellationToken cancellationToken = default);

    Task<int> ContarAdministradoresAtivosAsync(
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default);
}

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

public interface ILancamentoRepository
{
    Task<IReadOnlyList<Lancamento>> ListarAsync(
        string? pesquisa = null,
        DateTime? dataInicial = null,
        DateTime? dataFinal = null,
        int? vendedorId = null,
        StatusLancamento? status = null,
        CancellationToken cancellationToken = default);

    Task<Lancamento?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default);

    Task ExcluirAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default);
}