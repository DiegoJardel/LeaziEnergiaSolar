using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Infrastructure.Reports.Styles;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using PdfContainer =
    QuestPDF.Infrastructure.IContainer;

namespace LeaziEnergiaSolar.Infrastructure.Reports;

public sealed class RelatorioComissoesAPagarDocument
    : IDocument
{
    private readonly IReadOnlyList<LancamentoDto>
        _lancamentos;

    private readonly FiltroRelatorioComissaoDto
        _filtro;

    private readonly byte[]?
        _logo;

    public RelatorioComissoesAPagarDocument(
        IReadOnlyList<LancamentoDto> lancamentos,
        FiltroRelatorioComissaoDto filtro,
        byte[]? logo = null)
    {
        _lancamentos =
            lancamentos
                ?.Where(
                    x =>
                        x.Status ==
                        StatusLancamento.Pendente)
                .OrderBy(
                    x => x.DataVenda)
                .ThenBy(
                    x => x.VendedorNome)
                .ThenBy(
                    x => x.Cliente)
                .ToList()
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
                "Relatório de Comissões a Pagar",

            Author =
                "Leazi Energia Solar",

            Subject =
                "Relatório de comissões pendentes",

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
                                "RELATÓRIO DE COMISSÕES A PAGAR")
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
                                "Comissões pendentes de pagamento")
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
                    containerTituloVendedores =>
                        ComporTituloResumoVendedores(
                            containerTituloVendedores));

            column.Item()
                .Element(
                    containerVendedores =>
                        ComporResumoPorVendedor(
                            containerVendedores));

            column.Item()
                .Element(
                    containerTotal =>
                        ComporTotalFinal(
                            containerTotal));
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
                        $"Status: Pendente | " +
                        $"Pesquisa: {ObterPesquisa()}")
                    .FontSize(
                        7);

                column.Item()
                    .Text(
                        "Observação: os dias em aberto são calculados " +
                        "a partir da data da venda.")
                    .FontSize(
                        7)
                    .FontColor(
                        RelatorioLeaziEstilo.CinzaTexto);
            });
    }

    private void ComporIndicadores(
        PdfContainer container)
    {
        var totalVendido =
            _lancamentos.Sum(
                x => x.ValorVenda);

        var totalPendente =
            _lancamentos.Sum(
                x => x.ValorComissao);

        var quantidadePendencias =
            _lancamentos.Count;

        var quantidadeVendedores =
            _lancamentos
                .Select(
                    x => x.VendedorId)
                .Distinct()
                .Count();

        var diasMaiorPendencia =
            _lancamentos.Count > 0
                ? _lancamentos.Max(
                    CalcularDiasEmAberto)
                : 0;

        container.Column(column =>
        {
            column.Item()
                .Text(
                    "RESUMO DAS COMISSÕES PENDENTES")
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
                        "TOTAL A PAGAR",
                        RelatorioLeaziEstilo
                            .FormatarMoeda(
                                totalPendente),
                        true);

                    AdicionarIndicador(
                        row,
                        "PENDÊNCIAS",
                        quantidadePendencias.ToString(),
                        true);

                    AdicionarIndicador(
                        row,
                        "VENDEDORES",
                        quantidadeVendedores.ToString(),
                        false);

                    AdicionarIndicador(
                        row,
                        "MAIOR TEMPO EM ABERTO",
                        $"{diasMaiorPendencia} dia(s)",
                        diasMaiorPendencia > 30);
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
                "DETALHAMENTO DAS COMISSÕES A PAGAR")
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
                    62);

                columns.RelativeColumn(
                    1.8f);

                columns.RelativeColumn(
                    1.4f);

                columns.ConstantColumn(
                    82);

                columns.ConstantColumn(
                    48);

                columns.ConstantColumn(
                    82);

                columns.ConstantColumn(
                    65);
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
                    "A PAGAR");

                AdicionarCabecalhoTabela(
                    header,
                    "DIAS EM ABERTO");
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

                AdicionarCelulaDestaque(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarMoeda(
                            item.ValorComissao));

                AdicionarDiasEmAberto(
                    table,
                    item);
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
                RelatorioLeaziEstilo
                    .VerdeEscuro);
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

    private static void AdicionarCelulaDestaque(
        TableDescriptor table,
        string texto)
    {
        table.Cell()
            .BorderBottom(
                0.5f)
            .BorderColor(
                RelatorioLeaziEstilo.CinzaBorda)
            .Background(
                RelatorioLeaziEstilo.VermelhoClaro)
            .PaddingVertical(
                4)
            .PaddingHorizontal(
                4)
            .AlignCenter()
            .AlignMiddle()
            .Text(
                texto)
            .FontSize(
                6.5f)
            .SemiBold()
            .FontColor(
                RelatorioLeaziEstilo.Vermelho);
    }

    private static void AdicionarDiasEmAberto(
        TableDescriptor table,
        LancamentoDto item)
    {
        var dias =
            CalcularDiasEmAberto(
                item);

        var alerta =
            dias > 30;

        table.Cell()
            .BorderBottom(
                0.5f)
            .BorderColor(
                RelatorioLeaziEstilo.CinzaBorda)
            .Background(
                alerta
                    ? RelatorioLeaziEstilo
                        .VermelhoClaro
                    : RelatorioLeaziEstilo
                        .Branco)
            .PaddingVertical(
                4)
            .PaddingHorizontal(
                4)
            .AlignCenter()
            .AlignMiddle()
            .Text(
                $"{dias} dia(s)")
            .FontSize(
                6.5f)
            .SemiBold()
            .FontColor(
                alerta
                    ? RelatorioLeaziEstilo
                        .Vermelho
                    : RelatorioLeaziEstilo
                        .CinzaTitulo);
    }

    private void ComporTituloResumoVendedores(
        PdfContainer container)
    {
        container
            .Background(
                RelatorioLeaziEstilo.VerdeClaro)
            .Border(
                1)
            .BorderColor(
                RelatorioLeaziEstilo.CinzaBorda)
            .PaddingVertical(
                5)
            .PaddingHorizontal(
                8)
            .Text(
                "TOTAL PENDENTE POR VENDEDOR")
            .FontSize(
                8)
            .SemiBold()
            .FontColor(
                RelatorioLeaziEstilo.VerdeEscuro);
    }

    private void ComporResumoPorVendedor(
        PdfContainer container)
    {
        var resumo =
            _lancamentos
                .GroupBy(
                    x => new
                    {
                        x.VendedorId,
                        x.VendedorNome
                    })
                .Select(
                    grupo => new
                    {
                        grupo.Key.VendedorNome,
                        Quantidade =
                            grupo.Count(),
                        TotalVendido =
                            grupo.Sum(
                                x => x.ValorVenda),
                        TotalPendente =
                            grupo.Sum(
                                x => x.ValorComissao)
                    })
                .OrderBy(
                    x => x.VendedorNome)
                .ToList();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(
                    1.8f);

                columns.ConstantColumn(
                    90);

                columns.ConstantColumn(
                    110);

                columns.ConstantColumn(
                    110);
            });

            table.Header(header =>
            {
                AdicionarCabecalhoTabela(
                    header,
                    "VENDEDOR");

                AdicionarCabecalhoTabela(
                    header,
                    "PENDÊNCIAS");

                AdicionarCabecalhoTabela(
                    header,
                    "TOTAL VENDIDO");

                AdicionarCabecalhoTabela(
                    header,
                    "TOTAL A PAGAR");
            });

            foreach (var item in resumo)
            {
                AdicionarCelula(
                    table,
                    item.VendedorNome,
                    false);

                AdicionarCelula(
                    table,
                    item.Quantidade.ToString(),
                    true);

                AdicionarCelula(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarMoeda(
                            item.TotalVendido),
                    true);

                AdicionarCelulaDestaque(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarMoeda(
                            item.TotalPendente));
            }
        });
    }
    private void ComporTotalFinal(
        PdfContainer container)
    {
        var totalPendente =
            _lancamentos.Sum(
                item =>
                    item.ValorComissao);

        container
            .PaddingTop(
                4)
            .Background(
                RelatorioLeaziEstilo.VermelhoClaro)
            .Border(
                1)
            .BorderColor(
                RelatorioLeaziEstilo.Vermelho)
            .PaddingVertical(
                10)
            .PaddingHorizontal(
                14)
            .Row(row =>
            {
                row.RelativeItem()
                    .AlignLeft()
                    .AlignMiddle()
                    .Text(
                        "TOTAL GERAL A PAGAR")
                    .FontSize(
                        8)
                    .SemiBold()
                    .FontColor(
                        RelatorioLeaziEstilo.CinzaTexto);

                row.RelativeItem()
                    .AlignRight()
                    .AlignMiddle()
                    .Text(
                        RelatorioLeaziEstilo
                            .FormatarMoeda(
                                totalPendente))
                    .FontSize(
                        13)
                    .Bold()
                    .FontColor(
                        RelatorioLeaziEstilo.Vermelho);
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

    private static int CalcularDiasEmAberto(
        LancamentoDto item)
    {
        var dias =
            (DateTime.Today -
             item.DataVenda.Date).Days;

        return dias < 0
            ? 0
            : dias;
    }
}