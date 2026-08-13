using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> ListarAsync(
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default);

    Task<Cliente?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteDocumentoAsync(
        string cpfCnpj,
        int? ignorarId = null,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Cliente cliente,
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        Cliente cliente,
        CancellationToken cancellationToken = default);
}
