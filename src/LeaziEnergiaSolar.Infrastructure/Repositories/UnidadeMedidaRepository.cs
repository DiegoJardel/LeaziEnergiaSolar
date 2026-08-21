using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class UnidadeMedidaRepository : IUnidadeMedidaRepository
{
    private readonly LeaziDbContext _db;

    public UnidadeMedidaRepository(
        LeaziDbContext db) =>
        _db = db;

    public async Task<IReadOnlyList<UnidadeMedida>> ListarAsync(
        string? pesquisa,
        bool? ativo,
        CancellationToken cancellationToken = default)
    {
        var query = _db.UnidadesMedida
            .AsNoTracking()
            .AsQueryable();

        if (ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();
            query = query.Where(x =>
                x.Sigla.Contains(termo) ||
                x.Descricao.Contains(termo));
        }

        return await query
            .OrderByDescending(x => x.Ativo)
            .ThenBy(x => x.Sigla)
            .ToListAsync(cancellationToken);
    }

    public Task<UnidadeMedida?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _db.UnidadesMedida.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);

    public Task<bool> ExisteSiglaAsync(
        string sigla,
        int? ignorarId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizada = sigla.Trim().ToUpperInvariant();

        return _db.UnidadesMedida.AnyAsync(
            x => x.Sigla == normalizada &&
                 (!ignorarId.HasValue || x.Id != ignorarId.Value),
            cancellationToken);
    }

    public Task<bool> PossuiEquipamentosAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _db.Equipamentos.AnyAsync(
            x => x.UnidadeMedidaId == id,
            cancellationToken);

    public async Task AdicionarAsync(
        UnidadeMedida unidade,
        CancellationToken cancellationToken = default)
    {
        await _db.UnidadesMedida.AddAsync(
            unidade,
            cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        UnidadeMedida unidade,
        CancellationToken cancellationToken = default)
    {
        _db.UnidadesMedida.Update(unidade);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
