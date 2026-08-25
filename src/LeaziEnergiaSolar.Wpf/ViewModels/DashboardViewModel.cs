using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IDashboardService _dashboardService;

    [ObservableProperty]
    private int anoSelecionado =
        DateTime.Today.Year;

    [ObservableProperty]
    private MesFiltroDto? mesSelecionado;

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

    public ObservableCollection<ResumoMensalDto> ResumoMensal
    {
        get;
    } = new();

    public ObservableCollection<LancamentoDto> UltimosLancamentos
    {
        get;
    } = new();

    public ObservableCollection<int> AnosDisponiveis
    {
        get;
    } = new();

    public IReadOnlyList<MesFiltroDto> MesesDisponiveis
    {
        get;
    } = CriarMesesDisponiveis();

    public string PeriodoDescricao =>
        MesSelecionado?.Numero is null
            ? $"Ano de {AnoSelecionado}"
            : $"{MesSelecionado.Nome} de {AnoSelecionado}";

    public DashboardViewModel(
        IDashboardService dashboardService)
    {
        _dashboardService =
            dashboardService;

        MesSelecionado =
            MesesDisponiveis.First();
    }

    partial void OnAnoSelecionadoChanged(
        int value)
    {
        OnPropertyChanged(
            nameof(PeriodoDescricao));
    }

    partial void OnMesSelecionadoChanged(
        MesFiltroDto? value)
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
            await CarregarDashboardAsync();
        }
        catch (Exception exception)
        {
            MensagemErro =
                "Não foi possível carregar os indicadores " +
                "do dashboard. " +
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
    private async Task AtualizarAsync()
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

            await CarregarDashboardAsync();
        }
        catch (Exception exception)
        {
            MensagemErro =
                "Não foi possível atualizar os indicadores " +
                "do dashboard. " +
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

        AnoSelecionado =
            DateTime.Today.Year;

        MesSelecionado =
            MesesDisponiveis.FirstOrDefault();

        await AtualizarAsync();
    }

    private async Task CarregarAnosDisponiveisAsync()
    {
        var anoAtual =
            DateTime.Today.Year;

        var anos =
            await _dashboardService
                .ListarAnosDisponiveisAsync();

        AnosDisponiveis.Clear();

        foreach (var ano in anos
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

        if (!AnosDisponiveis.Contains(
                AnoSelecionado))
        {
            AnoSelecionado =
                anoAtual;
        }
    }

    private async Task CarregarDashboardAsync()
    {
        var dashboard =
            await _dashboardService.ObterAsync(
                AnoSelecionado,
                MesSelecionado?.Numero);

        TotalVendido =
            dashboard.TotalVendido;

        TotalComissao =
            dashboard.TotalComissao;

        QuantidadeRegistros =
            dashboard.QuantidadeRegistros;

        QuantidadePagos =
            dashboard.QuantidadePagos;

        QuantidadePendentes =
            dashboard.QuantidadePendentes;

        ResumoMensal.Clear();

        foreach (var resumo in dashboard.ResumoMensal)
        {
            ResumoMensal.Add(
                resumo);
        }

        UltimosLancamentos.Clear();

        foreach (var lancamento in dashboard.UltimosLancamentos)
        {
            UltimosLancamentos.Add(
                lancamento);
        }
    }

    private static IReadOnlyList<MesFiltroDto>
        CriarMesesDisponiveis()
    {
        var cultura =
            CultureInfo.GetCultureInfo(
                "pt-BR");

        var meses =
            new List<MesFiltroDto>
            {
                new()
                {
                    Numero =
                        null,

                    Nome =
                        "Todos os meses"
                }
            };

        meses.AddRange(
            Enumerable
                .Range(
                    1,
                    12)
                .Select(
                    mes =>
                        new MesFiltroDto
                        {
                            Numero =
                                mes,

                            Nome =
                                cultura
                                    .DateTimeFormat
                                    .GetMonthName(
                                        mes)
                        }));

        return meses;
    }
}

public sealed class MesFiltroDto
{
    public int? Numero
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