using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Wpf.Services;
using LeaziEnergiaSolar.Wpf.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class LancamentosViewModel : ObservableObject
{
    private static readonly CultureInfo CulturaBrasileira =
        CultureInfo.GetCultureInfo("pt-BR");

    private readonly ILancamentoService _lancamentoService;
    private readonly IVendedorService _vendedorService;
    private readonly IClienteService _clienteService;
    private readonly IUsuarioSessaoService _sessaoService;

    [ObservableProperty]
    private int? lancamentoId;

    [ObservableProperty]
    private DateTime? dataVenda = DateTime.Today;

    [ObservableProperty]
    private string cliente = string.Empty;

    [ObservableProperty]
    private string cpfCnpjCliente = string.Empty;

    [ObservableProperty]
    private ClienteDto? clienteSelecionado;

    [ObservableProperty]
    private int? usuarioIdResponsavel;

    [ObservableProperty]
    private VendedorDto? vendedorSelecionado;

    [ObservableProperty]
    private string valorVenda = "R$ 0,00";

    [ObservableProperty]
    private string percentualComissao = "5,00";

    [ObservableProperty]
    private string valorComissao = "R$ 0,00";

    [ObservableProperty]
    private StatusLancamento statusSelecionado =
        StatusLancamento.Pendente;

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

    public ObservableCollection<ClienteDto> Clientes { get; } = new();

    public IReadOnlyList<StatusLancamento> StatusDisponiveis { get; } =
        Enum.GetValues<StatusLancamento>();

    public bool EstaEditando => LancamentoId.HasValue;

    public string TituloFormulario => EstaEditando
        ? "Editar lançamento"
        : "Novo lançamento";

    public LancamentosViewModel(
        ILancamentoService lancamentoService,
        IVendedorService vendedorService,
        IClienteService clienteService,
        IUsuarioSessaoService sessaoService)
    {
        _lancamentoService = lancamentoService
            ?? throw new ArgumentNullException(nameof(lancamentoService));

        _vendedorService = vendedorService
            ?? throw new ArgumentNullException(nameof(vendedorService));

        _clienteService = clienteService
            ?? throw new ArgumentNullException(nameof(clienteService));

        _sessaoService = sessaoService
            ?? throw new ArgumentNullException(nameof(sessaoService));
    }

    partial void OnLancamentoIdChanged(int? value)
    {
        OnPropertyChanged(nameof(EstaEditando));
        OnPropertyChanged(nameof(TituloFormulario));
    }

    partial void OnClienteSelecionadoChanged(
        ClienteDto? value)
    {
        if (value is null)
        {
            return;
        }

        Cliente = value.NomeRazaoSocial;

        CpfCnpjCliente = MaskHelper.FormatCpfCnpj(
            value.CpfCnpj);
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

    partial void OnVendedorSelecionadoChanged(
        VendedorDto? value)
    {
        if (!EstaEditando &&
            value is not null)
        {
            PercentualComissao = value.PercentualComissao.ToString(
                "N2",
                CulturaBrasileira);
        }
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        await ExecutarAsync(
            async () =>
            {
                await CarregarVendedoresAsync();
                await CarregarClientesAsync();
                await CarregarLancamentosAsync();
            },
            "carregar os lançamentos");
    }

    [RelayCommand]
    private async Task FiltrarAsync()
    {
        await ExecutarAsync(
            CarregarLancamentosAsync,
            "filtrar os lançamentos");
    }

    [RelayCommand]
    private async Task LimparFiltrosAsync()
    {
        Pesquisa = string.Empty;
        FiltroDataInicial = null;
        FiltroDataFinal = null;
        FiltroVendedor = null;
        FiltroStatus = null;

        await ExecutarAsync(
            CarregarLancamentosAsync,
            "limpar os filtros");
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        NormalizarFormulario();

        if (!ValidarCamposObrigatorios())
        {
            return;
        }

        if (!ValidarDocumentoCliente())
        {
            return;
        }

        if (!TentarObterDecimal(
                ValorVenda,
                out var valorVendaDecimal))
        {
            ExibirMensagem(
                "Informe um valor de venda válido.",
                true);

            return;
        }

        if (valorVendaDecimal <= 0)
        {
            ExibirMensagem(
                "O valor da venda deve ser maior que zero.",
                true);

            return;
        }

        if (!TentarObterDecimal(
                PercentualComissao,
                out var percentualComissaoDecimal))
        {
            ExibirMensagem(
                "Informe um percentual de comissão válido.",
                true);

            return;
        }

        if (percentualComissaoDecimal <= 0)
        {
            ExibirMensagem(
                "O percentual de comissão deve ser maior que zero.",
                true);

            return;
        }

        if (percentualComissaoDecimal > 100)
        {
            ExibirMensagem(
                "O percentual de comissão não pode ser maior que 100%.",
                true);

            return;
        }

        var usuarioId = UsuarioIdResponsavel
            ?? _sessaoService.UsuarioAtual?.Id;

        if (!usuarioId.HasValue ||
            usuarioId.Value <= 0)
        {
            ExibirMensagem(
                "Não foi possível identificar o usuário responsável pelo lançamento. " +
                "Saia do sistema, entre novamente e tente salvar.",
                true);

            return;
        }

        var vendedorId = VendedorSelecionado?.Id;

        if (!vendedorId.HasValue ||
            vendedorId.Value <= 0)
        {
            ExibirMensagem(
                "O vendedor selecionado não possui um identificador válido.",
                true);

            return;
        }

        var clienteId = ClienteSelecionado?.Id;

        if (clienteId.HasValue &&
            clienteId.Value <= 0)
        {
            ExibirMensagem(
                "O cliente selecionado não possui um identificador válido.",
                true);

            return;
        }

        var dto = new SalvarLancamentoDto
        {
            Id = LancamentoId,
            DataVenda = DataVenda!.Value,
            Cliente = Cliente,
            CpfCnpjCliente = CpfCnpjCliente,
            ClienteId = clienteId,
            UsuarioId = usuarioId.Value,
            VendedorId = vendedorId.Value,
            ValorVenda = valorVendaDecimal,
            PercentualComissao = percentualComissaoDecimal,
            Status = StatusSelecionado,
            Observacao = Observacao
        };

        await ExecutarAsync(
            async () =>
            {
                var resultado = await _lancamentoService.SalvarAsync(dto);

                ExibirMensagem(
                    resultado.Mensagem,
                    !resultado.Sucesso);

                if (!resultado.Sucesso)
                {
                    return;
                }

                LimparFormulario(preservarMensagem: true);

                await CarregarLancamentosAsync();
            },
            "salvar o lançamento");
    }

    [RelayCommand]
    private void Editar(
        LancamentoDto? lancamento)
    {
        if (lancamento is null)
        {
            return;
        }

        LancamentoId = lancamento.Id;
        UsuarioIdResponsavel = lancamento.UsuarioId;
        DataVenda = lancamento.DataVenda;

        ClienteSelecionado = lancamento.ClienteId.HasValue
            ? Clientes.FirstOrDefault(
                cliente => cliente.Id == lancamento.ClienteId.Value)
            : null;

        Cliente = NormalizarNome(lancamento.Cliente);

        CpfCnpjCliente = MaskHelper.FormatCpfCnpj(
            lancamento.CpfCnpjCliente);

        VendedorSelecionado = Vendedores.FirstOrDefault(
            vendedor => vendedor.Id == lancamento.VendedorId);

        ValorVenda = lancamento.ValorVenda.ToString(
            "C2",
            CulturaBrasileira);

        PercentualComissao = lancamento.PercentualComissao.ToString(
            "N2",
            CulturaBrasileira);

        StatusSelecionado = lancamento.Status;

        Observacao = lancamento.Observacao?.Trim()
            ?? string.Empty;

        AtualizarCalculoComissao();

        Mensagem = string.Empty;
        MensagemEhErro = false;
    }

    [RelayCommand]
    private async Task AlternarStatusAsync(
        LancamentoDto? lancamento)
    {
        if (lancamento is null)
        {
            return;
        }

        var novoStatus = lancamento.Status == StatusLancamento.Pago
            ? StatusLancamento.Pendente
            : StatusLancamento.Pago;

        await ExecutarAsync(
            async () =>
            {
                var resultado = await _lancamentoService.AlterarStatusAsync(
                    lancamento.Id,
                    novoStatus);

                ExibirMensagem(
                    resultado.Mensagem,
                    !resultado.Sucesso);

                if (!resultado.Sucesso)
                {
                    return;
                }

                await CarregarLancamentosAsync();
            },
            "alterar o status do lançamento");
    }

    [RelayCommand]
    private async Task ExcluirAsync(
        LancamentoDto? lancamento)
    {
        if (lancamento is null)
        {
            return;
        }

        await ExecutarAsync(
            async () =>
            {
                var resultado = await _lancamentoService.ExcluirAsync(
                    lancamento.Id);

                ExibirMensagem(
                    resultado.Mensagem,
                    !resultado.Sucesso);

                if (!resultado.Sucesso)
                {
                    return;
                }

                if (LancamentoId == lancamento.Id)
                {
                    LimparFormulario(preservarMensagem: true);
                }

                await CarregarLancamentosAsync();
            },
            "excluir o lançamento");
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

        foreach (var vendedor in vendedores.Where(
                     vendedor => vendedor.Ativo))
        {
            Vendedores.Add(vendedor);
        }

        if (vendedorAtualId.HasValue)
        {
            VendedorSelecionado = Vendedores.FirstOrDefault(
                vendedor => vendedor.Id == vendedorAtualId.Value);
        }
    }

    private async Task CarregarClientesAsync()
    {
        var clienteAtualId = ClienteSelecionado?.Id;

        var clientes = await _clienteService.ListarAsync(
            ativo: true);

        Clientes.Clear();

        foreach (var cliente in clientes)
        {
            Clientes.Add(cliente);
        }

        if (clienteAtualId.HasValue)
        {
            ClienteSelecionado = Clientes.FirstOrDefault(
                cliente => cliente.Id == clienteAtualId.Value);
        }
    }

    private async Task CarregarLancamentosAsync()
    {
        var lancamentos = await _lancamentoService.ListarAsync(
            new FiltroLancamentoDto
            {
                Pesquisa = Pesquisa?.Trim() ?? string.Empty,
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
        if (!TentarObterDecimal(
                ValorVenda,
                out var venda) ||
            !TentarObterDecimal(
                PercentualComissao,
                out var percentual))
        {
            ValorComissao = "R$ 0,00";

            return;
        }

        if (venda <= 0 ||
            percentual <= 0)
        {
            ValorComissao = "R$ 0,00";

            return;
        }

        var comissao = _lancamentoService.CalcularComissao(
            venda,
            percentual);

        ValorComissao = comissao.ToString(
            "C2",
            CulturaBrasileira);
    }

    private void NormalizarFormulario()
    {
        Cliente = NormalizarNome(Cliente);

        CpfCnpjCliente = string.IsNullOrWhiteSpace(CpfCnpjCliente)
            ? string.Empty
            : MaskHelper.FormatCpfCnpj(CpfCnpjCliente);

        ValorVenda = ValorVenda?.Trim()
            ?? "R$ 0,00";

        PercentualComissao = PercentualComissao?.Trim()
            ?? string.Empty;

        Observacao = Observacao?.Trim()
            ?? string.Empty;
    }

    private bool ValidarCamposObrigatorios()
    {
        if (!DataVenda.HasValue)
        {
            ExibirMensagem(
                "Informe a data da venda.",
                true);

            return false;
        }

        if (string.IsNullOrWhiteSpace(Cliente))
        {
            ExibirMensagem(
                "Informe o nome do cliente.",
                true);

            return false;
        }

        if (Cliente.Length < 3)
        {
            ExibirMensagem(
                "O nome do cliente deve possuir pelo menos 3 caracteres.",
                true);

            return false;
        }

        if (VendedorSelecionado is null)
        {
            ExibirMensagem(
                "Selecione o vendedor.",
                true);

            return false;
        }

        if (string.IsNullOrWhiteSpace(ValorVenda))
        {
            ExibirMensagem(
                "Informe o valor da venda.",
                true);

            return false;
        }

        if (string.IsNullOrWhiteSpace(PercentualComissao))
        {
            ExibirMensagem(
                "Informe o percentual de comissão.",
                true);

            return false;
        }

        return true;
    }

    private bool ValidarDocumentoCliente()
    {
        if (string.IsNullOrWhiteSpace(CpfCnpjCliente))
        {
            return true;
        }

        if (DocumentValidator.IsValidCpfCnpj(CpfCnpjCliente))
        {
            return true;
        }

        ExibirMensagem(
            "Informe um CPF ou CNPJ válido para o cliente.",
            true);

        return false;
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
            .Join(' ', partes)
            .ToUpperInvariant();
    }

    private static bool TentarObterDecimal(
        string? valor,
        out decimal resultado)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            resultado = 0m;

            return false;
        }

        return decimal.TryParse(
            valor,
            NumberStyles.Currency,
            CulturaBrasileira,
            out resultado);
    }

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
            EstaCarregando = true;

            await acao();
        }
        catch (DbUpdateException ex)
        {
            RegistrarExcecao(
                descricaoOperacao,
                ex);

            ExibirMensagem(
                ObterMensagemBancoDados(ex),
                true);
        }
        catch (SqliteException ex)
        {
            RegistrarExcecao(
                descricaoOperacao,
                ex);

            ExibirMensagem(
                ObterMensagemSqlite(ex),
                true);
        }
        catch (InvalidOperationException ex)
        {
            RegistrarExcecao(
                descricaoOperacao,
                ex);

            ExibirMensagem(
                $"Não foi possível {descricaoOperacao}: {ex.Message}",
                true);
        }
        catch (Exception ex)
        {
            RegistrarExcecao(
                descricaoOperacao,
                ex);

            var mensagemRaiz = ex
                .GetBaseException()
                .Message;

            ExibirMensagem(
                $"Não foi possível {descricaoOperacao}. " +
                $"Detalhes: {mensagemRaiz}",
                true);
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    private static string ObterMensagemBancoDados(
        DbUpdateException exception)
    {
        var mensagemRaiz = exception
            .GetBaseException()
            .Message;

        if (ContemTexto(
                mensagemRaiz,
                "FOREIGN KEY constraint failed"))
        {
            return "Não foi possível salvar o lançamento porque um dos vínculos " +
                   "informados não existe no banco de dados. Verifique o cliente, " +
                   "o vendedor e o usuário responsável.";
        }

        if (ContemTexto(
                mensagemRaiz,
                "NOT NULL constraint failed"))
        {
            return "Não foi possível salvar porque o banco de dados exige um campo " +
                   $"que não foi informado. Detalhes: {mensagemRaiz}";
        }

        if (ContemTexto(
                mensagemRaiz,
                "UNIQUE constraint failed"))
        {
            return "Não foi possível salvar porque já existe um registro com uma " +
                   $"informação que deve ser única. Detalhes: {mensagemRaiz}";
        }

        if (ContemTexto(
                mensagemRaiz,
                "no column named") ||
            ContemTexto(
                mensagemRaiz,
                "no such column") ||
            ContemTexto(
                mensagemRaiz,
                "has no column named"))
        {
            return "A estrutura do banco de dados está desatualizada. " +
                   "Crie ou aplique a migration mais recente. " +
                   $"Detalhes: {mensagemRaiz}";
        }

        if (ContemTexto(
                mensagemRaiz,
                "no such table"))
        {
            return "A tabela necessária não existe no banco de dados. " +
                   "Verifique se a migration foi criada e aplicada. " +
                   $"Detalhes: {mensagemRaiz}";
        }

        return "Não foi possível gravar o lançamento no banco de dados. " +
               $"Detalhes: {mensagemRaiz}";
    }

    private static string ObterMensagemSqlite(
        SqliteException exception)
    {
        var mensagem = exception.Message;

        if (ContemTexto(
                mensagem,
                "no such table"))
        {
            return "A tabela de lançamentos não existe no banco de dados. " +
                   "Crie e aplique a migration inicial. " +
                   $"Detalhes: {mensagem}";
        }

        if (ContemTexto(
                mensagem,
                "no such column") ||
            ContemTexto(
                mensagem,
                "has no column named"))
        {
            return "A estrutura do banco de dados está diferente do modelo atual. " +
                   "Crie ou aplique a migration mais recente. " +
                   $"Detalhes: {mensagem}";
        }

        if (ContemTexto(
                mensagem,
                "FOREIGN KEY constraint failed"))
        {
            return "Um dos registros relacionados não foi encontrado no banco. " +
                   "Verifique o cliente, o vendedor e o usuário responsável.";
        }

        return $"Erro no banco de dados SQLite: {mensagem}";
    }

    private static bool ContemTexto(
        string texto,
        string valor)
    {
        return texto.Contains(
            valor,
            StringComparison.OrdinalIgnoreCase);
    }

    private static void RegistrarExcecao(
        string descricaoOperacao,
        Exception exception)
    {
        Debug.WriteLine(
            "==================================================");

        Debug.WriteLine(
            $"Erro ao {descricaoOperacao}");

        Debug.WriteLine(
            exception.ToString());

        Debug.WriteLine(
            "==================================================");
    }

    private void LimparFormulario(
        bool preservarMensagem = false)
    {
        LancamentoId = null;
        UsuarioIdResponsavel = null;
        DataVenda = DateTime.Today;
        ClienteSelecionado = null;
        Cliente = string.Empty;
        CpfCnpjCliente = string.Empty;
        VendedorSelecionado = null;
        ValorVenda = "R$ 0,00";
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

    private void ExibirMensagem(
        string mensagem,
        bool ehErro)
    {
        Mensagem = mensagem;
        MensagemEhErro = ehErro;
    }
}