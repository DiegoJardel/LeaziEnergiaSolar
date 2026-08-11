using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Wpf.Utils;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class VendedoresViewModel : ObservableObject
{
    private readonly IVendedorService _vendedorService;

    [ObservableProperty]
    private int? vendedorId;

    [ObservableProperty]
    private string nome = string.Empty;

    [ObservableProperty]
    private string cpfCnpj = string.Empty;

    [ObservableProperty]
    private string telefone = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string percentualComissao = "5,00";

    [ObservableProperty]
    private bool ativo = true;

    [ObservableProperty]
    private string pesquisa = string.Empty;

    [ObservableProperty]
    private VendedorDto? vendedorSelecionado;

    [ObservableProperty]
    private string mensagem = string.Empty;

    [ObservableProperty]
    private bool mensagemEhErro;

    [ObservableProperty]
    private bool estaCarregando;

    public ObservableCollection<VendedorDto> Vendedores { get; } = new();

    public bool EstaEditando => VendedorId.HasValue;

    public string TituloFormulario => EstaEditando
        ? "Editar vendedor"
        : "Novo vendedor";

    public VendedoresViewModel(IVendedorService vendedorService)
    {
        _vendedorService = vendedorService;
    }

    partial void OnVendedorIdChanged(int? value)
    {
        OnPropertyChanged(nameof(EstaEditando));
        OnPropertyChanged(nameof(TituloFormulario));
    }

    partial void OnCpfCnpjChanged(string value)
    {
        var formatado = MaskHelper.FormatCpfCnpj(value);

        if (value != formatado)
        {
            CpfCnpj = formatado;
        }
    }

    partial void OnTelefoneChanged(string value)
    {
        var formatado = MaskHelper.FormatPhone(value);

        if (value != formatado)
        {
            Telefone = formatado;
        }
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        await ExecutarAsync(async () =>
        {
            var vendedores = await _vendedorService.ListarAsync(Pesquisa);

            Vendedores.Clear();

            foreach (var vendedor in vendedores)
            {
                Vendedores.Add(vendedor);
            }
        });
    }

    [RelayCommand]
    private async Task PesquisarAsync()
    {
        await CarregarAsync();
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (!decimal.TryParse(
                PercentualComissao,
                NumberStyles.Number,
                CultureInfo.GetCultureInfo("pt-BR"),
                out var comissao))
        {
            ExibirMensagem("Informe um percentual de comissão válido.", true);
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado = await _vendedorService.SalvarAsync(
                new SalvarVendedorDto
                {
                    Id = VendedorId,
                    Nome = Nome,
                    CpfCnpj = CpfCnpj,
                    Telefone = Telefone,
                    Email = Email,
                    PercentualComissao = comissao,
                    Ativo = Ativo
                });

            ExibirMensagem(resultado.Mensagem, !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            LimparFormulario(preservarMensagem: true);
            await CarregarListaInternaAsync();
        });
    }

    [RelayCommand]
    private void Editar(VendedorDto? vendedor)
    {
        if (vendedor is null)
        {
            return;
        }

        VendedorId = vendedor.Id;
        Nome = vendedor.Nome;
        CpfCnpj = MaskHelper.FormatCpfCnpj(vendedor.CpfCnpj);
        Telefone = MaskHelper.FormatPhone(vendedor.Telefone);
        Email = vendedor.Email;
        PercentualComissao = vendedor.PercentualComissao
            .ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
        Ativo = vendedor.Ativo;
        VendedorSelecionado = vendedor;
        Mensagem = string.Empty;
    }

    [RelayCommand]
    private async Task AlterarStatusAsync(VendedorDto? vendedor)
    {
        if (vendedor is null)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado = await _vendedorService.AlterarStatusAsync(
                vendedor.Id,
                !vendedor.Ativo);

            ExibirMensagem(resultado.Mensagem, !resultado.Sucesso);

            if (resultado.Sucesso)
            {
                LimparFormulario(preservarMensagem: true);
                await CarregarListaInternaAsync();
            }
        });
    }

    [RelayCommand]
    private void Novo()
    {
        LimparFormulario();
    }

    [RelayCommand]
    private void Limpar()
    {
        LimparFormulario();
    }

    private async Task CarregarListaInternaAsync()
    {
        var vendedores = await _vendedorService.ListarAsync(Pesquisa);

        Vendedores.Clear();

        foreach (var vendedor in vendedores)
        {
            Vendedores.Add(vendedor);
        }
    }

    private async Task ExecutarAsync(Func<Task> acao)
    {
        if (EstaCarregando)
        {
            return;
        }

        try
        {
            EstaCarregando = true;
            await acao();
        }
        catch (Exception)
        {
            ExibirMensagem(
                "Não foi possível concluir a operação. Tente novamente.",
                true);
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    private void LimparFormulario(bool preservarMensagem = false)
    {
        VendedorId = null;
        Nome = string.Empty;
        CpfCnpj = string.Empty;
        Telefone = string.Empty;
        Email = string.Empty;
        PercentualComissao = "5,00";
        Ativo = true;
        VendedorSelecionado = null;

        if (!preservarMensagem)
        {
            Mensagem = string.Empty;
            MensagemEhErro = false;
        }
    }

    private void ExibirMensagem(string mensagem, bool ehErro)
    {
        Mensagem = mensagem;
        MensagemEhErro = ehErro;
    }
}
