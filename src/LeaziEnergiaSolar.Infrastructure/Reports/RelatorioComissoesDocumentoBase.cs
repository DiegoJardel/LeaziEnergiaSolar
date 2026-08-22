using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Infrastructure.Reports.Styles;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using PdfContainer =
    QuestPDF.Infrastructure.IContainer;

namespace LeaziEnergiaSolar.Infrastructure.Reports;

public abstract class RelatorioComissoesDocumentoBase
    : IDocument
{
    protected IReadOnlyList<LancamentoDto>
        Lancamentos
    { get; }

    protected FiltroRelatorioComissaoDto
        Filtro
    { get; }

    private readonly byte[]?
        _logo;

    /*
     * INFORMAÇÕES DEFINIDAS POR CADA RELATÓRIO
     */

    protected abstract string Titulo { get; }

    protected abstract string Subtitulo { get; }

    protected abstract string Assunto { get; }

    /*
     * CONFIGURAÇÕES OPCIONAIS
     */

    protected virtual bool ExibirDataPagamento =>
        true;

    protected virtual bool ExibirStatus =>
        true;

    protected virtual bool ExibirResumoPorVendedor =>
        false;

    protected virtual bool ExibirIndicadoresExecutivos =>
        false;

    /*
     * CONSTRUTOR
     */

    protected RelatorioComissoesDocumentoBase(
        IReadOnlyList<LancamentoDto> lancamentos,
        FiltroRelatorioComissaoDto filtro,
        byte[]? logo = null)
    {
        Lancamentos =
            lancamentos
            ?? throw new ArgumentNullException(
                nameof(lancamentos));

        Filtro =
            filtro
            ?? throw new ArgumentNullException(
                nameof(filtro));

        _logo =
            logo;
    }

    /*
     * METADADOS
     */

    public DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Title =
                Titulo,

            Author =
                "Leazi Energia Solar",

            Subject =
                Assunto,

            Creator =
                "Sistema Leazi Energia Solar"
        };
    }

    /*
     * DOCUMENTO
     */

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

    /*
     * CABEÇALHO
     */

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
                                Titulo.ToUpperInvariant())
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
                                Subtitulo)
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

    /*
     * LOGO
     */

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

    /*
     * CONTEÚDO
     */

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

            if (ExibirResumoPorVendedor)
            {
                column.Item()
                    .Element(
                        containerTituloVendedores =>
                            ComporTituloResumoVendedores(
                                containerTituloVendedores));

                column.Item()
                    .Element(
                        containerResumoVendedores =>
                            ComporResumoPorVendedor(
                                containerResumoVendedores));
            }

            column.Item()
                .Element(
                    containerTotal =>
                        ComporTotalFinal(
                            containerTotal));
        });
    }

    /*
     * FILTROS
     */

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

    /*
     * INDICADORES
     */

    private void ComporIndicadores(
        PdfContainer container)
    {
        var totalVendido =
            Lancamentos.Sum(
                item =>
                    item.ValorVenda);

        var totalComissoes =
            Lancamentos.Sum(
                item =>
                    item.ValorComissao);

        var totalPago =
            Lancamentos
                .Where(
                    item =>
                        item.Status ==
                        StatusLancamento.Pago)
                .Sum(
                    item =>
                        item.ValorComissao);

        var totalPendente =
            Lancamentos
                .Where(
                    item =>
                        item.Status ==
                        StatusLancamento.Pendente)
                .Sum(
                    item =>
                        item.ValorComissao);

        var quantidadePaga =
            Lancamentos.Count(
                item =>
                    item.Status ==
                    StatusLancamento.Pago);

        var quantidadePendente =
            Lancamentos.Count(
                item =>
                    item.Status ==
                    StatusLancamento.Pendente);

        var quantidadeVendedores =
            Lancamentos
                .Select(
                    item =>
                        item.VendedorId)
                .Distinct()
                .Count();

        var ticketMedio =
            Lancamentos.Count > 0
                ? totalVendido /
                  Lancamentos.Count
                : 0;

        container.Column(column =>
        {
            column.Item()
                .Text(
                    ExibirIndicadoresExecutivos
                        ? "INDICADORES EXECUTIVOS"
                        : "RESUMO DO RELATÓRIO")
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
                        true,
                        removerMargemDireita: true);
                });

            column.Item()
                .PaddingTop(
                    6)
                .Row(row =>
                {
                    AdicionarIndicador(
                        row,
                        "LANÇAMENTOS",
                        Lancamentos.Count.ToString(),
                        false);

                    AdicionarIndicador(
                        row,
                        "PAGOS",
                        quantidadePaga.ToString(),
                        false);

                    AdicionarIndicador(
                        row,
                        "PENDENTES",
                        quantidadePendente.ToString(),
                        true);

                    if (ExibirIndicadoresExecutivos)
                    {
                        AdicionarIndicador(
                            row,
                            "VENDEDORES",
                            quantidadeVendedores.ToString(),
                            false);

                        AdicionarIndicador(
                            row,
                            "TICKET MÉDIO",
                            RelatorioLeaziEstilo
                                .FormatarMoeda(
                                    ticketMedio),
                            false,
                            removerMargemDireita: true);
                    }
                    else
                    {
                        AdicionarIndicador(
                            row,
                            "VENDEDORES",
                            quantidadeVendedores.ToString(),
                            false,
                            removerMargemDireita: true);
                    }
                });
        });
    }

    private static void AdicionarIndicador(
        RowDescriptor row,
        string titulo,
        string valor,
        bool alerta,
        bool removerMargemDireita = false)
    {
        var item =
            row.RelativeItem();

        var container =
            removerMargemDireita
                ? item
                : item.PaddingRight(
                    6);

        container
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

    /*
     * TÍTULO DO DETALHAMENTO
     */

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

    /*
     * TABELA PRINCIPAL
     */

    private void ComporTabela(
        PdfContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(
                    58);

                columns.RelativeColumn(
                    1.7f);

                columns.RelativeColumn(
                    1.3f);

                columns.ConstantColumn(
                    80);

                columns.ConstantColumn(
                    48);

                columns.ConstantColumn(
                    80);

                if (ExibirStatus)
                {
                    columns.ConstantColumn(
                        62);
                }

                if (ExibirDataPagamento)
                {
                    columns.ConstantColumn(
                        68);
                }
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

                if (ExibirStatus)
                {
                    AdicionarCabecalhoTabela(
                        header,
                        "STATUS");
                }

                if (ExibirDataPagamento)
                {
                    AdicionarCabecalhoTabela(
                        header,
                        "PAGO EM");
                }
            });

            foreach (var item in Lancamentos)
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

                if (ExibirStatus)
                {
                    AdicionarStatus(
                        table,
                        item);
                }

                if (ExibirDataPagamento)
                {
                    AdicionarCelula(
                        table,
                        RelatorioLeaziEstilo
                            .FormatarData(
                                item.DataPagamento),
                        true);
                }
            }
        });
    }

    /*
     * CÉLULAS DA TABELA
     */

    protected static void AdicionarCabecalhoTabela(
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

    protected static void AdicionarCelula(
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

    /*
     * RESUMO POR VENDEDOR
     */

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
                "RESUMO POR VENDEDOR")
            .FontSize(
                8)
            .SemiBold()
            .FontColor(
                RelatorioLeaziEstilo
                    .VerdeEscuro);
    }

    private void ComporResumoPorVendedor(
        PdfContainer container)
    {
        var resumo =
            Lancamentos
                .GroupBy(
                    item => new
                    {
                        item.VendedorId,
                        item.VendedorNome
                    })
                .Select(
                    grupo => new
                    {
                        grupo.Key.VendedorNome,

                        Quantidade =
                            grupo.Count(),

                        TotalVendido =
                            grupo.Sum(
                                item =>
                                    item.ValorVenda),

                        TotalComissao =
                            grupo.Sum(
                                item =>
                                    item.ValorComissao),

                        TotalPago =
                            grupo
                                .Where(
                                    item =>
                                        item.Status ==
                                        StatusLancamento.Pago)
                                .Sum(
                                    item =>
                                        item.ValorComissao),

                        TotalPendente =
                            grupo
                                .Where(
                                    item =>
                                        item.Status ==
                                        StatusLancamento.Pendente)
                                .Sum(
                                    item =>
                                        item.ValorComissao)
                    })
                .OrderBy(
                    item =>
                        item.VendedorNome)
                .ToList();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(
                    1.6f);

                columns.ConstantColumn(
                    65);

                columns.ConstantColumn(
                    100);

                columns.ConstantColumn(
                    100);

                columns.ConstantColumn(
                    100);

                columns.ConstantColumn(
                    100);
            });

            table.Header(header =>
            {
                AdicionarCabecalhoTabela(
                    header,
                    "VENDEDOR");

                AdicionarCabecalhoTabela(
                    header,
                    "QTD.");

                AdicionarCabecalhoTabela(
                    header,
                    "TOTAL VENDIDO");

                AdicionarCabecalhoTabela(
                    header,
                    "COMISSÃO");

                AdicionarCabecalhoTabela(
                    header,
                    "PAGO");

                AdicionarCabecalhoTabela(
                    header,
                    "PENDENTE");
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

                AdicionarCelula(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarMoeda(
                            item.TotalComissao),
                    true);

                AdicionarCelula(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarMoeda(
                            item.TotalPago),
                    true);

                AdicionarCelulaPendente(
                    table,
                    RelatorioLeaziEstilo
                        .FormatarMoeda(
                            item.TotalPendente));
            }
        });
    }

    private static void AdicionarCelulaPendente(
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

    /*
     * TOTAL FINAL
     */

    private void ComporTotalFinal(
        PdfContainer container)
    {
        var totalComissoes =
            Lancamentos.Sum(
                item =>
                    item.ValorComissao);

        var totalPago =
            Lancamentos
                .Where(
                    item =>
                        item.Status ==
                        StatusLancamento.Pago)
                .Sum(
                    item =>
                        item.ValorComissao);

        var totalPendente =
            Lancamentos
                .Where(
                    item =>
                        item.Status ==
                        StatusLancamento.Pendente)
                .Sum(
                    item =>
                        item.ValorComissao);

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
                    "TOTAL DE COMISSÕES",
                    totalComissoes,
                    false,
                    removerMargemDireita: true);
            });
    }

    private static void AdicionarTotalFinal(
        RowDescriptor row,
        string titulo,
        decimal valor,
        bool alerta,
        bool removerMargemDireita = false)
    {
        var item =
            row.RelativeItem();

        var container =
            removerMargemDireita
                ? item
                : item.PaddingRight(
                    8);

        container
            .Border(
                1)
            .BorderColor(
                alerta
                    ? RelatorioLeaziEstilo.Vermelho
                    : RelatorioLeaziEstilo.VerdeEscuro)
            .Background(
                alerta
                    ? RelatorioLeaziEstilo.VermelhoClaro
                    : RelatorioLeaziEstilo.VerdeClaro)
            .PaddingVertical(
                8)
            .PaddingHorizontal(
                10)
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
                        RelatorioLeaziEstilo.CinzaTexto);

                column.Item()
                    .PaddingTop(
                        3)
                    .AlignCenter()
                    .Text(
                        RelatorioLeaziEstilo
                            .FormatarMoeda(
                                valor))
                    .FontSize(
                        10)
                    .Bold()
                    .FontColor(
                        alerta
                            ? RelatorioLeaziEstilo.Vermelho
                            : RelatorioLeaziEstilo.VerdeEscuro);
            });
    }

    /*
     * RODAPÉ
     */

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

    /*
     * TEXTO DOS FILTROS
     */

    private string ObterPeriodoVenda()
    {
        if (!Filtro.DataVendaInicial.HasValue &&
            !Filtro.DataVendaFinal.HasValue)
        {
            return "Todos";
        }

        return
            $"{RelatorioLeaziEstilo.FormatarData(
                Filtro.DataVendaInicial)} até " +
            $"{RelatorioLeaziEstilo.FormatarData(
                Filtro.DataVendaFinal)}";
    }

    private string ObterPeriodoPagamento()
    {
        if (!Filtro.DataPagamentoInicial.HasValue &&
            !Filtro.DataPagamentoFinal.HasValue)
        {
            return "Todos";
        }

        return
            $"{RelatorioLeaziEstilo.FormatarData(
                Filtro.DataPagamentoInicial)} até " +
            $"{RelatorioLeaziEstilo.FormatarData(
                Filtro.DataPagamentoFinal)}";
    }

    private string ObterStatus()
    {
        return Filtro.Status switch
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
                Filtro.Pesquisa)
            ? "Todas"
            : Filtro.Pesquisa.Trim();
    }

    private string ObterUsuarioEmissor()
    {
        return string.IsNullOrWhiteSpace(
                Filtro.UsuarioEmissor)
            ? "Não informado"
            : Filtro.UsuarioEmissor.Trim();
    }
}