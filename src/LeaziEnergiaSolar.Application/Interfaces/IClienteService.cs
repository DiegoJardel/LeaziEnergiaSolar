using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IClienteService
{
    Task<IReadOnlyList<ClienteDto>> ListarAsync(
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarClienteDto cliente,
        CancellationToken cancellationToken = default);

    Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default);
}
