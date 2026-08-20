using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IModeloEquipamentoService
{
    Task<IReadOnlyList<ModeloEquipamentoDto>> ListarAsync(
        int marcaId,
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default);

    Task<ModeloEquipamentoDto?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarModeloEquipamentoDto dto,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> ExcluirAsync(
        int id,
        CancellationToken cancellationToken = default);
}