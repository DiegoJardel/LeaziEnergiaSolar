using System.Globalization;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class ResumoMensalDto
{
    public int Mes { get; init; }

    public decimal TotalVendido { get; init; }

    public decimal TotalComissao { get; init; }

    public int QuantidadeRegistros { get; init; }

    public string MesAbreviado => CultureInfo
        .GetCultureInfo("pt-BR")
        .DateTimeFormat
        .GetAbbreviatedMonthName(Mes)
        .TrimEnd('.')
        .ToUpperInvariant();

    public double AlturaVendas { get; set; }

    public double AlturaComissoes { get; set; }
}
