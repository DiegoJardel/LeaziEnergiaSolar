using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IFornecedorService
{
    Task<IReadOnlyList<FornecedorDto>> ListarAsync(
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default);

    Task<FornecedorDto?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarFornecedorDto dto,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default);
}