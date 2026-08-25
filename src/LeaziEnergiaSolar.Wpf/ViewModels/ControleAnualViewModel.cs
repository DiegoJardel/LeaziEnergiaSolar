using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class ControleAnualViewModel : ObservableObject
{
    private readonly IControleAnualService _controleAnualService;
    private readonly IVendedorService _vendedorService;

    [ObservableProperty]
    private int anoSelecionado =
        DateTime.Today.Year;

    [ObservableProperty]
    private VendedorDto? vendedorSelecionado;

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

    public ObservableCollection<ResumoAnualMesDto> Meses { get; } =
        new();

    public ObservableCollection<int> AnosDisponiveis { get; } =
        new();

    public string PeriodoDescricao =>
        VendedorSelecionado is null
            ? $"Resumo geral de {AnoSelecionado}"
            : $"{VendedorSelecionado.Nome} em {AnoSelecionado}";

    public ControleAnualViewModel(
        IControleAnualService controleAnualService,
        IVendedorService vendedorService)
    {
        _controleAnualService =
            controleAnualService;

        _vendedorService =
            vendedorService;
    }

    partial void OnAnoSelecionadoChanged(
        int value)
    {
        OnPropertyChanged(
            nameof(PeriodoDescricao));
    }

    partial void OnVendedorSelecionadoChanged(
        VendedorDto? value)
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
                "Não foi possível carregar o controle anual. " +
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
                "do controle anual. " +
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

        VendedorSelecionado =
            null;

        await FiltrarAsync();
    }

    private async Task CarregarAnosDisponiveisAsync()
    {
        var anoAtual =
            DateTime.Today.Year;

        var anoSelecionadoAtual =
            AnoSelecionado;

        var anos =
            await _controleAnualService
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
        var controle =
            await _controleAnualService.ObterAsync(
                new FiltroControleAnualDto
                {
                    Ano =
                        AnoSelecionado,

                    VendedorId =
                        VendedorSelecionado?.Id
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

        Meses.Clear();

        foreach (var mes in controle.Meses)
        {
            Meses.Add(
                mes);
        }
    }
}