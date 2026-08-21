using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class LancamentoDto
{
    public int Id { get; init; }

    public DateTime DataVenda { get; init; }

    public string Cliente { get; init; } =
        string.Empty;

    public string CpfCnpjCliente { get; init; } =
        string.Empty;

    public int? ClienteId { get; init; }

    public int? UsuarioId { get; init; }

    public int VendedorId { get; init; }

    public string VendedorNome { get; init; } =
        string.Empty;

    public decimal ValorVenda { get; init; }

    public decimal PercentualComissao { get; init; }

    public decimal ValorComissao { get; init; }

    public StatusLancamento Status { get; init; }

    public string StatusDescricao =>
        Status == StatusLancamento.Pago
            ? "Pago"
            : "Pendente";

    public DateTime? DataPagamento { get; init; }

    public DateTime DataCadastro { get; init; }

    public DateTime? DataAtualizacao { get; init; }

    public string Observacao { get; init; } =
        string.Empty;
}