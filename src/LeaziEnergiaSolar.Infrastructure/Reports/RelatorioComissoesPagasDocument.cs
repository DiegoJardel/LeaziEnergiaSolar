using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Infrastructure.Reports;

public sealed class RelatorioComissoesPagasDocument
    : RelatorioComissoesDocumentoBase
{
    protected override string Titulo =>
        "Relatório de Comissões Pagas";

    protected override string Subtitulo =>
        "Comissões liquidadas no período";

    protected override string Assunto =>
        "Relatório de comissões pagas";

    protected override bool ExibirDataPagamento =>
        true;

    protected override bool ExibirStatus =>
        false;

    protected override bool ExibirResumoPorVendedor =>
        true;

    public RelatorioComissoesPagasDocument(
        IReadOnlyList<LancamentoDto> lancamentos,
        FiltroRelatorioComissaoDto filtro,
        byte[]? logo = null)
        : base(
            lancamentos
                .Where(
                    item =>
                        item.Status ==
                        StatusLancamento.Pago)
                .OrderByDescending(
                    item =>
                        item.DataPagamento)
                .ThenBy(
                    item =>
                        item.VendedorNome)
                .ToList(),
            filtro,
            logo)
    {
    }
}