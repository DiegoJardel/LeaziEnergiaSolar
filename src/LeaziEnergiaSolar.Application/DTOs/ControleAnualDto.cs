namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class ControleAnualDto
{
    public decimal TotalVendido { get; init; }

    public decimal TotalComissao { get; init; }

    public int QuantidadeRegistros { get; init; }

    public int QuantidadePagos { get; init; }

    public int QuantidadePendentes { get; init; }

    public IReadOnlyList<ResumoAnualMesDto> Meses { get; init; } =
        Array.Empty<ResumoAnualMesDto>();
}
