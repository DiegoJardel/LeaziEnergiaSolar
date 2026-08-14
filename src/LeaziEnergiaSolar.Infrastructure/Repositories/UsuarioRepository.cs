using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly LeaziDbContext _dbContext;

    public UsuarioRepository(LeaziDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Usuario>> ListarAsync(
        string? pesquisa = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Usuarios
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            var termo = pesquisa.Trim();

            consulta = consulta.Where(usuario =>
                usuario.Nome.Contains(termo) ||
                usuario.Login.Contains(termo));
        }

        return await consulta
            .OrderByDescending(usuario => usuario.Ativo)
            .ThenBy(usuario => usuario.Nome)
            .ToListAsync(cancellationToken);
    }

    public Task<Usuario?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios.FirstOrDefaultAsync(
            usuario => usuario.Id == id,
            cancellationToken);
    }

    public Task<Usuario?> ObterPorLoginAsync(
        string login,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(
                usuario => usuario.Login == login,
                cancellationToken);
    }

    public Task<bool> ExisteLoginAsync(
        string login,
        int? ignorarId = null,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios.AnyAsync(
            usuario => usuario.Login == login &&
                       (!ignorarId.HasValue || usuario.Id != ignorarId.Value),
            cancellationToken);
    }

    public Task<int> ContarAdministradoresAtivosAsync(
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios.CountAsync(
            usuario => usuario.Ativo &&
                       usuario.Perfil == PerfilUsuario.Administrador,
            cancellationToken);
    }

    public Task<bool> PossuiLancamentosAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Lancamentos
            .AnyAsync(
                lancamento => lancamento.UsuarioId == usuarioId,
                cancellationToken);
    }

    public async Task ExcluirAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Usuarios.Remove(usuario);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Usuarios.AddAsync(
            usuario,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Usuarios.Update(usuario);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
