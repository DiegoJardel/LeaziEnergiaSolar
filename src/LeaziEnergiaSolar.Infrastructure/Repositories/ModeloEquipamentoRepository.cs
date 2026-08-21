using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class ModeloEquipamentoRepository(
    LeaziDbContext db)
    : IModeloEquipamentoRepository
{
    public async Task<IReadOnlyList<ModeloEquipamento>> ListarAsync(
        int marcaId,
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default)
    {
        var query = db.ModelosEquipamentos
            .AsNoTracking()
            .Include(x => x.Marca)
            .Where(x => x.MarcaId == marcaId);

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa
                .Trim()
                .ToUpper();

            query = query.Where(x =>
                x.Nome.ToUpper().Contains(termo) ||
                (x.Observacao != null &&
                 x.Observacao.ToUpper().Contains(termo)));
        }

        if (ativo.HasValue)
        {
            query = query.Where(
                x => x.Ativo == ativo.Value);
        }

        return await query
            .OrderByDescending(x => x.Ativo)
            .ThenBy(x => x.Nome)
            .ToListAsync(cancellationToken);
    }

    public Task<ModeloEquipamento?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return db.ModelosEquipamentos
            .Include(x => x.Marca)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<bool> ExisteNomeAsync(
        int marcaId,
        string nome,
        int? ignorarId = null,
        CancellationToken cancellationToken = default)
    {
        var nomeNormalizado = nome
            .Trim()
            .ToUpper();

        return db.ModelosEquipamentos
            .AnyAsync(
                x =>
                    x.MarcaId == marcaId &&
                    x.Nome.ToUpper() == nomeNormalizado &&
                    (!ignorarId.HasValue ||
                     x.Id != ignorarId.Value),
                cancellationToken);
    }

    public Task<bool> ExisteAlgumPorMarcaAsync(
        int marcaId,
        CancellationToken cancellationToken = default)
    {
        return db.ModelosEquipamentos
            .AnyAsync(
                x => x.MarcaId == marcaId,
                cancellationToken);
    }

    public async Task AdicionarAsync(
        ModeloEquipamento modelo,
        CancellationToken cancellationToken = default)
    {
        await db.ModelosEquipamentos.AddAsync(
            modelo,
            cancellationToken);

        await db.SaveChangesAsync(
            cancellationToken);
    }

    public async Task AtualizarAsync(
        ModeloEquipamento modelo,
        CancellationToken cancellationToken = default)
    {
        db.ModelosEquipamentos.Update(
            modelo);

        await db.SaveChangesAsync(
            cancellationToken);
    }

    public async Task ExcluirAsync(
        ModeloEquipamento modelo,
        CancellationToken cancellationToken = default)
    {
        db.ModelosEquipamentos.Remove(
            modelo);

        await db.SaveChangesAsync(
            cancellationToken);
    }
}