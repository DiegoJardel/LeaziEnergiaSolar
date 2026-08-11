namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class DashboardDto
{
    public decimal TotalVendido { get; init; }

    public decimal TotalComissao { get; init; }

    public int QuantidadeRegistros { get; init; }

    public int QuantidadePagos { get; init; }

    public int QuantidadePendentes { get; init; }

    public IReadOnlyList<ResumoMensalDto> ResumoMensal { get; init; } =
        Array.Empty<ResumoMensalDto>();

    public IReadOnlyList<LancamentoDto> UltimosLancamentos { get; init; } =
        Array.Empty<LancamentoDto>();
}
