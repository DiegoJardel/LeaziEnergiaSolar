using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Wpf.Utils;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class ControleMensalViewModel : ObservableObject
{
    private readonly IControleMensalService _controleMensalService;
    private readonly IVendedorService _vendedorService;

    [ObservableProperty]
    private MesControleDto? mesSelecionado;

    [ObservableProperty]
    private int anoSelecionado = DateTime.Today.Year;

    [ObservableProperty]
    private VendedorDto? vendedorSelecionado;

    [ObservableProperty]
    private StatusLancamento? statusSelecionado;

    [ObservableProperty]
    private string pesquisa = string.Empty;

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
    private string mensagemErro = string.Empty;

    public ObservableCollection<VendedorDto> Vendedores { get; } = new();

    public ObservableCollection<LancamentoDto> Lancamentos { get; } = new();

    public IReadOnlyList<MesControleDto> MesesDisponiveis { get; } =
        CriarMesesDisponiveis();

    public IReadOnlyList<int> AnosDisponiveis { get; } =
        YearFilterHelper.CriarAnosDisponiveis();

    public IReadOnlyList<StatusLancamento> StatusDisponiveis { get; } =
        Enum.GetValues<StatusLancamento>();

    public string PeriodoDescricao => MesSelecionado is null
        ? string.Empty
        : $"{MesSelecionado.Nome} de {AnoSelecionado}";

    public ControleMensalViewModel(
        IControleMensalService controleMensalService,
        IVendedorService vendedorService)
    {
        _controleMensalService = controleMensalService;
        _vendedorService = vendedorService;

        MesSelecionado = MesesDisponiveis.First(mes =>
            mes.Numero == DateTime.Today.Month);
    }

    partial void OnMesSelecionadoChanged(MesControleDto? value)
    {
        OnPropertyChanged(nameof(PeriodoDescricao));
    }

    partial void OnAnoSelecionadoChanged(int value)
    {
        OnPropertyChanged(nameof(PeriodoDescricao));
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
            EstaCarregando = true;
            MensagemErro = string.Empty;

            await CarregarVendedoresAsync();
            await CarregarControleAsync();
        }
        catch (Exception)
        {
            MensagemErro =
                "Não foi possível carregar o controle mensal.";
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task FiltrarAsync()
    {
        if (EstaCarregando)
        {
            return;
        }

        try
        {
            EstaCarregando = true;
            MensagemErro = string.Empty;
            await CarregarControleAsync();
        }
        catch (Exception)
        {
            MensagemErro =
                "Não foi possível aplicar os filtros do controle mensal.";
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task LimparFiltrosAsync()
    {
        MesSelecionado = MesesDisponiveis.First(mes =>
            mes.Numero == DateTime.Today.Month);
        AnoSelecionado = DateTime.Today.Year;
        VendedorSelecionado = null;
        StatusSelecionado = null;
        Pesquisa = string.Empty;

        await FiltrarAsync();
    }

    private async Task CarregarVendedoresAsync()
    {
        var vendedorAtualId = VendedorSelecionado?.Id;
        var vendedores = await _vendedorService.ListarAsync();

        Vendedores.Clear();

        foreach (var vendedor in vendedores)
        {
            Vendedores.Add(vendedor);
        }

        if (vendedorAtualId.HasValue)
        {
            VendedorSelecionado = Vendedores.FirstOrDefault(vendedor =>
                vendedor.Id == vendedorAtualId.Value);
        }
    }

    private async Task CarregarControleAsync()
    {
        if (MesSelecionado is null)
        {
            return;
        }

        var controle = await _controleMensalService.ObterAsync(
            new FiltroControleMensalDto
            {
                Mes = MesSelecionado.Numero,
                Ano = AnoSelecionado,
                VendedorId = VendedorSelecionado?.Id,
                Status = StatusSelecionado,
                Pesquisa = Pesquisa
            });

        TotalVendido = controle.TotalVendido;
        TotalComissao = controle.TotalComissao;
        QuantidadeRegistros = controle.QuantidadeRegistros;
        QuantidadePagos = controle.QuantidadePagos;
        QuantidadePendentes = controle.QuantidadePendentes;

        Lancamentos.Clear();

        foreach (var lancamento in controle.Lancamentos)
        {
            Lancamentos.Add(lancamento);
        }
    }

    private static IReadOnlyList<MesControleDto> CriarMesesDisponiveis()
    {
        var cultura = CultureInfo.GetCultureInfo("pt-BR");

        return Enumerable
            .Range(1, 12)
            .Select(mes => new MesControleDto
            {
                Numero = mes,
                Nome = cultura.DateTimeFormat.GetMonthName(mes)
            })
            .ToList();
    }
}

public sealed class MesControleDto
{
    public int Numero { get; init; }

    public string Nome { get; init; } = string.Empty;
}
