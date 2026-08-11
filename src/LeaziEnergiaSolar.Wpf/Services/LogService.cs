using System.IO;
using System.Text;

namespace LeaziEnergiaSolar.Wpf.Services;

public interface ILogService
{
    void RegistrarErro(Exception exception, string contexto);
}

public sealed class LogService : ILogService
{
    private static readonly object Sincronizacao = new();

    public void RegistrarErro(Exception exception, string contexto)
    {
        try
        {
            AppPaths.PrepararPastas();

            var arquivo = Path.Combine(
                AppPaths.PastaLogs,
                $"leazi-{DateTime.Now:yyyy-MM}.log");

            var texto = new StringBuilder()
                .AppendLine(new string('-', 80))
                .AppendLine($"Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                .AppendLine($"Contexto: {contexto}")
                .AppendLine($"Tipo: {exception.GetType().FullName}")
                .AppendLine($"Mensagem: {exception.Message}")
                .AppendLine(exception.StackTrace)
                .ToString();

            lock (Sincronizacao)
            {
                File.AppendAllText(
                    arquivo,
                    texto,
                    Encoding.UTF8);
            }
        }
        catch
        {
            // O log nunca deve interromper o funcionamento do sistema.
        }
    }
}
