using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Domain.Interfaces;

public interface IFornecedorRepository
{
    Task<IReadOnlyList<Fornecedor>> ListarAsync(string? pesquisa, bool? ativo, CancellationToken cancellationToken = default);
    Task<Fornecedor?> ObterAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteDocumentoAsync(string documento, int? ignorarId = null, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Fornecedor entidade, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Fornecedor entidade, CancellationToken cancellationToken = default);
}
