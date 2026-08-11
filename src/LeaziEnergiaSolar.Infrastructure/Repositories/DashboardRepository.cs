using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly LeaziDbContext _dbContext;

    public DashboardRepository(LeaziDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<decimal> ObterTotalVendidoAsync(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default)
    {
        var valores = await _dbContext.Lancamentos
            .AsNoTracking()
            .Where(lancamento =>
                lancamento.DataVenda >= inicio &&
                lancamento.DataVenda < fim)
            .Select(lancamento => lancamento.ValorVenda)
            .ToListAsync(cancellationToken);

        return valores.Sum();
    }

    public async Task<decimal> ObterTotalComissaoAsync(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default)
    {
        var valores = await _dbContext.Lancamentos
            .AsNoTracking()
            .Where(lancamento =>
                lancamento.DataVenda >= inicio &&
                lancamento.DataVenda < fim)
            .Select(lancamento => lancamento.ValorComissao)
            .ToListAsync(cancellationToken);

        return valores.Sum();
    }

    public Task<int> ContarLancamentosAsync(
        DateTime inicio,
        DateTime fim,
        StatusLancamento? status = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Lancamentos
            .AsNoTracking()
            .Where(lancamento =>
                lancamento.DataVenda >= inicio &&
                lancamento.DataVenda < fim);

        if (status.HasValue)
        {
            consulta = consulta.Where(lancamento =>
                lancamento.Status == status.Value);
        }

        return consulta.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Lancamento>>
        ObterUltimosLancamentosAsync(
            DateTime inicio,
            DateTime fim,
            int quantidade,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.Lancamentos
            .AsNoTracking()
            .Include(lancamento => lancamento.Vendedor)
            .Where(lancamento =>
                lancamento.DataVenda >= inicio &&
                lancamento.DataVenda < fim)
            .OrderByDescending(lancamento =>
                lancamento.DataVenda)
            .ThenByDescending(lancamento =>
                lancamento.Id)
            .Take(quantidade)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Lancamento>>
        ObterLancamentosDoAnoAsync(
            int ano,
            CancellationToken cancellationToken = default)
    {
        var inicio = new DateTime(
            ano,
            month: 1,
            day: 1);

        var fim = inicio.AddYears(1);

        return await _dbContext.Lancamentos
            .AsNoTracking()
            .Where(lancamento =>
                lancamento.DataVenda >= inicio &&
                lancamento.DataVenda < fim)
            .ToListAsync(cancellationToken);
    }
}