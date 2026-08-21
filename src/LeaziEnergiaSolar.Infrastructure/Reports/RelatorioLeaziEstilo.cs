using System.Globalization;

namespace LeaziEnergiaSolar.Infrastructure.Reports;

public static class RelatorioLeaziEstilo
{
    public const string VerdeEscuro =
        "#0B5D2A";

    public const string VerdePrincipal =
        "#16803C";

    public const string VerdeClaro =
        "#EAF4EC";

    public const string Vermelho =
        "#C62828";

    public const string VermelhoClaro =
        "#FDECEC";

    public const string CinzaTexto =
        "#667085";

    public const string CinzaTitulo =
        "#344054";

    public const string CinzaBorda =
        "#D0D5DD";

    public const string CinzaFundo =
        "#F5F7F6";

    public const string Branco =
        "#FFFFFF";

    public const string Preto =
        "#101828";

    public static readonly CultureInfo CulturaBrasileira =
        CultureInfo.GetCultureInfo(
            "pt-BR");

    public static string FormatarMoeda(
        decimal valor)
    {
        return valor.ToString(
            "C2",
            CulturaBrasileira);
    }

    public static string FormatarPercentual(
        decimal valor)
    {
        return valor.ToString(
            "N2",
            CulturaBrasileira) + "%";
    }

    public static string FormatarData(
        DateTime? data)
    {
        return data.HasValue
            ? data.Value.ToString(
                "dd/MM/yyyy",
                CulturaBrasileira)
            : "-";
    }

    public static string FormatarDataHora(
        DateTime? data)
    {
        return data.HasValue
            ? data.Value.ToString(
                "dd/MM/yyyy HH:mm",
                CulturaBrasileira)
            : "-";
    }
}
