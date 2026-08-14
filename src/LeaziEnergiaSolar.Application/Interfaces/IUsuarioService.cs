using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioDto>> ListarAsync(
        int usuarioLogadoId,
        string? pesquisa = null,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> SalvarAsync(
        int usuarioLogadoId,
        SalvarUsuarioDto usuario,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int usuarioLogadoId,
        int usuarioId,
        bool ativo,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> RedefinirSenhaAsync(
        int usuarioLogadoId,
        RedefinirSenhaDto redefinicao,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> ExcluirAsync(
        int usuarioLogadoId,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
