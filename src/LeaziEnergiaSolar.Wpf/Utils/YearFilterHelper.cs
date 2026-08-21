namespace LeaziEnergiaSolar.Wpf.Utils;

public static class YearFilterHelper
{
    private const int AnoInicial = 1900;

    public static IReadOnlyList<int> CriarAnosDisponiveis()
    {
        var anoAtual = DateTime.Today.Year;

        return Enumerable
            .Range(AnoInicial, anoAtual - AnoInicial + 1)
            .OrderByDescending(ano => ano)
            .ToList();
    }
}
