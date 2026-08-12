using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LeaziEnergiaSolar.Infrastructure.Data;

public sealed class LeaziDbContextFactory :
    IDesignTimeDbContextFactory<LeaziDbContext>
{
    public LeaziDbContext CreateDbContext(string[] args)
    {
        var pastaDados = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "LeaziEnergiaSolar");

        Directory.CreateDirectory(pastaDados);

        var caminhoBanco = Path.Combine(
            pastaDados,
            "leazi.db");

        var optionsBuilder =
            new DbContextOptionsBuilder<LeaziDbContext>();

        optionsBuilder.UseSqlite(
            $"Data Source={caminhoBanco}",
            sqliteOptions =>
            {
                sqliteOptions.MigrationsAssembly(
                    typeof(LeaziDbContext).Assembly.FullName);
            });

        return new LeaziDbContext(
            optionsBuilder.Options);
    }
}