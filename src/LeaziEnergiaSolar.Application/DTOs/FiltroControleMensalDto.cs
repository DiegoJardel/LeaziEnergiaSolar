using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class FiltroControleMensalDto
{
    public int Mes { get; init; }

    public int Ano { get; init; }

    public int? VendedorId { get; init; }

    public StatusLancamento? Status { get; init; }

    public string? Pesquisa { get; init; }
}
