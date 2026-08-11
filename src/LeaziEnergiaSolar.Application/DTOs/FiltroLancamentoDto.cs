using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class FiltroLancamentoDto
{
    public string? Pesquisa { get; init; }

    public DateTime? DataInicial { get; init; }

    public DateTime? DataFinal { get; init; }

    public int? VendedorId { get; init; }

    public StatusLancamento? Status { get; init; }
}
