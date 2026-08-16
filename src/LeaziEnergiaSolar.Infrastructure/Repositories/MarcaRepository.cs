using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class MarcaRepository : IMarcaRepository
{
    private readonly LeaziDbContext _db;

    public MarcaRepository(
        LeaziDbContext db) =>
        _db = db;

    public async Task<IReadOnlyList<Marca>> ListarAsync(
        string? pesquisa,
        bool? ativo,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Marcas
            .AsNoTracking()
            .AsQueryable();

        if (ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();
            query = query.Where(x => x.Nome.Contains(termo));
        }

        return await query
            .OrderByDescending(x => x.Ativo)
            .ThenBy(x => x.Nome)
            .ToListAsync(cancellationToken);
    }

    public Task<Marca?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _db.Marcas.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);

    public Task<bool> ExisteNomeAsync(
        string nome,
        int? ignorarId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizado = nome.Trim().ToUpperInvariant();

        return _db.Marcas.AnyAsync(
            x => x.Nome == normalizado &&
                 (!ignorarId.HasValue || x.Id != ignorarId.Value),
            cancellationToken);
    }

    public Task<bool> PossuiEquipamentosAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _db.Equipamentos.AnyAsync(
            x => x.MarcaId == id,
            cancellationToken);

    public async Task AdicionarAsync(
        Marca marca,
        CancellationToken cancellationToken = default)
    {
        await _db.Marcas.AddAsync(
            marca,
            cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        Marca marca,
        CancellationToken cancellationToken = default)
    {
        _db.Marcas.Update(marca);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
