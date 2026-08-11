using System.IO;

namespace LeaziEnergiaSolar.Wpf.Services;

public static class AppPaths
{
    private const string NomeAplicacao = "LeaziEnergiaSolar";

    public static string PastaDados => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        NomeAplicacao);

    public static string PastaBackups => Path.Combine(
        PastaDados,
        "Backups");

    public static string PastaLogs => Path.Combine(
        PastaDados,
        "Logs");

    public static string BancoDados => Path.Combine(
        PastaDados,
        "leazi.db");

    public static void PrepararPastas()
    {
        Directory.CreateDirectory(PastaDados);
        Directory.CreateDirectory(PastaBackups);
        Directory.CreateDirectory(PastaLogs);

        MigrarBancoLegado();
    }

    private static void MigrarBancoLegado()
    {
        var bancoLegado = Path.Combine(
            AppContext.BaseDirectory,
            "leazi.db");

        if (File.Exists(BancoDados) || !File.Exists(bancoLegado))
        {
            return;
        }

        File.Copy(
            bancoLegado,
            BancoDados,
            overwrite: false);
    }
}
