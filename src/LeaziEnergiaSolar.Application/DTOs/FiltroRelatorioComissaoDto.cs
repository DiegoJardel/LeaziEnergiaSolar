using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class FiltroRelatorioComissaoDto
{
    public TipoRelatorioComissao TipoRelatorio { get; init; }

    public DateTime? DataVendaInicial { get; init; }

    public DateTime? DataVendaFinal { get; init; }

    public DateTime? DataPagamentoInicial { get; init; }

    public DateTime? DataPagamentoFinal { get; init; }

    public int? VendedorId { get; init; }

    public int? ClienteId { get; init; }

    public StatusLancamento? Status { get; init; }

    public string Pesquisa { get; init; } =
        string.Empty;

    public int? UsuarioEmissorId { get; init; }

    public string UsuarioEmissor { get; init; } =
        string.Empty;
}