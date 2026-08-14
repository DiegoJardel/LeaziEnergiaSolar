using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class ClienteRepository : IClienteRepository
{
    private readonly LeaziDbContext _dbContext;

    public ClienteRepository(
        LeaziDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Cliente>> ListarAsync(
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Clientes
            .AsNoTracking()
            .AsQueryable();

        if (ativo.HasValue)
        {
            consulta = consulta.Where(cliente =>
                cliente.Ativo == ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();
            var documento = new string(termo.Where(char.IsDigit).ToArray());

            consulta = consulta.Where(cliente =>
                cliente.NomeRazaoSocial.Contains(termo) ||
                (cliente.NomeFantasia != null &&
                 cliente.NomeFantasia.Contains(termo)) ||
                (cliente.CpfCnpj != null &&
                 (cliente.CpfCnpj.Contains(termo) ||
                  (!string.IsNullOrEmpty(documento) &&
                   cliente.CpfCnpj.Contains(documento)))) ||
                (cliente.Telefone != null &&
                 cliente.Telefone.Contains(documento)) ||
                (cliente.WhatsApp != null &&
                 cliente.WhatsApp.Contains(documento)) ||
                (cliente.Cidade != null &&
                 cliente.Cidade.Contains(termo)));
        }

        return await consulta
            .OrderByDescending(cliente => cliente.Ativo)
            .ThenBy(cliente => cliente.NomeRazaoSocial)
            .ToListAsync(cancellationToken);
    }

    public Task<Cliente?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Clientes
            .FirstOrDefaultAsync(
                cliente => cliente.Id == id,
                cancellationToken);
    }

    public Task<bool> ExisteDocumentoAsync(
        string cpfCnpj,
        int? ignorarId = null,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Clientes.AnyAsync(
            cliente =>
                cliente.CpfCnpj == cpfCnpj &&
                (!ignorarId.HasValue ||
                 cliente.Id != ignorarId.Value),
            cancellationToken);
    }

    public async Task AdicionarAsync(
        Cliente cliente,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Clientes.AddAsync(
            cliente,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task AtualizarAsync(
        Cliente cliente,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Clientes.Update(cliente);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
