using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using PdfContainer =
    QuestPDF.Infrastructure.IContainer;

namespace LeaziEnergiaSolar.Infrastructure.Reports;

public sealed class RelatorioGeralComissoesDocument
    : IDocument
{
    private readonly IReadOnlyList<LancamentoDto>
        _lancamentos;

    private readonly FiltroRelatorioComissaoDto
        _filtro;

    private readonly byte[]?
        _logo;

    public RelatorioGeralComissoesDocument(
        IReadOnlyList<LancamentoDto> lancamentos,
        FiltroRelatorioComissaoDto filtro,
        byte[]? logo = null)
    {
        _lancamentos =
            lancamentos
            ?? throw new ArgumentNullException(
                nameof(lancamentos));

        _filtro =
            filtro
            ?? throw new ArgumentNullException(
                nameof(filtro));

        _logo =
            logo;
    }

    public DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Title =
                "Relatório Geral de Comissões",

            Author =
                "Leazi Energia Solar",

            Subject =
                "Relatório de comissões pagas e pendentes",

            Creator =
                "Sistema Leazi Energia Solar"
        };
    }

    public void Compose(
        IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(
                PageSizes.A4.Landscape());

            page.Margin(
                24);

            page.DefaultTextStyle(
                style => style
                    .FontFamily(
                        Fonts.Arial)
                    .FontSize(
                        8)
                    .FontColor(
                        RelatorioLeaziEstilo.Preto));

            page.Header()
                .Element(
                    containerCabecalho =>
                        ComporCabecalho(
                            containerCabecalho));

            page.Content()
                .PaddingVertical(
                    12)
                .Element(
                    containerConteudo =>
                        ComporConteudo(
                            containerConteudo));

            page.Footer()
                .Element(
                    containerRodape =>
                        ComporRodape(
                            containerRodape));
        });
    }

    private void ComporCabecalho(
        PdfContainer container)
    {
        container
            .BorderBottom(
                2)
            .BorderColor(
                RelatorioLeaziEstilo.VerdeEscuro)
            .PaddingBottom(
                10)
            .Row(row =>
            {
                row.ConstantItem(
                        145)
                    .AlignMiddle()
                    .Element(
                        containerLogo =>
                            ComporLogo(
                                containerLogo));

                row.RelativeItem()
                    .AlignCenter()
                    .AlignMiddle()
                    .Column(column =>
                    {
                        column.Item()
                            .AlignCenter()
                            .Text(
                                "RELATÓRIO GERAL DE COMISSÕES")
                            .FontSize(
                                16)
                            .SemiBold()
                            .FontColor(
                                RelatorioLeaziEstilo
                                    .VerdeEscuro);

                        column.Item()
                            .PaddingTop(
                                3)
                            .AlignCenter()
                            .Text(
                                "Comissões pagas e pendentes")
                            .FontSize(
                                9)
                            .FontColor(
                                RelatorioLeaziEstilo
                                    .CinzaTexto);
                    });

                row.ConstantItem(
                        180)
                    .AlignRight()
                    .AlignMiddle()
                    .Column(column =>
                    {
                        column.Item()
                            .AlignRight()
                            .Text(
                                $"Emitido em: " +
                                $"{DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(
                                7)
                            .FontColor(
                                RelatorioLeaziEstilo
                                    .CinzaTexto);

                        column.Item()
                            .PaddingTop(
                                2)
                            .AlignRight()
                            .Text(
                                $"Emitido por: " +
                                $"{ObterUsuarioEmissor()}")
                            .FontSize(
                                7)
                            .FontColor(
                                RelatorioLeaziEstilo
                                    .CinzaTexto);
                    });
            });
    }

    private void ComporLogo(
        PdfContainer container)
    {
        if (_logo is not null &&
            _logo.Length > 0)
        {
            container
                .Height(
                    48)
                .AlignLeft()
                .Image(
                    _logo)
                .FitArea();

            return;
        }

        container
            .AlignLeft()
            .Column(column =>
            {
                column.Item()
                    .Text(
                        "Leazi")
                    .FontSize(
                        22)
                    .Bold()
                    .FontColor(
                        RelatorioLeaziEstilo
                            .VerdeEscuro);

                column.Item()
                    .Text(
                        "ENERGIA SOLAR")
                    .FontSize(
                        7)
                    .SemiBold()
                    .FontColor(
                        RelatorioLeaziEstilo
                            .VerdePrincipal);
            });
    }

    private void ComporConteudo(
        PdfContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(
                10);

            column.Item()
                .Element(
                    containerFiltros =>
                        ComporFiltros(
                            containerFiltros));

            column.Item()
                .Element(
                    containerIndicadores =>
                        ComporIndicadores(
                            containerIndicadores));

            column.Item()
                .Element(
                    containerTitulo =>
                        ComporTituloDetalhamento(
                            containerTitulo));

            column.Item()
                .Element(
                    containerTabela =>
                        ComporTabela(
                            containerTabela));

            column.Item()
                .Element(
                    containerTotais =>
                        ComporTotaisFinais(
                            containerTotais));
        });
    }

    private void ComporFiltros(
        PdfContainer container)
    {
        container
            .Background(
                RelatorioLeaziEstilo.CinzaFundo)
            .Border(
                1)
            .BorderColor(
                RelatorioLeaziEstilo.CinzaBorda)
            .Padding(
                8)
            .Column(column =>
            {
                column.Spacing(
                    3);

                column.Item()
                    .Text(
                        $"Período da venda: " +
                        $"{ObterPeriodoVenda()}")
                    .FontSize(
                        7);

                column.Item()
                    .Text(
                        $"Período do pagamento: " +
                        $"{ObterPeriodoPagamento()}")
                    .FontSize(
                        7);

                column.Item()
                    .Text(
                        $"Status: {ObterStatus()} | " +
                        $"Pesquisa: {ObterPesquisa()}")
                    .FontSize(
                        7);
            });
    }

    private void ComporIndicadores(
        PdfContainer container)
    {
        var totalVendido =
            _lancamentos.Sum(
                x => x.ValorVenda);

        var totalComissoes =
            _lancamentos.Sum(
                x => x.ValorComissao);

        var totalPago =
            _lancamentos
                .Where(
                    x =>
                        x.Status ==
                        StatusLancamento.Pago)
                .Sum(
                    x => x.ValorComissao);

        var totalPendente =
            _lancamentos
                .Where(
                    x =>
                        x.Status ==
                        StatusLancamento.Pendente)
                .Sum(
                    x => x.ValorComissao);

        var quantidadePaga =
            _lancamentos.Count(
                x =>
                    x.Status ==
                    StatusLancamento.Pago);

        var quantidadePendente =
            _lancamentos.Count(
                x =>
                    x.Status ==
                    StatusLancamento.Pendente);

        var percentualPago =
            totalComissoes > 0
                ? totalPago /
                  totalComissoes *
                  100
                : 0;

        var percentualPendente =
            totalComissoes > 0
                ? totalPendente /
                  totalComissoes *
                  100
                : 0;

        container.Column(column =>
        {
            column.Item()
                .Text(
                    "RESUMO GERAL")
                .FontSize(
                    9)
                .SemiBold()
                .FontColor(
                    RelatorioLeaziEstilo
                        .VerdeEscuro);

            column.Item()
                .PaddingTop(
                    5)
                .Row(row =>
                {
                    AdicionarIndicador(
                        row,
                        "TOTAL VENDIDO",
                        RelatorioLeaziEstilo
                            .FormatarMoeda(
                                totalVendido),
                        false);

                    AdicionarIndicador(
                        row,
                        "COMISSÃO TOTAL",
                        RelatorioLeaziEstilo
                            .FormatarMoeda(
                                totalComissoes),
                        false);

                    AdicionarIndicador(
                        row,
                        "COMISSÃO PAGA",
                        RelatorioLeaziEstilo
                            .FormatarMoeda(
                                totalPago),
                        false);

                    AdicionarIndicador(
                        row,
                        "COMISSÃO PENDENTE",
                        RelatorioLeaziEstilo
                            .FormatarMoeda(
                                totalPendente),
                        true);
                });

            column.Item()
                .PaddingTop(
                    6)
                .Row(row =>
                {
                    AdicionarIndicador(
                        row,
                        "QTD. TOTAL",
                        _lancamentos
                            .Count
                            .ToString(),
                        false);

                    AdicionarIndicador(
                        row,
                        "QTD. PAGAS",
                        quantidadePaga
                            .ToString(),
                        false);

                    AdicionarIndicador(
                        row,
                        "QTD. PENDENTES",
                        quantidadePendente
                            .ToString(),
                        true);

                    AdicionarIndicador(
                        row,
                        "% PAGO",
                        RelatorioLeaziEstilo
                            .FormatarPercentual(
                                percentualPago),
                        false);

                    AdicionarIndicador(
                        row,
                        "% PENDENTE",
                        RelatorioLeaziEstilo
                            .FormatarPercentual(
                                percentualPendente),
                        true);
                });
        });
    }

    private static void AdicionarIndicador(
        RowDescriptor row,
        string titulo,
        string valor,
        bool alerta)
    {
        row.RelativeItem()
            .PaddingRight(
                6)
            .Border(
                1)
            .BorderColor(
                RelatorioLeaziEstilo.CinzaBorda)
            .Background(
                alerta
                    ? RelatorioLeaziEstilo
                        .VermelhoClaro
                    : RelatorioLeaziEstilo
                        .Branco)
            .Padding(
                7)
            .Column(column =>
            {
                column.Item()
                    .AlignCenter()
                    .Text(
                        titulo)
                    .FontSize(
                        6)
                    .SemiBold()
                    .FontColor(
                        RelatorioLeaziEstilo
                            .CinzaTexto);

                column.Item()
                    .PaddingTop(
                        4)
                    .AlignCenter()
                    .Text(
                        valor)
                    .FontSize(
                        10)
                    .Bold()
                    .FontColor(
                        alerta
                            ? RelatorioLeaziEstilo
                                .Vermelho
                            : RelatorioLeaziEstilo
                                .VerdeEscuro);
            });
    }

    private void ComporTituloDetalhamento(
        PdfContainer container)
    {
        container
            .Background(
                RelatorioLeaziEstilo.VerdeEscuro)
            .PaddingVertical(
                5)
            .PaddingHorizontal(
                8)
            .Text(
                "DETALHAMENTO DAS COMISSÕES")
            .FontSize(
                8)
            .SemiBold()
            .FontColor(
                RelatorioLeaziEstilo.Branco);
    }

    private void ComporTabela(
        PdfContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(
                    55);

                columns.RelativeColumn(
                    1.7f);

                columns.RelativeColumn(
                    1.3f);

                columns.ConstantColumn(
                    78);

                columns.ConstantColumn(
                    48);

                columns.ConstantColumn(
                    78);

                columns.ConstantColumn(
                    58);

                columns.ConstantColumn(
                    68);
            });

            table.Header(header =>
            {
                AdicionarCabecalhoTabela(
                    header,
                    "DATA VENDA");

                AdicionarCabecalhoTabela(
                    header,
                    "CLIENTE");

                AdicionarCabecalhoTabela(
                    header,
                    "VENDEDOR");

                AdicionarCabecalhoTabela(
                    header,
                    "VALOR VENDA");

                AdicionarCabecalhoTabela(
                    header,
                    "%");

                AdicionarCabecalhoTabela(
                    header,
                    "COMISSÃO");

                AdicionarCabecalhoTabela(
                    header,
                    "STATUS");

                AdicionarCabecalhoTabela(
                    header,
                    "PAGO EM");
            });

            foreach (var item in _lancamentos)
            {
                AdicionarCelula(
                    table,
                    item.DataVenda.ToString(
                        "dd/MM/yyyy"),
                    true);

                AdicionarCelula(
                    table,
                    item.Cliente,
                    false);

                AdicionarCelula(
                    table,
                    item.VendedorNome,
                    false);

                AdicionarCelula(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarMoeda(
                            item.ValorVenda),
                    true);

                AdicionarCelula(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarPercentual(
                            item.PercentualComissao),
                    true);

                AdicionarCelula(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarMoeda(
                            item.ValorComissao),
                    true);

                AdicionarStatus(
                    table,
                    item);

                AdicionarCelula(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarData(
                            item.DataPagamento),
                    true);
            }
        });
    }

    private static void AdicionarCabecalhoTabela(
    TableCellDescriptor header,
    string texto)
    {
        header.Cell()
            .Background(
                RelatorioLeaziEstilo.VerdeClaro)
            .BorderBottom(
                1)
            .BorderColor(
                RelatorioLeaziEstilo.CinzaBorda)
            .PaddingVertical(
                5)
            .PaddingHorizontal(
                4)
            .AlignCenter()
            .AlignMiddle()
            .Text(
                texto)
            .FontSize(
                6)
            .SemiBold()
            .FontColor(
                RelatorioLeaziEstilo.VerdeEscuro);
    }

    private static void AdicionarCelula(
        TableDescriptor table,
        string texto,
        bool centralizar)
    {
        if (centralizar)
        {
            table.Cell()
                .BorderBottom(
                    0.5f)
                .BorderColor(
                    RelatorioLeaziEstilo.CinzaBorda)
                .PaddingVertical(
                    4)
                .PaddingHorizontal(
                    4)
                .AlignCenter()
                .AlignMiddle()
                .Text(
                    texto)
                .FontSize(
                    6.5f);

            return;
        }

        table.Cell()
            .BorderBottom(
                0.5f)
            .BorderColor(
                RelatorioLeaziEstilo.CinzaBorda)
            .PaddingVertical(
                4)
            .PaddingHorizontal(
                4)
            .AlignLeft()
            .AlignMiddle()
            .Text(
                texto)
            .FontSize(
                6.5f);
    }

    private static void AdicionarStatus(
        TableDescriptor table,
        LancamentoDto item)
    {
        var pago =
            item.Status ==
            StatusLancamento.Pago;

        table.Cell()
            .BorderBottom(
                0.5f)
            .BorderColor(
                RelatorioLeaziEstilo.CinzaBorda)
            .PaddingVertical(
                4)
            .PaddingHorizontal(
                4)
            .AlignCenter()
            .AlignMiddle()
            .Text(
                pago
                    ? "PAGO"
                    : "PENDENTE")
            .FontSize(
                6.5f)
            .SemiBold()
            .FontColor(
                pago
                    ? RelatorioLeaziEstilo
                        .VerdePrincipal
                    : RelatorioLeaziEstilo
                        .Vermelho);
    }

    private void ComporTotaisFinais(
        PdfContainer container)
    {
        var totalPago =
            _lancamentos
                .Where(
                    x =>
                        x.Status ==
                        StatusLancamento.Pago)
                .Sum(
                    x => x.ValorComissao);

        var totalPendente =
            _lancamentos
                .Where(
                    x =>
                        x.Status ==
                        StatusLancamento.Pendente)
                .Sum(
                    x => x.ValorComissao);

        var totalGeral =
            _lancamentos.Sum(
                x => x.ValorComissao);

        container
            .PaddingTop(
                4)
            .Row(row =>
            {
                AdicionarTotalFinal(
                    row,
                    "TOTAL PAGO",
                    totalPago,
                    false);

                AdicionarTotalFinal(
                    row,
                    "TOTAL PENDENTE",
                    totalPendente,
                    true);

                AdicionarTotalFinal(
                    row,
                    "TOTAL GERAL",
                    totalGeral,
                    false);
            });
    }

    private static void AdicionarTotalFinal(
        RowDescriptor row,
        string titulo,
        decimal valor,
        bool alerta)
    {
        row.RelativeItem()
            .PaddingRight(
                8)
            .Border(
                1)
            .BorderColor(
                RelatorioLeaziEstilo.CinzaBorda)
            .Padding(
                6)
            .Column(column =>
            {
                column.Item()
                    .AlignCenter()
                    .Text(
                        titulo)
                    .FontSize(
                        6)
                    .SemiBold()
                    .FontColor(
                        RelatorioLeaziEstilo
                            .CinzaTexto);

                column.Item()
                    .PaddingTop(
                        3)
                    .AlignCenter()
                    .Text(
                        RelatorioLeaziEstilo
                            .FormatarMoeda(
                                valor))
                    .FontSize(
                        9)
                    .Bold()
                    .FontColor(
                        alerta
                            ? RelatorioLeaziEstilo
                                .Vermelho
                            : RelatorioLeaziEstilo
                                .VerdeEscuro);
            });
    }

    private void ComporRodape(
        PdfContainer container)
    {
        container
            .BorderTop(
                1)
            .BorderColor(
                RelatorioLeaziEstilo.VerdeEscuro)
            .PaddingTop(
                5)
            .DefaultTextStyle(
                style => style
                    .FontSize(
                        6)
                    .FontColor(
                        RelatorioLeaziEstilo
                            .CinzaTexto))
            .Row(row =>
            {
                row.RelativeItem()
                    .Text(
                        "Leazi Energia Solar");

                row.RelativeItem()
                    .AlignCenter()
                    .Text(
                        "Sistema de Lançamentos e Comissões");

                row.RelativeItem()
                    .AlignRight()
                    .Text(text =>
                    {
                        text.Span(
                            "Página ");

                        text.CurrentPageNumber();

                        text.Span(
                            " de ");

                        text.TotalPages();
                    });
            });
    }

    private string ObterPeriodoVenda()
    {
        return
            $"{RelatorioLeaziEstilo.FormatarData(
                _filtro.DataVendaInicial)} até " +
            $"{RelatorioLeaziEstilo.FormatarData(
                _filtro.DataVendaFinal)}";
    }

    private string ObterPeriodoPagamento()
    {
        if (!_filtro.DataPagamentoInicial.HasValue &&
            !_filtro.DataPagamentoFinal.HasValue)
        {
            return "Todos";
        }

        return
            $"{RelatorioLeaziEstilo.FormatarData(
                _filtro.DataPagamentoInicial)} até " +
            $"{RelatorioLeaziEstilo.FormatarData(
                _filtro.DataPagamentoFinal)}";
    }

    private string ObterStatus()
    {
        return _filtro.Status switch
        {
            StatusLancamento.Pago =>
                "Pago",

            StatusLancamento.Pendente =>
                "Pendente",

            _ =>
                "Todos"
        };
    }

    private string ObterPesquisa()
    {
        return string.IsNullOrWhiteSpace(
                _filtro.Pesquisa)
            ? "Todas"
            : _filtro.Pesquisa.Trim();
    }

    private string ObterUsuarioEmissor()
    {
        return string.IsNullOrWhiteSpace(
                _filtro.UsuarioEmissor)
            ? "Não informado"
            : _filtro.UsuarioEmissor.Trim();
    }
}