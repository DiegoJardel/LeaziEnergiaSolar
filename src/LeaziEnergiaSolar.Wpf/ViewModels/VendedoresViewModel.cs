using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Wpf.Utils;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class VendedoresViewModel : ObservableObject
{
    private static readonly CultureInfo CulturaBrasileira =
        CultureInfo.GetCultureInfo("pt-BR");

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

    public ObservableCollection<VendedorDto> Vendedores { get; }
        = new();

    public bool EstaEditando =>
        VendedorId.HasValue;

    public string TituloFormulario =>
        EstaEditando
            ? "Editar vendedor"
            : "Novo vendedor";

    public VendedoresViewModel(
        IVendedorService vendedorService)
    {
        _vendedorService = vendedorService;
    }

    partial void OnVendedorIdChanged(
        int? value)
    {
        OnPropertyChanged(nameof(EstaEditando));
        OnPropertyChanged(nameof(TituloFormulario));
    }

    partial void OnCpfCnpjChanged(
        string value)
    {
        var formatado =
            MaskHelper.FormatCpfCnpj(value);

        if (value != formatado)
        {
            CpfCnpj = formatado;
        }
    }

    partial void OnTelefoneChanged(
        string value)
    {
        var formatado =
            MaskHelper.FormatPhone(value);

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
            var vendedores =
                await _vendedorService.ListarAsync(
                    Pesquisa?.Trim());

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
        NormalizarFormulario();

        if (!ValidarFormulario())
        {
            return;
        }

        if (!decimal.TryParse(
                PercentualComissao,
                NumberStyles.Number,
                CulturaBrasileira,
                out var comissao))
        {
            ExibirMensagem(
                "Informe um percentual de comissão válido.",
                true);

            return;
        }

        if (comissao <= 0)
        {
            ExibirMensagem(
                "O percentual de comissão deve ser maior que zero.",
                true);

            return;
        }

        if (comissao > 100)
        {
            ExibirMensagem(
                "O percentual de comissão não pode ser maior que 100%.",
                true);

            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado =
                await _vendedorService.SalvarAsync(
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

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            LimparFormulario(
                preservarMensagem: true);

            await CarregarListaInternaAsync();
        });
    }

    [RelayCommand]
    private void Editar(
        VendedorDto? vendedor)
    {
        if (vendedor is null)
        {
            return;
        }

        VendedorId = vendedor.Id;
        Nome = NormalizarNome(vendedor.Nome);
        CpfCnpj =
            MaskHelper.FormatCpfCnpj(
                vendedor.CpfCnpj);
        Telefone =
            MaskHelper.FormatPhone(
                vendedor.Telefone);
        Email =
            EmailValidator.Normalize(
                vendedor.Email);
        PercentualComissao =
            vendedor.PercentualComissao
                .ToString(
                    "N2",
                    CulturaBrasileira);
        Ativo = vendedor.Ativo;
        VendedorSelecionado = vendedor;
        Mensagem = string.Empty;
        MensagemEhErro = false;
    }

    [RelayCommand]
    private async Task AlterarStatusAsync(
        VendedorDto? vendedor)
    {
        if (vendedor is null)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado =
                await _vendedorService.AlterarStatusAsync(
                    vendedor.Id,
                    !vendedor.Ativo);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            LimparFormulario(
                preservarMensagem: true);

            await CarregarListaInternaAsync();
        });
    }

    [RelayCommand]
    private async Task ExcluirAsync(
        VendedorDto? vendedor)
    {
        if (vendedor is null)
        {
            return;
        }

        var resposta = MessageBox.Show(
            $"Deseja realmente excluir o vendedor " +
            $"\"{vendedor.Nome}\"?" +
            Environment.NewLine +
            Environment.NewLine +
            "Esta operação não poderá ser desfeita.",
            "Excluir vendedor",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (resposta != MessageBoxResult.Yes)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado =
                await _vendedorService.ExcluirAsync(
                    vendedor.Id);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            LimparFormulario(
                preservarMensagem: true);

            await CarregarListaInternaAsync();
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
        var vendedores =
            await _vendedorService.ListarAsync(
                Pesquisa?.Trim());

        Vendedores.Clear();

        foreach (var vendedor in vendedores)
        {
            Vendedores.Add(vendedor);
        }
    }

    private async Task ExecutarAsync(
        Func<Task> acao)
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

    private void NormalizarFormulario()
    {
        Nome = NormalizarNome(Nome);
        CpfCnpj =
            MaskHelper.FormatCpfCnpj(CpfCnpj);
        Telefone =
            MaskHelper.FormatPhone(Telefone);
        Email =
            EmailValidator.Normalize(Email);
        PercentualComissao =
            PercentualComissao?.Trim()
            ?? string.Empty;
    }

    private bool ValidarFormulario()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            ExibirMensagem(
                "Informe o nome do vendedor.",
                true);

            return false;
        }

        if (Nome.Length < 3)
        {
            ExibirMensagem(
                "O nome do vendedor deve possuir pelo menos 3 caracteres.",
                true);

            return false;
        }

        if (string.IsNullOrWhiteSpace(CpfCnpj))
        {
            ExibirMensagem(
                "Informe o CPF ou CNPJ.",
                true);

            return false;
        }

        if (!DocumentValidator.IsValidCpfCnpj(
                CpfCnpj))
        {
            ExibirMensagem(
                "Informe um CPF ou CNPJ válido.",
                true);

            return false;
        }

        if (string.IsNullOrWhiteSpace(Telefone))
        {
            ExibirMensagem(
                "Informe o telefone.",
                true);

            return false;
        }

        if (!TelefoneValido(Telefone))
        {
            ExibirMensagem(
                "Informe um telefone com DDD válido.",
                true);

            return false;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            ExibirMensagem(
                "Informe o e-mail.",
                true);

            return false;
        }

        if (!EmailValidator.IsValid(Email))
        {
            ExibirMensagem(
                "Informe um e-mail válido.",
                true);

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                PercentualComissao))
        {
            ExibirMensagem(
                "Informe o percentual de comissão.",
                true);

            return false;
        }

        return true;
    }

    private static string NormalizarNome(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var partes = value
            .Trim()
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

        return string
            .Join(
                ' ',
                partes)
            .ToUpperInvariant();
    }

    private static bool TelefoneValido(
        string? value)
    {
        var numbers =
            MaskHelper.OnlyNumbers(value);

        return numbers.Length is 10 or 11;
    }

    private void LimparFormulario(
        bool preservarMensagem = false)
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

    private void ExibirMensagem(
        string mensagem,
        bool ehErro)
    {
        Mensagem = mensagem;
        MensagemEhErro = ehErro;
    }
}