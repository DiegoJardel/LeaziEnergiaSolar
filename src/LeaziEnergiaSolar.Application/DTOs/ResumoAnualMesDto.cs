using System.Globalization;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class ResumoAnualMesDto
{
    public int Mes { get; init; }

    public decimal TotalVendido { get; init; }

    public decimal TotalComissao { get; init; }

    public int QuantidadeRegistros { get; init; }

    public int QuantidadePagos { get; init; }

    public int QuantidadePendentes { get; init; }

    public double AlturaVendas { get; set; }

    public double AlturaComissoes { get; set; }

    public string MesNome => CultureInfo
        .GetCultureInfo("pt-BR")
        .DateTimeFormat
        .GetMonthName(Mes);

    public string MesAbreviado => CultureInfo
        .GetCultureInfo("pt-BR")
        .DateTimeFormat
        .GetAbbreviatedMonthName(Mes)
        .TrimEnd('.')
        .ToUpperInvariant();
}
