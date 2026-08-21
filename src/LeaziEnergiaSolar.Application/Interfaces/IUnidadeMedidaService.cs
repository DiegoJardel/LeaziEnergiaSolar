using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;
public interface IUnidadeMedidaService
{
    Task<IReadOnlyList<UnidadeMedidaDto>> ListarAsync(string? pesquisa = null, bool? ativo = null, CancellationToken cancellationToken = default);
    Task<UnidadeMedidaDto?> ObterAsync(int id, CancellationToken cancellationToken = default);
    Task<ResultadoOperacaoDto> SalvarAsync(SalvarUnidadeMedidaDto dto, CancellationToken cancellationToken = default);
    Task<ResultadoOperacaoDto> AlterarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default);
}
