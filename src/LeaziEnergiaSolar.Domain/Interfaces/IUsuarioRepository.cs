using LeaziEnergiaSolar.Domain.Entities;

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

    Task<bool> PossuiLancamentosAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task ExcluirAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default);
}