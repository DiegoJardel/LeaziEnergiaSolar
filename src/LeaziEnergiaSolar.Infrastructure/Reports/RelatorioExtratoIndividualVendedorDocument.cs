using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Domain.Entities;

namespace LeaziEnergiaSolar.Infrastructure.Reports;

public sealed class RelatorioExtratoIndividualVendedorDocument
    : RelatorioComissoesDocumentoBase
{
    protected override string Titulo =>
        "Extrato Individual do Vendedor";

    protected override string Subtitulo =>
        ObterSubtitulo();

    protected override string Assunto =>
        "Extrato individual de vendas e comissões";

    protected override bool ExibirDataPagamento =>
        true;

    protected override bool ExibirStatus =>
        true;

    protected override bool ExibirResumoPorVendedor =>
        false;

    public RelatorioExtratoIndividualVendedorDocument(
        IReadOnlyList<LancamentoDto> lancamentos,
        FiltroRelatorioComissaoDto filtro,
        byte[]? logo = null)
        : base(
            lancamentos
                .Where(
                    item =>
                        !filtro.VendedorId.HasValue ||
                        item.VendedorId ==
                        filtro.VendedorId.Value)
                .OrderByDescending(
                    item =>
                        item.DataVenda)
                .ToList(),
            filtro,
            logo)
    {
    }

    private string ObterSubtitulo()
    {
        var vendedor =
            Lancamentos
                .Select(
                    item =>
                        item.VendedorNome)
                .FirstOrDefault();

        return string.IsNullOrWhiteSpace(
                vendedor)
            ? "Demonstrativo individual de comissões"
            : $"Vendedor: {vendedor}";
    }
}