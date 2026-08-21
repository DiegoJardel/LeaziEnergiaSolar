using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class CategoriaEquipamentoRepository : ICategoriaEquipamentoRepository
{
    private readonly LeaziDbContext _db;

    public CategoriaEquipamentoRepository(
        LeaziDbContext db) =>
        _db = db;

    public async Task<IReadOnlyList<CategoriaEquipamento>> ListarAsync(
        string? pesquisa,
        bool? ativo,
        CancellationToken cancellationToken = default)
    {
        var query = _db.CategoriasEquipamento
            .AsNoTracking()
            .AsQueryable();

        if (ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();
            query = query.Where(x => x.Descricao.Contains(termo));
        }

        return await query
            .OrderByDescending(x => x.Ativo)
            .ThenBy(x => x.Descricao)
            .ToListAsync(cancellationToken);
    }

    public Task<CategoriaEquipamento?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _db.CategoriasEquipamento.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);

    public Task<bool> ExisteDescricaoAsync(
        string descricao,
        int? ignorarId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizada = descricao.Trim().ToUpperInvariant();

        return _db.CategoriasEquipamento.AnyAsync(
            x => x.Descricao == normalizada &&
                 (!ignorarId.HasValue || x.Id != ignorarId.Value),
            cancellationToken);
    }

    public Task<bool> PossuiEquipamentosAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _db.Equipamentos.AnyAsync(
            x => x.CategoriaEquipamentoId == id,
            cancellationToken);

    public async Task AdicionarAsync(
        CategoriaEquipamento categoria,
        CancellationToken cancellationToken = default)
    {
        await _db.CategoriasEquipamento.AddAsync(
            categoria,
            cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        CategoriaEquipamento categoria,
        CancellationToken cancellationToken = default)
    {
        _db.CategoriasEquipamento.Update(categoria);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
