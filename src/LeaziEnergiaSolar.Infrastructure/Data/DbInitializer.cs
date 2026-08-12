using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        LeaziDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(
            cancellationToken);

        var administradorExiste = await dbContext.Usuarios
            .AnyAsync(
                usuario => usuario.Login == "admin",
                cancellationToken);

        if (administradorExiste)
        {
            return;
        }

        await dbContext.Usuarios.AddAsync(
            new Usuario
            {
                Nome = "Administrador",
                Login = "admin",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(
                    "admin",
                    workFactor: 12),
                Perfil = PerfilUsuario.Administrador,
                Ativo = true
            },
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}