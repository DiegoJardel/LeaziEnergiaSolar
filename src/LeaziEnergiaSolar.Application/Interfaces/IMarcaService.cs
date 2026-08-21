using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IMarcaService
{
    Task<IReadOnlyList<MarcaDto>> ListarAsync(
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default);

    Task<MarcaDto?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarMarcaDto dto,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> ExcluirAsync(
        int id,
        CancellationToken cancellationToken = default);
}