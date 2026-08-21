using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class FornecedorRepository : IFornecedorRepository
{
    private readonly LeaziDbContext _db;

    public FornecedorRepository(
        LeaziDbContext db) =>
        _db = db;

    public async Task<IReadOnlyList<Fornecedor>> ListarAsync(
        string? pesquisa,
        bool? ativo,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Fornecedores
            .AsNoTracking()
            .AsQueryable();

        if (ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();
            var numeros = new string(
                termo.Where(char.IsDigit).ToArray());

            query = query.Where(x =>
                x.NomeRazaoSocial.Contains(termo) ||
                (x.NomeFantasia != null &&
                 x.NomeFantasia.Contains(termo)) ||
                (x.CpfCnpj != null &&
                 x.CpfCnpj.Contains(numeros)) ||
                (x.Telefone != null &&
                 x.Telefone.Contains(numeros)));
        }

        return await query
            .OrderByDescending(x => x.Ativo)
            .ThenBy(x => x.NomeRazaoSocial)
            .ToListAsync(cancellationToken);
    }

    public Task<Fornecedor?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _db.Fornecedores.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);

    public Task<bool> ExisteDocumentoAsync(
        string documento,
        int? ignorarId = null,
        CancellationToken cancellationToken = default) =>
        _db.Fornecedores.AnyAsync(
            x => x.CpfCnpj == documento &&
                 (!ignorarId.HasValue || x.Id != ignorarId.Value),
            cancellationToken);

    public async Task AdicionarAsync(
        Fornecedor fornecedor,
        CancellationToken cancellationToken = default)
    {
        await _db.Fornecedores.AddAsync(
            fornecedor,
            cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        Fornecedor fornecedor,
        CancellationToken cancellationToken = default)
    {
        _db.Fornecedores.Update(fornecedor);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
