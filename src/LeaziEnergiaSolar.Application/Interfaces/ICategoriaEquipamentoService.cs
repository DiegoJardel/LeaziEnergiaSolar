using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;
public interface ICategoriaEquipamentoService
{
    Task<IReadOnlyList<CategoriaEquipamentoDto>> ListarAsync(string? pesquisa = null, bool? ativo = null, CancellationToken cancellationToken = default);
    Task<CategoriaEquipamentoDto?> ObterAsync(int id, CancellationToken cancellationToken = default);
    Task<ResultadoOperacaoDto> SalvarAsync(SalvarCategoriaEquipamentoDto dto, CancellationToken cancellationToken = default);
    Task<ResultadoOperacaoDto> AlterarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default);
}
