using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Infrastructure.Reports;

public sealed class RelatorioComissoesPorVendedorDocument
    : RelatorioComissoesDocumentoBase
{
    protected override string Titulo =>
        "Relatório de Comissões por Vendedor";

    protected override string Subtitulo =>
        "Resumo e detalhamento por vendedor";

    protected override string Assunto =>
        "Relatório de comissões agrupadas por vendedor";

    protected override bool ExibirDataPagamento =>
        true;

    protected override bool ExibirStatus =>
        true;

    protected override bool ExibirResumoPorVendedor =>
        true;

    public RelatorioComissoesPorVendedorDocument(
        IReadOnlyList<LancamentoDto> lancamentos,
        FiltroRelatorioComissaoDto filtro,
        byte[]? logo = null)
        : base(
            lancamentos
                .OrderBy(
                    item =>
                        item.VendedorNome)
                .ThenByDescending(
                    item =>
                        item.DataVenda)
                .ToList(),
            filtro,
            logo)
    {
    }
}