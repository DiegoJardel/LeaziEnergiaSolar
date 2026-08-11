using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Wpf.Utils;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class LancamentosViewModel : ObservableObject
{
    private readonly ILancamentoService _lancamentoService;
    private readonly IVendedorService _vendedorService;

    [ObservableProperty]
    private int? lancamentoId;

    [ObservableProperty]
    private DateTime? dataVenda = DateTime.Today;

    [ObservableProperty]
    private string cliente = string.Empty;

    [ObservableProperty]
    private string cpfCnpjCliente = string.Empty;

    [ObservableProperty]
    private VendedorDto? vendedorSelecionado;

    [ObservableProperty]
    private string valorVenda = string.Empty;

    [ObservableProperty]
    private string percentualComissao = "5,00";

    [ObservableProperty]
    private string valorComissao = "R$ 0,00";

    [ObservableProperty]
    private StatusLancamento statusSelecionado = StatusLancamento.Pendente;

    [ObservableProperty]
    private string observacao = string.Empty;

    [ObservableProperty]
    private string pesquisa = string.Empty;

    [ObservableProperty]
    private DateTime? filtroDataInicial;

    [ObservableProperty]
    private DateTime? filtroDataFinal;

    [ObservableProperty]
    private VendedorDto? filtroVendedor;

    [ObservableProperty]
    private StatusLancamento? filtroStatus;

    [ObservableProperty]
    private string mensagem = string.Empty;

    [ObservableProperty]
    private bool mensagemEhErro;

    [ObservableProperty]
    private bool estaCarregando;

    public ObservableCollection<LancamentoDto> Lancamentos { get; } = new();

    public ObservableCollection<VendedorDto> Vendedores { get; } = new();

    public IReadOnlyList<StatusLancamento> StatusDisponiveis { get; } =
        Enum.GetValues<StatusLancamento>();

    public bool EstaEditando => LancamentoId.HasValue;

    public string TituloFormulario => EstaEditando
        ? "Editar lançamento"
        : "Novo lançamento";

    public LancamentosViewModel(
        ILancamentoService lancamentoService,
        IVendedorService vendedorService)
    {
        _lancamentoService = lancamentoService;
        _vendedorService = vendedorService;
    }

    partial void OnLancamentoIdChanged(int? value)
    {
        OnPropertyChanged(nameof(EstaEditando));
        OnPropertyChanged(nameof(TituloFormulario));
    }

    partial void OnCpfCnpjClienteChanged(string value)
    {
        var formatado = MaskHelper.FormatCpfCnpj(value);

        if (value != formatado)
        {
            CpfCnpjCliente = formatado;
        }
    }

    partial void OnValorVendaChanged(string value)
    {
        AtualizarCalculoComissao();
    }

    partial void OnPercentualComissaoChanged(string value)
    {
        AtualizarCalculoComissao();
    }

    partial void OnVendedorSelecionadoChanged(VendedorDto? value)
    {
        if (!EstaEditando && value is not null)
        {
            PercentualComissao = value.PercentualComissao.ToString(
                "N2",
                CultureInfo.GetCultureInfo("pt-BR"));
        }
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        await ExecutarAsync(async () =>
        {
            await CarregarVendedoresAsync();
            await CarregarLancamentosAsync();
        });
    }

    [RelayCommand]
    private async Task FiltrarAsync()
    {
        await ExecutarAsync(CarregarLancamentosAsync);
    }

    [RelayCommand]
    private async Task LimparFiltrosAsync()
    {
        Pesquisa = string.Empty;
        FiltroDataInicial = null;
        FiltroDataFinal = null;
        FiltroVendedor = null;
        FiltroStatus = null;

        await ExecutarAsync(CarregarLancamentosAsync);
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (!TentarObterDecimal(ValorVenda, out var valorVendaDecimal))
        {
            ExibirMensagem("Informe um valor de venda válido.", true);
            return;
        }

        if (!TentarObterDecimal(
                PercentualComissao,
                out var percentualComissaoDecimal))
        {
            ExibirMensagem("Informe um percentual de comissão válido.", true);
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado = await _lancamentoService.SalvarAsync(
                new SalvarLancamentoDto
                {
                    Id = LancamentoId,
                    DataVenda = DataVenda ?? default,
                    Cliente = Cliente,
                    CpfCnpjCliente = CpfCnpjCliente,
                    VendedorId = VendedorSelecionado?.Id ?? 0,
                    ValorVenda = valorVendaDecimal,
                    PercentualComissao = percentualComissaoDecimal,
                    Status = StatusSelecionado,
                    Observacao = Observacao
                });

            ExibirMensagem(resultado.Mensagem, !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            LimparFormulario(preservarMensagem: true);
            await CarregarLancamentosAsync();
        });
    }

    [RelayCommand]
    private void Editar(LancamentoDto? lancamento)
    {
        if (lancamento is null)
        {
            return;
        }

        LancamentoId = lancamento.Id;
        DataVenda = lancamento.DataVenda;
        Cliente = lancamento.Cliente;
        CpfCnpjCliente = MaskHelper.FormatCpfCnpj(lancamento.CpfCnpjCliente);
        VendedorSelecionado = Vendedores.FirstOrDefault(
            vendedor => vendedor.Id == lancamento.VendedorId);
        ValorVenda = lancamento.ValorVenda.ToString(
            "N2",
            CultureInfo.GetCultureInfo("pt-BR"));
        PercentualComissao = lancamento.PercentualComissao.ToString(
            "N2",
            CultureInfo.GetCultureInfo("pt-BR"));
        StatusSelecionado = lancamento.Status;
        Observacao = lancamento.Observacao;
        AtualizarCalculoComissao();
        Mensagem = string.Empty;
    }

    [RelayCommand]
    private async Task AlternarStatusAsync(LancamentoDto? lancamento)
    {
        if (lancamento is null)
        {
            return;
        }

        var novoStatus = lancamento.Status == StatusLancamento.Pago
            ? StatusLancamento.Pendente
            : StatusLancamento.Pago;

        await ExecutarAsync(async () =>
        {
            var resultado = await _lancamentoService.AlterarStatusAsync(
                lancamento.Id,
                novoStatus);

            ExibirMensagem(resultado.Mensagem, !resultado.Sucesso);

            if (resultado.Sucesso)
            {
                await CarregarLancamentosAsync();
            }
        });
    }

    [RelayCommand]
    private async Task ExcluirAsync(LancamentoDto? lancamento)
    {
        if (lancamento is null)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado = await _lancamentoService.ExcluirAsync(lancamento.Id);

            ExibirMensagem(resultado.Mensagem, !resultado.Sucesso);

            if (resultado.Sucesso)
            {
                if (LancamentoId == lancamento.Id)
                {
                    LimparFormulario(preservarMensagem: true);
                }

                await CarregarLancamentosAsync();
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

    private async Task CarregarVendedoresAsync()
    {
        var vendedorAtualId = VendedorSelecionado?.Id;
        var vendedores = await _vendedorService.ListarAsync();

        Vendedores.Clear();

        foreach (var vendedor in vendedores.Where(vendedor => vendedor.Ativo))
        {
            Vendedores.Add(vendedor);
        }

        if (vendedorAtualId.HasValue)
        {
            VendedorSelecionado = Vendedores.FirstOrDefault(
                vendedor => vendedor.Id == vendedorAtualId.Value);
        }
    }

    private async Task CarregarLancamentosAsync()
    {
        var lancamentos = await _lancamentoService.ListarAsync(
            new FiltroLancamentoDto
            {
                Pesquisa = Pesquisa,
                DataInicial = FiltroDataInicial,
                DataFinal = FiltroDataFinal,
                VendedorId = FiltroVendedor?.Id,
                Status = FiltroStatus
            });

        Lancamentos.Clear();

        foreach (var lancamento in lancamentos)
        {
            Lancamentos.Add(lancamento);
        }
    }

    private void AtualizarCalculoComissao()
    {
        if (!TentarObterDecimal(ValorVenda, out var venda) ||
            !TentarObterDecimal(PercentualComissao, out var percentual))
        {
            ValorComissao = "R$ 0,00";
            return;
        }

        var comissao = _lancamentoService.CalcularComissao(venda, percentual);
        ValorComissao = comissao.ToString(
            "C2",
            CultureInfo.GetCultureInfo("pt-BR"));
    }

    private static bool TentarObterDecimal(
        string valor,
        out decimal resultado)
    {
        return decimal.TryParse(
            valor,
            NumberStyles.Number,
            CultureInfo.GetCultureInfo("pt-BR"),
            out resultado);
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
        LancamentoId = null;
        DataVenda = DateTime.Today;
        Cliente = string.Empty;
        CpfCnpjCliente = string.Empty;
        VendedorSelecionado = null;
        ValorVenda = string.Empty;
        PercentualComissao = "5,00";
        ValorComissao = "R$ 0,00";
        StatusSelecionado = StatusLancamento.Pendente;
        Observacao = string.Empty;

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
