using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class VendedorRepository : IVendedorRepository
{
    private readonly LeaziDbContext _dbContext;

    public VendedorRepository(
        LeaziDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Vendedor>> ListarAsync(
        string? pesquisa = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Vendedores
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();

            consulta = consulta.Where(vendedor =>
                vendedor.Nome.Contains(termo) ||
                (vendedor.CpfCnpj != null &&
                 vendedor.CpfCnpj.Contains(termo)) ||
                (vendedor.Email != null &&
                 vendedor.Email.Contains(termo)));
        }

        return await consulta
            .OrderByDescending(vendedor => vendedor.Ativo)
            .ThenBy(vendedor => vendedor.Nome)
            .ToListAsync(cancellationToken);
    }

    public Task<Vendedor?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Vendedores
            .FirstOrDefaultAsync(
                vendedor => vendedor.Id == id,
                cancellationToken);
    }

    public Task<bool> ExisteDocumentoAsync(
        string cpfCnpj,
        int? ignorarId = null,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Vendedores.AnyAsync(
            vendedor =>
                vendedor.CpfCnpj == cpfCnpj &&
                (!ignorarId.HasValue ||
                 vendedor.Id != ignorarId.Value),
            cancellationToken);
    }

    public Task<bool> PossuiLancamentosAsync(
        int vendedorId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Lancamentos
            .AsNoTracking()
            .AnyAsync(
                lancamento =>
                    lancamento.VendedorId == vendedorId,
                cancellationToken);
    }

    public async Task AdicionarAsync(
        Vendedor vendedor,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Vendedores.AddAsync(
            vendedor,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task AtualizarAsync(
        Vendedor vendedor,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Vendedores.Update(vendedor);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task ExcluirAsync(
        Vendedor vendedor,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Vendedores.Remove(vendedor);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}