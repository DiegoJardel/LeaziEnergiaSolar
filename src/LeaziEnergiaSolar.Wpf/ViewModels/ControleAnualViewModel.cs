using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Wpf.Utils;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class ControleAnualViewModel : ObservableObject
{
    private readonly IControleAnualService _controleAnualService;
    private readonly IVendedorService _vendedorService;

    [ObservableProperty]
    private int anoSelecionado = DateTime.Today.Year;

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
    private string mensagemErro = string.Empty;

    public ObservableCollection<VendedorDto> Vendedores { get; } = new();

    public ObservableCollection<ResumoAnualMesDto> Meses { get; } = new();

    public IReadOnlyList<int> AnosDisponiveis { get; } =
        YearFilterHelper.CriarAnosDisponiveis();

    public string PeriodoDescricao => VendedorSelecionado is null
        ? $"Resumo geral de {AnoSelecionado}"
        : $"{VendedorSelecionado.Nome} em {AnoSelecionado}";

    public ControleAnualViewModel(
        IControleAnualService controleAnualService,
        IVendedorService vendedorService)
    {
        _controleAnualService = controleAnualService;
        _vendedorService = vendedorService;
    }

    partial void OnAnoSelecionadoChanged(int value)
    {
        OnPropertyChanged(nameof(PeriodoDescricao));
    }

    partial void OnVendedorSelecionadoChanged(VendedorDto? value)
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
                "Não foi possível carregar o controle anual.";
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
                "Não foi possível aplicar os filtros do controle anual.";
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task LimparFiltrosAsync()
    {
        AnoSelecionado = DateTime.Today.Year;
        VendedorSelecionado = null;
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
        var controle = await _controleAnualService.ObterAsync(
            new FiltroControleAnualDto
            {
                Ano = AnoSelecionado,
                VendedorId = VendedorSelecionado?.Id
            });

        TotalVendido = controle.TotalVendido;
        TotalComissao = controle.TotalComissao;
        QuantidadeRegistros = controle.QuantidadeRegistros;
        QuantidadePagos = controle.QuantidadePagos;
        QuantidadePendentes = controle.QuantidadePendentes;

        Meses.Clear();

        foreach (var mes in controle.Meses)
        {
            Meses.Add(mes);
        }
    }
}
