using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class EquipamentoRepository : IEquipamentoRepository
{
    private readonly LeaziDbContext _db;

    public EquipamentoRepository(
        LeaziDbContext db) =>
        _db = db;

    public async Task<IReadOnlyList<Equipamento>> ListarAsync(
        string? pesquisa,
        int? categoriaId,
        int? marcaId,
        bool? ativo,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Equipamentos
            .AsNoTracking()
            .Include(x => x.CategoriaEquipamento)
            .Include(x => x.Marca)
            .Include(x => x.UnidadeMedida)
            .AsQueryable();

        if (ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == ativo.Value);
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(x =>
                x.CategoriaEquipamentoId == categoriaId.Value);
        }

        if (marcaId.HasValue)
        {
            query = query.Where(x => x.MarcaId == marcaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();

            query = query.Where(x =>
                x.Descricao.Contains(termo) ||
                (x.Marca != null && x.Marca.Nome.Contains(termo)) ||
                (x.Modelo != null && x.Modelo.Contains(termo)) ||
                x.Id.ToString().Contains(termo));
        }

        return await query
            .OrderByDescending(x => x.Ativo)
            .ThenBy(x => x.Descricao)
            .ToListAsync(cancellationToken);
    }

    public Task<Equipamento?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _db.Equipamentos
            .Include(x => x.CategoriaEquipamento)
            .Include(x => x.Marca)
            .Include(x => x.UnidadeMedida)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

    public Task<bool> ExisteDuplicadoAsync(
        string descricao,
        int? marcaId,
        string? modelo,
        int? ignorarId = null,
        CancellationToken cancellationToken = default)
    {
        var descricaoNormalizada =
            descricao.Trim().ToUpperInvariant();
        var modeloNormalizado = string.IsNullOrWhiteSpace(modelo)
            ? null
            : modelo.Trim().ToUpperInvariant();

        return _db.Equipamentos.AnyAsync(
            x => x.Descricao == descricaoNormalizada &&
                 x.MarcaId == marcaId &&
                 x.Modelo == modeloNormalizado &&
                 (!ignorarId.HasValue || x.Id != ignorarId.Value),
            cancellationToken);
    }

    public async Task AdicionarAsync(
        Equipamento equipamento,
        CancellationToken cancellationToken = default)
    {
        await _db.Equipamentos.AddAsync(
            equipamento,
            cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        Equipamento equipamento,
        CancellationToken cancellationToken = default)
    {
        _db.Equipamentos.Update(equipamento);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
