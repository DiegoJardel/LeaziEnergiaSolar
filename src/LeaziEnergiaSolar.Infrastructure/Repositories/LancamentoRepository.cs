using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class LancamentoRepository : ILancamentoRepository
{
    private readonly LeaziDbContext _dbContext;

    public LancamentoRepository(LeaziDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Lancamento>> ListarAsync(
        string? pesquisa = null,
        DateTime? dataInicial = null,
        DateTime? dataFinal = null,
        int? vendedorId = null,
        StatusLancamento? status = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Lancamentos
            .AsNoTracking()
            .Include(lancamento => lancamento.Vendedor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();

            consulta = consulta.Where(lancamento =>
                lancamento.Cliente.Contains(termo) ||
                lancamento.Vendedor.Nome.Contains(termo) ||
                (lancamento.CpfCnpjCliente != null &&
                 lancamento.CpfCnpjCliente.Contains(termo)));
        }

        if (dataInicial.HasValue)
        {
            consulta = consulta.Where(lancamento =>
                lancamento.DataVenda.Date >= dataInicial.Value.Date);
        }

        if (dataFinal.HasValue)
        {
            consulta = consulta.Where(lancamento =>
                lancamento.DataVenda.Date <= dataFinal.Value.Date);
        }

        if (vendedorId.HasValue)
        {
            consulta = consulta.Where(lancamento =>
                lancamento.VendedorId == vendedorId.Value);
        }

        if (status.HasValue)
        {
            consulta = consulta.Where(lancamento =>
                lancamento.Status == status.Value);
        }

        return await consulta
            .OrderByDescending(lancamento => lancamento.DataVenda)
            .ThenByDescending(lancamento => lancamento.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Lancamento?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Lancamentos
            .FirstOrDefaultAsync(
                lancamento => lancamento.Id == id,
                cancellationToken);
    }

    public async Task AdicionarAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Lancamentos.AddAsync(
            lancamento,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Lancamentos.Update(lancamento);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ExcluirAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Lancamentos.Remove(lancamento);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
