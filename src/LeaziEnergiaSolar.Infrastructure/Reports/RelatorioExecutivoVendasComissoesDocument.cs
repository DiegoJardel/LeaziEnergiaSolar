using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Infrastructure.Reports;

public sealed class RelatorioExecutivoVendasComissoesDocument
    : RelatorioComissoesDocumentoBase
{
    protected override string Titulo =>
        "Relatório Executivo de Vendas e Comissões";

    protected override string Subtitulo =>
        "Visão gerencial dos resultados comerciais";

    protected override string Assunto =>
        "Relatório executivo de vendas e comissões";

    protected override bool ExibirDataPagamento =>
        true;

    protected override bool ExibirStatus =>
        true;

    protected override bool ExibirResumoPorVendedor =>
        true;

    protected override bool ExibirIndicadoresExecutivos =>
        true;

    public RelatorioExecutivoVendasComissoesDocument(
        IReadOnlyList<LancamentoDto> lancamentos,
        FiltroRelatorioComissaoDto filtro,
        byte[]? logo = null)
        : base(
            lancamentos
                .OrderByDescending(
                    item =>
                        item.DataVenda)
                .ThenBy(
                    item =>
                        item.VendedorNome)
                .ToList(),
            filtro,
            logo)
    {
    }
}