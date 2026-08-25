using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class ControleMensalViewModel : ObservableObject
{
    private readonly IControleMensalService _controleMensalService;
    private readonly IVendedorService _vendedorService;

    [ObservableProperty]
    private MesControleDto? mesSelecionado;

    [ObservableProperty]
    private int anoSelecionado =
        DateTime.Today.Year;

    [ObservableProperty]
    private VendedorDto? vendedorSelecionado;

    [ObservableProperty]
    private StatusLancamento? statusSelecionado;

    [ObservableProperty]
    private string pesquisa =
        string.Empty;

    [ObservableProperty]
    private decimal totalVendido;

    [ObservableProperty]
    private decimal totalComissao;

    [ObservableProperty]
    private int quantidadeRegistros;

    [ObservableProperty]
    private int quantidadePagos;

    [ObservableProperty]
    private int quantidadePendentes;

    [ObservableProperty]
    private bool estaCarregando;

    [ObservableProperty]
    private string mensagemErro =
        string.Empty;

    public ObservableCollection<VendedorDto> Vendedores { get; } =
        new();

    public ObservableCollection<LancamentoDto> Lancamentos { get; } =
        new();

    public ObservableCollection<int> AnosDisponiveis { get; } =
        new();

    public IReadOnlyList<MesControleDto> MesesDisponiveis { get; } =
        CriarMesesDisponiveis();

    public IReadOnlyList<StatusLancamento> StatusDisponiveis { get; } =
        Enum.GetValues<StatusLancamento>();

    public string PeriodoDescricao =>
        MesSelecionado is null
            ? string.Empty
            : $"{MesSelecionado.Nome} de {AnoSelecionado}";

    public ControleMensalViewModel(
        IControleMensalService controleMensalService,
        IVendedorService vendedorService)
    {
        _controleMensalService =
            controleMensalService
            ?? throw new ArgumentNullException(
                nameof(controleMensalService));

        _vendedorService =
            vendedorService
            ?? throw new ArgumentNullException(
                nameof(vendedorService));

        MesSelecionado =
            MesesDisponiveis.First(
                mes =>
                    mes.Numero ==
                    DateTime.Today.Month);
    }

    partial void OnMesSelecionadoChanged(
        MesControleDto? value)
    {
        OnPropertyChanged(
            nameof(PeriodoDescricao));
    }

    partial void OnAnoSelecionadoChanged(
        int value)
    {
        OnPropertyChanged(
            nameof(PeriodoDescricao));
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        if (EstaCarregando)
        {
            return;
        }

        try
        {
            EstaCarregando =
                true;

            MensagemErro =
                string.Empty;

            await CarregarAnosDisponiveisAsync();
            await CarregarVendedoresAsync();
            await CarregarControleAsync();
        }
        catch (Exception exception)
        {
            MensagemErro =
                "Não foi possível carregar o controle mensal. " +
                exception
                    .GetBaseException()
                    .Message;
        }
        finally
        {
            EstaCarregando =
                false;
        }
    }

    [RelayCommand]
    private async Task FiltrarAsync()
    {
        if (EstaCarregando)
        {
            return;
        }

        if (MesSelecionado is null)
        {
            MensagemErro =
                "Selecione um mês válido.";

            return;
        }

        if (!AnosDisponiveis.Contains(
                AnoSelecionado))
        {
            MensagemErro =
                "Selecione um ano válido.";

            return;
        }

        try
        {
            EstaCarregando =
                true;

            MensagemErro =
                string.Empty;

            await CarregarControleAsync();
        }
        catch (Exception exception)
        {
            MensagemErro =
                "Não foi possível aplicar os filtros " +
                "do controle mensal. " +
                exception
                    .GetBaseException()
                    .Message;
        }
        finally
        {
            EstaCarregando =
                false;
        }
    }

    [RelayCommand]
    private async Task LimparFiltrosAsync()
    {
        if (EstaCarregando)
        {
            return;
        }

        MesSelecionado =
            MesesDisponiveis.First(
                mes =>
                    mes.Numero ==
                    DateTime.Today.Month);

        AnoSelecionado =
            DateTime.Today.Year;

        VendedorSelecionado =
            null;

        StatusSelecionado =
            null;

        Pesquisa =
            string.Empty;

        await FiltrarAsync();
    }

    private async Task CarregarAnosDisponiveisAsync()
    {
        var anoAtual =
            DateTime.Today.Year;

        var anoSelecionadoAtual =
            AnoSelecionado;

        var anos =
            await _controleMensalService
                .ListarAnosDisponiveisAsync();

        AnosDisponiveis.Clear();

        foreach (var ano in anos
                     .Where(
                         item =>
                             item is >= 2000 and <= 2100)
                     .Distinct()
                     .OrderByDescending(
                         item =>
                             item))
        {
            AnosDisponiveis.Add(
                ano);
        }

        if (!AnosDisponiveis.Contains(
                anoAtual))
        {
            AnosDisponiveis.Insert(
                0,
                anoAtual);
        }

        if (AnosDisponiveis.Contains(
                anoSelecionadoAtual))
        {
            AnoSelecionado =
                anoSelecionadoAtual;

            return;
        }

        AnoSelecionado =
            anoAtual;
    }

    private async Task CarregarVendedoresAsync()
    {
        var vendedorAtualId =
            VendedorSelecionado?.Id;

        var vendedores =
            await _vendedorService.ListarAsync();

        Vendedores.Clear();

        foreach (var vendedor in vendedores)
        {
            Vendedores.Add(
                vendedor);
        }

        if (!vendedorAtualId.HasValue)
        {
            VendedorSelecionado =
                null;

            return;
        }

        VendedorSelecionado =
            Vendedores.FirstOrDefault(
                vendedor =>
                    vendedor.Id ==
                    vendedorAtualId.Value);
    }

    private async Task CarregarControleAsync()
    {
        if (MesSelecionado is null)
        {
            LimparResultados();

            return;
        }

        var controle =
            await _controleMensalService.ObterAsync(
                new FiltroControleMensalDto
                {
                    Mes =
                        MesSelecionado.Numero,

                    Ano =
                        AnoSelecionado,

                    VendedorId =
                        VendedorSelecionado?.Id,

                    Status =
                        StatusSelecionado,

                    Pesquisa =
                        Pesquisa?.Trim()
                });

        TotalVendido =
            controle.TotalVendido;

        TotalComissao =
            controle.TotalComissao;

        QuantidadeRegistros =
            controle.QuantidadeRegistros;

        QuantidadePagos =
            controle.QuantidadePagos;

        QuantidadePendentes =
            controle.QuantidadePendentes;

        Lancamentos.Clear();

        foreach (var lancamento in controle.Lancamentos)
        {
            Lancamentos.Add(
                lancamento);
        }
    }

    private void LimparResultados()
    {
        TotalVendido =
            0;

        TotalComissao =
            0;

        QuantidadeRegistros =
            0;

        QuantidadePagos =
            0;

        QuantidadePendentes =
            0;

        Lancamentos.Clear();
    }

    private static IReadOnlyList<MesControleDto>
        CriarMesesDisponiveis()
    {
        var cultura =
            CultureInfo.GetCultureInfo(
                "pt-BR");

        return Enumerable
            .Range(
                1,
                12)
            .Select(
                mes =>
                    new MesControleDto
                    {
                        Numero =
                            mes,

                        Nome =
                            cultura
                                .DateTimeFormat
                                .GetMonthName(
                                    mes)
                    })
            .ToList();
    }
}

public sealed class MesControleDto
{
    public int Numero
    {
        get;
        init;
    }

    public string Nome
    {
        get;
        init;
    } = string.Empty;
}