using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class LocalidadeRepository : ILocalidadeRepository
{
    private readonly LeaziDbContext _dbContext;

    public LocalidadeRepository(LeaziDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Estado>> ListarEstadosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Estados
            .AsNoTracking()
            .Where(estado => estado.Ativo)
            .OrderBy(estado => estado.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Municipio>> ListarMunicipiosAsync(string codigoIbgeEstado, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Municipios
            .AsNoTracking()
            .Include(municipio => municipio.Estado)
            .Where(municipio => municipio.Ativo && municipio.Estado.CodigoIbge == codigoIbgeEstado)
            .OrderBy(municipio => municipio.Nome)
            .ToListAsync(cancellationToken);
    }

    public Task<Estado?> ObterEstadoPorCodigoIbgeAsync(string codigoIbge, CancellationToken cancellationToken = default)
    {
        return _dbContext.Estados.FirstOrDefaultAsync(estado => estado.CodigoIbge == codigoIbge, cancellationToken);
    }

    public Task<Municipio?> ObterMunicipioPorCodigoIbgeAsync(string codigoIbge, CancellationToken cancellationToken = default)
    {
        return _dbContext.Municipios.FirstOrDefaultAsync(municipio => municipio.CodigoIbge == codigoIbge, cancellationToken);
    }

    public async Task SalvarEstadosAsync(IEnumerable<Estado> estados, CancellationToken cancellationToken = default)
    {
        foreach (var estado in estados)
        {
            var existente = await _dbContext.Estados.FirstOrDefaultAsync(item => item.CodigoIbge == estado.CodigoIbge, cancellationToken);
            if (existente is null)
            {
                await _dbContext.Estados.AddAsync(estado, cancellationToken);
            }
            else
            {
                existente.Nome = estado.Nome;
                existente.Sigla = estado.Sigla;
                existente.Ativo = estado.Ativo;
                existente.DataAtualizacao = DateTime.Now;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SalvarMunicipiosAsync(IEnumerable<Municipio> municipios, CancellationToken cancellationToken = default)
    {
        foreach (var municipio in municipios)
        {
            var existente = await _dbContext.Municipios.FirstOrDefaultAsync(item => item.CodigoIbge == municipio.CodigoIbge, cancellationToken);
            if (existente is null)
            {
                await _dbContext.Municipios.AddAsync(municipio, cancellationToken);
            }
            else
            {
                existente.Nome = municipio.Nome;
                existente.EstadoId = municipio.EstadoId;
                existente.Ativo = municipio.Ativo;
                existente.DataAtualizacao = DateTime.Now;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
