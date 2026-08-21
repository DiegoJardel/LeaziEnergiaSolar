using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;
public interface IEquipamentoService
{
    Task<IReadOnlyList<EquipamentoDto>> ListarAsync(string? pesquisa = null, int? categoriaId = null, int? marcaId = null, bool? ativo = null, CancellationToken cancellationToken = default);
    Task<EquipamentoDto?> ObterAsync(int id, CancellationToken cancellationToken = default);
    Task<ResultadoOperacaoDto> SalvarAsync(SalvarEquipamentoDto dto, CancellationToken cancellationToken = default);
    Task<ResultadoOperacaoDto> AlterarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default);
}
