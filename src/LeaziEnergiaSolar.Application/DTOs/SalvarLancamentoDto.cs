using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class SalvarLancamentoDto
{
    public int? Id { get; init; }

    public DateTime DataVenda { get; init; }

    public string Cliente { get; init; } = string.Empty;

    public string CpfCnpjCliente { get; init; } = string.Empty;

    public int VendedorId { get; init; }

    public decimal ValorVenda { get; init; }

    public decimal PercentualComissao { get; init; }

    public StatusLancamento Status { get; init; }

    public string Observacao { get; init; } = string.Empty;
}
