using System.IO;

namespace LeaziEnergiaSolar.Wpf.Services;

public interface IBackupService
{
    Task<string> CriarBackupAsync(
        CancellationToken cancellationToken = default);

    Task CriarBackupAutomaticoAsync(
        CancellationToken cancellationToken = default);
}

public sealed class BackupService : IBackupService
{
    private const int QuantidadeMaximaBackupsAutomaticos = 10;

    public async Task<string> CriarBackupAsync(
        CancellationToken cancellationToken = default)
    {
        AppPaths.PrepararPastas();

        if (!File.Exists(AppPaths.BancoDados))
        {
            throw new FileNotFoundException(
                "O banco de dados ainda não foi criado.",
                AppPaths.BancoDados);
        }

        var destino = Path.Combine(
            AppPaths.PastaBackups,
            $"leazi-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db");

        await CopiarAsync(
            AppPaths.BancoDados,
            destino,
            cancellationToken);

        return destino;
    }

    public async Task CriarBackupAutomaticoAsync(
        CancellationToken cancellationToken = default)
    {
        AppPaths.PrepararPastas();

        if (!File.Exists(AppPaths.BancoDados))
        {
            return;
        }

        var arquivoHoje = Path.Combine(
            AppPaths.PastaBackups,
            $"leazi-auto-{DateTime.Today:yyyyMMdd}.db");

        if (!File.Exists(arquivoHoje))
        {
            await CopiarAsync(
                AppPaths.BancoDados,
                arquivoHoje,
                cancellationToken);
        }

        LimparBackupsAutomaticosAntigos();
    }

    private static async Task CopiarAsync(
        string origem,
        string destino,
        CancellationToken cancellationToken)
    {
        await using var entrada = new FileStream(
            origem,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite);

        await using var saida = new FileStream(
            destino,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);

        await entrada.CopyToAsync(saida, cancellationToken);
    }

    private static void LimparBackupsAutomaticosAntigos()
    {
        var excedentes = Directory
            .EnumerateFiles(
                AppPaths.PastaBackups,
                "leazi-auto-*.db")
            .Select(caminho => new FileInfo(caminho))
            .OrderByDescending(arquivo => arquivo.CreationTimeUtc)
            .Skip(QuantidadeMaximaBackupsAutomaticos);

        foreach (var arquivo in excedentes)
        {
            try
            {
                arquivo.Delete();
            }
            catch
            {
                // A limpeza não deve impedir a abertura do sistema.
            }
        }
    }
}
