using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Wpf.Services;
using Microsoft.Win32;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class RelatoriosViewModel : ObservableObject
{
    private readonly IRelatorioComissaoService
        _relatorioComissaoService;

    private readonly IVendedorService
        _vendedorService;

    private readonly IClienteService
        _clienteService;

    private readonly IUsuarioSessaoService
        _sessaoService;

    /*
     * FILTROS
     */

    [ObservableProperty]
    private OpcaoRelatorioComissaoDto?
        tipoRelatorioSelecionado;

    [ObservableProperty]
    private DateTime?
        dataVendaInicial;

    [ObservableProperty]
    private DateTime?
        dataVendaFinal;

    [ObservableProperty]
    private DateTime?
        dataPagamentoInicial;

    [ObservableProperty]
    private DateTime?
        dataPagamentoFinal;

    [ObservableProperty]
    private VendedorDto?
        vendedorSelecionado;

    [ObservableProperty]
    private ClienteDto?
        clienteSelecionado;

    [ObservableProperty]
    private StatusLancamento?
        statusSelecionado;

    [ObservableProperty]
    private string pesquisa =
        string.Empty;

    /*
     * MENSAGEM E CARREGAMENTO
     */

    [ObservableProperty]
    private string mensagem =
        string.Empty;

    [ObservableProperty]
    private bool mensagemEhErro;

    [ObservableProperty]
    private bool estaCarregando;

    /*
     * COLEÇÕES
     */

    public ObservableCollection<VendedorDto>
        Vendedores
    { get; } =
        new();

    public ObservableCollection<ClienteDto>
        Clientes
    { get; } =
        new();

    public IReadOnlyList<StatusLancamento>
        StatusDisponiveis
    { get; } =
        Enum.GetValues<StatusLancamento>();

    public IReadOnlyList<OpcaoRelatorioComissaoDto>
        TiposRelatorio
    { get; } =
        new[]
        {
            new OpcaoRelatorioComissaoDto
            {
                Tipo =
                    TipoRelatorioComissao
                        .ExecutivoVendasComissoes,

                Descricao =
                    "Executivo de vendas e comissões"
            },

            new OpcaoRelatorioComissaoDto
            {
                Tipo =
                    TipoRelatorioComissao
                        .ComissoesAPagar,

                Descricao =
                    "Comissões a pagar"
            },

            new OpcaoRelatorioComissaoDto
            {
                Tipo =
                    TipoRelatorioComissao
                        .ComissoesPagas,

                Descricao =
                    "Comissões pagas"
            },

            new OpcaoRelatorioComissaoDto
            {
                Tipo =
                    TipoRelatorioComissao
                        .ComissoesPorVendedor,

                Descricao =
                    "Comissões por vendedor"
            },

            new OpcaoRelatorioComissaoDto
            {
                Tipo =
                    TipoRelatorioComissao
                        .ExtratoIndividualVendedor,

                Descricao =
                    "Extrato individual do vendedor"
            },

            new OpcaoRelatorioComissaoDto
            {
                Tipo =
                    TipoRelatorioComissao
                        .GeralComissoes,

                Descricao =
                    "Relatório geral de comissões"
            }
        };

    /*
     * PROPRIEDADES CALCULADAS
     */

    public TipoRelatorioComissao?
        TipoRelatorio =>
        TipoRelatorioSelecionado?.Tipo;

    public bool PodeFiltrarDataPagamento =>
        TipoRelatorio is
            TipoRelatorioComissao
                .ExecutivoVendasComissoes or
            TipoRelatorioComissao
                .ComissoesPagas or
            TipoRelatorioComissao
                .ComissoesPorVendedor or
            TipoRelatorioComissao
                .ExtratoIndividualVendedor or
            TipoRelatorioComissao
                .GeralComissoes;

    public bool PodeSelecionarStatus =>
        TipoRelatorio is not
            TipoRelatorioComissao
                .ComissoesAPagar &&
        TipoRelatorio is not
            TipoRelatorioComissao
                .ComissoesPagas;

    public bool VendedorObrigatorio =>
        TipoRelatorio ==
        TipoRelatorioComissao
            .ExtratoIndividualVendedor;

    public string NomeTipoRelatorio =>
        TipoRelatorioSelecionado?.Descricao
        ?? "Nenhum relatório selecionado";

    public string DescricaoTipoRelatorio =>
        TipoRelatorio switch
        {
            TipoRelatorioComissao
                .ExecutivoVendasComissoes =>
                "Apresenta uma visão resumida para a diretoria, " +
                "com total vendido, comissão total, valores pagos, " +
                "valores pendentes, ticket médio e resumo por vendedor.",

            TipoRelatorioComissao
                .ComissoesAPagar =>
                "Apresenta somente as comissões pendentes, o total " +
                "que a empresa precisa pagar, os dias em aberto e " +
                "o resumo dos valores por vendedor.",

            TipoRelatorioComissao
                .ComissoesPagas =>
                "Apresenta somente as comissões pagas, incluindo " +
                "a data em que cada pagamento foi realizado e " +
                "os totais agrupados por vendedor.",

            TipoRelatorioComissao
                .ComissoesPorVendedor =>
                "Agrupa os lançamentos e os valores de comissão " +
                "por vendedor, com totais pagos e pendentes.",

            TipoRelatorioComissao
                .ExtratoIndividualVendedor =>
                "Gera um extrato detalhado para um único vendedor. " +
                "A seleção do vendedor é obrigatória.",

            TipoRelatorioComissao
                .GeralComissoes =>
                "Reúne todas as comissões pagas e pendentes em um " +
                "único PDF, com indicadores, totais e detalhamento.",

            _ =>
                "Selecione um tipo de relatório para visualizar " +
                "as opções disponíveis."
        };

    /*
     * CONSTRUTOR
     */

    public RelatoriosViewModel(
        IRelatorioComissaoService relatorioComissaoService,
        IVendedorService vendedorService,
        IClienteService clienteService,
        IUsuarioSessaoService sessaoService)
    {
        _relatorioComissaoService =
            relatorioComissaoService
            ?? throw new ArgumentNullException(
                nameof(relatorioComissaoService));

        _vendedorService =
            vendedorService
            ?? throw new ArgumentNullException(
                nameof(vendedorService));

        _clienteService =
            clienteService
            ?? throw new ArgumentNullException(
                nameof(clienteService));

        _sessaoService =
            sessaoService
            ?? throw new ArgumentNullException(
                nameof(sessaoService));

        TipoRelatorioSelecionado =
            TiposRelatorio.FirstOrDefault(
                item =>
                    item.Tipo ==
                    TipoRelatorioComissao
                        .GeralComissoes);

        DefinirPeriodoPadrao();
    }

    /*
     * NOTIFICAÇÕES
     */

    partial void OnTipoRelatorioSelecionadoChanged(
        OpcaoRelatorioComissaoDto? value)
    {
        AplicarRegrasTipoRelatorio();

        OnPropertyChanged(
            nameof(TipoRelatorio));

        OnPropertyChanged(
            nameof(PodeFiltrarDataPagamento));

        OnPropertyChanged(
            nameof(PodeSelecionarStatus));

        OnPropertyChanged(
            nameof(VendedorObrigatorio));

        OnPropertyChanged(
            nameof(NomeTipoRelatorio));

        OnPropertyChanged(
            nameof(DescricaoTipoRelatorio));
    }

    /*
     * COMANDOS
     */

    [RelayCommand]
    private async Task CarregarAsync()
    {
        await ExecutarAsync(
            async () =>
            {
                await CarregarVendedoresAsync();

                await CarregarClientesAsync();
            },
            "carregar os filtros dos relatórios");
    }

    [RelayCommand]
    private void Limpar()
    {
        LimparFiltros();
    }

    [RelayCommand]
    private async Task GerarPdfAsync()
    {
        if (!ValidarFiltros())
        {
            return;
        }

        var filtro =
            CriarFiltro();

        var nomeArquivo =
            CriarNomeArquivo(
                filtro.TipoRelatorio);

        var janelaSalvar =
            new SaveFileDialog
            {
                Title =
                    "Salvar relatório em PDF",

                Filter =
                    "Arquivo PDF (*.pdf)|*.pdf",

                DefaultExt =
                    ".pdf",

                AddExtension =
                    true,

                OverwritePrompt =
                    true,

                FileName =
                    nomeArquivo
            };

        var resultadoJanela =
            janelaSalvar.ShowDialog();

        if (resultadoJanela != true ||
            string.IsNullOrWhiteSpace(
                janelaSalvar.FileName))
        {
            return;
        }

        await ExecutarAsync(
            async () =>
            {
                var resultado =
                    await _relatorioComissaoService
                        .GerarPdfAsync(
                            filtro,
                            janelaSalvar.FileName);

                ExibirMensagem(
                    resultado.Mensagem,
                    !resultado.Sucesso);

                if (!resultado.Sucesso)
                {
                    return;
                }

                if (!File.Exists(
                        resultado.CaminhoArquivo))
                {
                    ExibirMensagem(
                        "O serviço informou que o PDF foi gerado, " +
                        "mas o arquivo não foi encontrado.",
                        true);

                    return;
                }

                AbrirPdf(
                    resultado.CaminhoArquivo);
            },
            "gerar o relatório em PDF");
    }

    /*
     * CARREGAMENTO
     */

    private async Task CarregarVendedoresAsync()
    {
        var vendedorAtualId =
            VendedorSelecionado?.Id;

        var vendedores =
            await _vendedorService.ListarAsync();

        Vendedores.Clear();

        foreach (var vendedor in vendedores
                     .Where(
                         item =>
                             item.Ativo)
                     .OrderBy(
                         item =>
                             item.Nome))
        {
            Vendedores.Add(
                vendedor);
        }

        if (vendedorAtualId.HasValue)
        {
            VendedorSelecionado =
                Vendedores.FirstOrDefault(
                    item =>
                        item.Id ==
                        vendedorAtualId.Value);
        }
    }

    private async Task CarregarClientesAsync()
    {
        var clienteAtualId =
            ClienteSelecionado?.Id;

        var clientes =
            await _clienteService.ListarAsync(
                ativo: true);

        Clientes.Clear();

        foreach (var cliente in clientes
                     .OrderBy(
                         item =>
                             item.NomeRazaoSocial))
        {
            Clientes.Add(
                cliente);
        }

        if (clienteAtualId.HasValue)
        {
            ClienteSelecionado =
                Clientes.FirstOrDefault(
                    item =>
                        item.Id ==
                        clienteAtualId.Value);
        }
    }

    /*
     * REGRAS DOS TIPOS DE RELATÓRIO
     */

    private void AplicarRegrasTipoRelatorio()
    {
        Mensagem =
            string.Empty;

        MensagemEhErro =
            false;

        switch (TipoRelatorio)
        {
            case TipoRelatorioComissao
                .ComissoesAPagar:

                StatusSelecionado =
                    StatusLancamento.Pendente;

                DataPagamentoInicial =
                    null;

                DataPagamentoFinal =
                    null;

                break;

            case TipoRelatorioComissao
                .ComissoesPagas:

                StatusSelecionado =
                    StatusLancamento.Pago;

                break;

            case TipoRelatorioComissao
                .ComissoesPorVendedor:

                StatusSelecionado =
                    null;

                break;

            case TipoRelatorioComissao
                .ExtratoIndividualVendedor:

                StatusSelecionado =
                    null;

                break;

            case TipoRelatorioComissao
                .ExecutivoVendasComissoes:

                StatusSelecionado =
                    null;

                break;

            case TipoRelatorioComissao
                .GeralComissoes:

                StatusSelecionado =
                    null;

                break;

            default:

                StatusSelecionado =
                    null;

                break;
        }
    }

    private void LimparFiltros()
    {
        Pesquisa =
            string.Empty;

        VendedorSelecionado =
            null;

        ClienteSelecionado =
            null;

        StatusSelecionado =
            null;

        DataPagamentoInicial =
            null;

        DataPagamentoFinal =
            null;

        DefinirPeriodoPadrao();

        TipoRelatorioSelecionado =
            TiposRelatorio.FirstOrDefault(
                item =>
                    item.Tipo ==
                    TipoRelatorioComissao
                        .GeralComissoes);

        AplicarRegrasTipoRelatorio();

        Mensagem =
            string.Empty;

        MensagemEhErro =
            false;
    }

    private void DefinirPeriodoPadrao()
    {
        var hoje =
            DateTime.Today;

        DataVendaInicial =
            new DateTime(
                hoje.Year,
                hoje.Month,
                1);

        DataVendaFinal =
            hoje;
    }

    /*
     * VALIDAÇÕES
     */

    private bool ValidarFiltros()
    {
        if (TipoRelatorioSelecionado is null)
        {
            ExibirMensagem(
                "Selecione o tipo de relatório.",
                true);

            return false;
        }

        if (DataVendaInicial.HasValue &&
            DataVendaFinal.HasValue &&
            DataVendaInicial.Value.Date >
            DataVendaFinal.Value.Date)
        {
            ExibirMensagem(
                "A data inicial da venda não pode ser " +
                "maior que a data final.",
                true);

            return false;
        }

        if (DataVendaFinal.HasValue &&
            DataVendaFinal.Value.Date >
            DateTime.Today)
        {
            ExibirMensagem(
                "A data final da venda não pode ser futura.",
                true);

            return false;
        }

        if (PodeFiltrarDataPagamento &&
            DataPagamentoInicial.HasValue &&
            DataPagamentoFinal.HasValue &&
            DataPagamentoInicial.Value.Date >
            DataPagamentoFinal.Value.Date)
        {
            ExibirMensagem(
                "A data inicial do pagamento não pode ser " +
                "maior que a data final.",
                true);

            return false;
        }

        if (PodeFiltrarDataPagamento &&
            DataPagamentoFinal.HasValue &&
            DataPagamentoFinal.Value.Date >
            DateTime.Today)
        {
            ExibirMensagem(
                "A data final do pagamento não pode ser futura.",
                true);

            return false;
        }

        if (TipoRelatorio ==
                TipoRelatorioComissao
                    .ExtratoIndividualVendedor &&
            VendedorSelecionado is null)
        {
            ExibirMensagem(
                "Selecione o vendedor para gerar " +
                "o extrato individual.",
                true);

            return false;
        }

        return true;
    }

    /*
     * MONTAGEM DO FILTRO
     */

    private FiltroRelatorioComissaoDto CriarFiltro()
    {
        var usuarioAtual =
            _sessaoService.UsuarioAtual;

        var status =
            TipoRelatorio switch
            {
                TipoRelatorioComissao
                    .ComissoesAPagar =>
                    StatusLancamento.Pendente,

                TipoRelatorioComissao
                    .ComissoesPagas =>
                    StatusLancamento.Pago,

                _ =>
                    StatusSelecionado
            };

        return new FiltroRelatorioComissaoDto
        {
            TipoRelatorio =
                TipoRelatorioSelecionado!.Tipo,

            DataVendaInicial =
                DataVendaInicial?.Date,

            DataVendaFinal =
                DataVendaFinal?.Date,

            DataPagamentoInicial =
                PodeFiltrarDataPagamento
                    ? DataPagamentoInicial?.Date
                    : null,

            DataPagamentoFinal =
                PodeFiltrarDataPagamento
                    ? DataPagamentoFinal?.Date
                    : null,

            VendedorId =
                VendedorSelecionado?.Id,

            ClienteId =
                ClienteSelecionado?.Id,

            Status =
                status,

            Pesquisa =
                Pesquisa?.Trim()
                ?? string.Empty,

            UsuarioEmissorId =
                usuarioAtual?.Id,

            UsuarioEmissor =
                usuarioAtual?.Nome
                ?? string.Empty
        };
    }

    /*
     * NOME DO ARQUIVO
     */

    private static string CriarNomeArquivo(
        TipoRelatorioComissao tipoRelatorio)
    {
        var nome =
            tipoRelatorio switch
            {
                TipoRelatorioComissao
                    .ExecutivoVendasComissoes =>
                    "Relatorio_Executivo",

                TipoRelatorioComissao
                    .ComissoesAPagar =>
                    "Comissoes_A_Pagar",

                TipoRelatorioComissao
                    .ComissoesPagas =>
                    "Comissoes_Pagas",

                TipoRelatorioComissao
                    .ComissoesPorVendedor =>
                    "Comissoes_Por_Vendedor",

                TipoRelatorioComissao
                    .ExtratoIndividualVendedor =>
                    "Extrato_Individual_Vendedor",

                TipoRelatorioComissao
                    .GeralComissoes =>
                    "Relatorio_Geral_Comissoes",

                _ =>
                    "Relatorio_Comissoes"
            };

        return $"{nome}_" +
               $"{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
    }

    /*
     * ABERTURA DO PDF
     */

    private static void AbrirPdf(
        string caminhoArquivo)
    {
        try
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName =
                        caminhoArquivo,

                    UseShellExecute =
                        true
                });
        }
        catch
        {
            /*
             * O PDF já foi gerado.
             *
             * Caso não exista um aplicativo padrão
             * configurado para abrir arquivos PDF,
             * o sistema mantém a mensagem de sucesso
             * com o caminho em que o arquivo foi salvo.
             */
        }
    }

    /*
     * EXECUÇÃO SEGURA
     */

    private async Task ExecutarAsync(
        Func<Task> acao,
        string descricaoOperacao)
    {
        if (EstaCarregando)
        {
            return;
        }

        try
        {
            EstaCarregando =
                true;

            Mensagem =
                string.Empty;

            MensagemEhErro =
                false;

            await acao();
        }
        catch (Exception exception)
        {
            var mensagemRaiz =
                exception
                    .GetBaseException()
                    .Message;

            ExibirMensagem(
                $"Não foi possível {descricaoOperacao}. " +
                $"Detalhes: {mensagemRaiz}",
                true);
        }
        finally
        {
            EstaCarregando =
                false;
        }
    }

    /*
     * MENSAGENS
     */

    private void ExibirMensagem(
        string texto,
        bool erro)
    {
        Mensagem =
            texto;

        MensagemEhErro =
            erro;
    }
}