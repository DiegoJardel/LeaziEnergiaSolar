using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Wpf.Utils;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class ClientesViewModel : ObservableObject
{
    private readonly IClienteService _clienteService;
    private readonly ICepService _cepService;
    private readonly IIbgeLocalidadeService _ibgeLocalidadeService;

    private bool _atualizandoLocalidade;

    [ObservableProperty]
    private int? clienteId;

    [ObservableProperty]
    private TipoPessoa tipoPessoa = TipoPessoa.Fisica;

    [ObservableProperty]
    private string nomeRazaoSocial = string.Empty;

    [ObservableProperty]
    private string nomeFantasia = string.Empty;

    [ObservableProperty]
    private string cpfCnpj = string.Empty;

    [ObservableProperty]
    private string rgInscricaoEstadual = string.Empty;

    [ObservableProperty]
    private string telefone = string.Empty;

    [ObservableProperty]
    private string whatsApp = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string cep = string.Empty;

    [ObservableProperty]
    private string logradouro = string.Empty;

    [ObservableProperty]
    private string numero = string.Empty;

    [ObservableProperty]
    private string complemento = string.Empty;

    [ObservableProperty]
    private string bairro = string.Empty;

    [ObservableProperty]
    private string cidade = string.Empty;

    [ObservableProperty]
    private string codigoIbgeCidade = string.Empty;

    [ObservableProperty]
    private string siglaUf = string.Empty;

    [ObservableProperty]
    private string estado = string.Empty;

    [ObservableProperty]
    private string codigoIbgeUf = string.Empty;

    [ObservableProperty]
    private EstadoDto? estadoSelecionado;

    [ObservableProperty]
    private MunicipioDto? municipioSelecionado;

    [ObservableProperty]
    private string pontoReferencia = string.Empty;

    [ObservableProperty]
    private string observacao = string.Empty;

    [ObservableProperty]
    private bool ativo = true;

    [ObservableProperty]
    private string pesquisa = string.Empty;

    [ObservableProperty]
    private string filtroStatus = "Todos";

    [ObservableProperty]
    private ClienteDto? clienteSelecionado;

    [ObservableProperty]
    private string mensagem = string.Empty;

    [ObservableProperty]
    private bool mensagemEhErro;

    [ObservableProperty]
    private bool estaCarregando;

    public ObservableCollection<ClienteDto> Clientes { get; }
        = new();

    public ObservableCollection<EstadoDto> Estados { get; }
        = new();

    public ObservableCollection<MunicipioDto> Municipios { get; }
        = new();

    public IReadOnlyList<TipoPessoa> TiposPessoa { get; } =
        Enum.GetValues<TipoPessoa>();

    public IReadOnlyList<string> FiltrosStatus { get; } =
        new[]
        {
            "Todos",
            "Ativos",
            "Inativos"
        };

    public bool EstaEditando =>
        ClienteId.HasValue;

    public string TituloFormulario =>
        EstaEditando
            ? "Editar cliente"
            : "Novo cliente";

    public ClientesViewModel(
        IClienteService clienteService,
        ICepService cepService,
        IIbgeLocalidadeService ibgeLocalidadeService)
    {
        _clienteService = clienteService;
        _cepService = cepService;
        _ibgeLocalidadeService = ibgeLocalidadeService;
    }

    partial void OnClienteIdChanged(
        int? value)
    {
        OnPropertyChanged(nameof(EstaEditando));
        OnPropertyChanged(nameof(TituloFormulario));
    }

    partial void OnTipoPessoaChanged(
        TipoPessoa value)
    {
        CpfCnpj = string.Empty;
    }

    partial void OnEstadoSelecionadoChanged(
        EstadoDto? value)
    {
        if (_atualizandoLocalidade)
        {
            return;
        }

        MunicipioSelecionado = null;
        Municipios.Clear();
        Cidade = string.Empty;
        CodigoIbgeCidade = string.Empty;

        if (value is null)
        {
            SiglaUf = string.Empty;
            Estado = string.Empty;
            CodigoIbgeUf = string.Empty;

            return;
        }

        SiglaUf = value.Sigla;
        Estado = value.Nome;
        CodigoIbgeUf = value.CodigoIbge;

        _ = CarregarMunicipiosAsync(
            value.CodigoIbge,
            limparCidade: true);
    }

    partial void OnMunicipioSelecionadoChanged(
        MunicipioDto? value)
    {
        if (_atualizandoLocalidade)
        {
            return;
        }

        if (value is null)
        {
            Cidade = string.Empty;
            CodigoIbgeCidade = string.Empty;

            return;
        }

        Cidade = NormalizarTexto(value.Nome);
        CodigoIbgeCidade = MaskHelper.OnlyNumbers(
            value.CodigoIbge);
    }

    partial void OnCpfCnpjChanged(
        string value)
    {
        var formatado = MaskHelper.FormatCpfCnpj(
            value);

        if (value != formatado)
        {
            CpfCnpj = formatado;
        }
    }

    partial void OnRgInscricaoEstadualChanged(
        string value)
    {
        var formatado = MaskHelper.FormatRg(value);

        if (value != formatado)
        {
            RgInscricaoEstadual = formatado;
        }
    }

    partial void OnTelefoneChanged(
        string value)
    {
        var formatado = MaskHelper.FormatPhone(
            value);

        if (value != formatado)
        {
            Telefone = formatado;
        }
    }

    partial void OnWhatsAppChanged(
        string value)
    {
        var formatado = MaskHelper.FormatPhone(
            value);

        if (value != formatado)
        {
            WhatsApp = formatado;
        }
    }

    partial void OnCepChanged(
        string value)
    {
        var numeros = MaskHelper.OnlyNumbers(
            value);

        numeros = numeros[
            ..Math.Min(numeros.Length, 8)];

        var formatado = numeros.Length <= 5
            ? numeros
            : $"{numeros[..5]}-{numeros[5..]}";

        if (value != formatado)
        {
            Cep = formatado;
        }
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        await ExecutarAsync(async () =>
        {
            await CarregarEstadosAsync();
            await CarregarListaInternaAsync();
        });
    }

    [RelayCommand]
    private async Task PesquisarAsync()
    {
        await ExecutarAsync(
            CarregarListaInternaAsync);
    }

    [RelayCommand]
    private async Task ConsultarCepAsync()
    {
        var cepNumeros = MaskHelper.OnlyNumbers(
            Cep);

        if (cepNumeros.Length != 8)
        {
            ExibirMensagem(
                "Informe um CEP com 8 dígitos antes de consultar.",
                true);

            return;
        }

        await ExecutarAsync(async () =>
        {
            var endereco =
                await _cepService.ConsultarAsync(
                    cepNumeros);

            if (endereco is null ||
                !endereco.Encontrado)
            {
                ExibirMensagem(
                    "Não foi possível localizar o CEP. Confira o número ou preencha o endereço manualmente.",
                    true);

                return;
            }

            Logradouro = NormalizarTexto(
                endereco.Logradouro);

            Bairro = NormalizarTexto(
                endereco.Bairro);

            if (string.IsNullOrWhiteSpace(
                    Complemento))
            {
                Complemento = NormalizarTexto(
                    endereco.Complemento);
            }

            Cep = FormatCep(
                endereco.Cep);

            await SelecionarLocalidadeAsync(
                endereco.SiglaUf,
                endereco.CodigoIbgeCidade);

            if (EstadoSelecionado is null ||
                MunicipioSelecionado is null)
            {
                return;
            }

            ExibirMensagem(
                "Endereço localizado. Confira os dados e informe o número do imóvel.",
                false);
        });
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (EstadoSelecionado is null)
        {
            ExibirMensagem(
                "Selecione o estado do cliente.",
                true);

            return;
        }

        if (MunicipioSelecionado is null)
        {
            ExibirMensagem(
                "Selecione um município válido na lista.",
                true);

            return;
        }

        var codigoIbgeMunicipio =
            MaskHelper.OnlyNumbers(
                MunicipioSelecionado.CodigoIbge);

        if (string.IsNullOrWhiteSpace(
                codigoIbgeMunicipio))
        {
            ExibirMensagem(
                "O município selecionado não possui código IBGE.",
                true);

            return;
        }

        var codigoIbgeEstado =
            MaskHelper.OnlyNumbers(
                EstadoSelecionado.CodigoIbge);

        if (string.IsNullOrWhiteSpace(
                codigoIbgeEstado))
        {
            ExibirMensagem(
                "O estado selecionado não possui código IBGE.",
                true);

            return;
        }

        NormalizarFormulario();

        await ExecutarAsync(async () =>
        {
            int? municipioId =
               MunicipioSelecionado.Id > 0
                   ? MunicipioSelecionado.Id
                   : null;

            var resultado =
                await _clienteService.SalvarAsync(
                    new SalvarClienteDto
                    {
                        Id = ClienteId,
                        TipoPessoa = TipoPessoa,
                        NomeRazaoSocial =
                            NomeRazaoSocial,
                        NomeFantasia =
                            NomeFantasia,
                        CpfCnpj =
                            CpfCnpj,
                        RgInscricaoEstadual =
                            RgInscricaoEstadual,
                        Telefone =
                            Telefone,
                        WhatsApp =
                            WhatsApp,
                        Email =
                            Email,
                        Cep =
                            Cep,
                        Logradouro =
                            Logradouro,
                        Numero =
                            Numero,
                        Complemento =
                            Complemento,
                        Bairro =
                            Bairro,
                        Cidade =
                            NormalizarTexto(
                                MunicipioSelecionado.Nome),
                        CodigoIbgeCidade =
                            codigoIbgeMunicipio,
                        Estado =
                            NormalizarTexto(
                                EstadoSelecionado.Nome),
                        SiglaUf =
                            EstadoSelecionado.Sigla
                                .Trim()
                                .ToUpperInvariant(),
                        CodigoIbgeUf =
                            codigoIbgeEstado,
                        MunicipioId =
                            municipioId,
                        PontoReferencia =
                            PontoReferencia,
                        Observacao =
                            Observacao,
                        Ativo =
                            Ativo
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
    private async Task EditarAsync(
        ClienteDto? cliente)
    {
        if (cliente is null)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            ClienteId =
                cliente.Id;

            TipoPessoa =
                cliente.TipoPessoa == "Pessoa Física"
                    ? TipoPessoa.Fisica
                    : TipoPessoa.Juridica;

            NomeRazaoSocial =
                cliente.NomeRazaoSocial;

            NomeFantasia =
                cliente.NomeFantasia;

            CpfCnpj =
                MaskHelper.FormatCpfCnpj(
                    cliente.CpfCnpj);

            RgInscricaoEstadual =
                cliente.RgInscricaoEstadual;

            Telefone =
                MaskHelper.FormatPhone(
                    cliente.Telefone);

            WhatsApp =
                MaskHelper.FormatPhone(
                    cliente.WhatsApp);

            Email =
                cliente.Email;

            Cep =
                FormatCep(cliente.Cep);

            Logradouro =
                cliente.Logradouro;

            Numero =
                cliente.Numero;

            Complemento =
                cliente.Complemento;

            Bairro =
                cliente.Bairro;

            PontoReferencia =
                cliente.PontoReferencia;

            Observacao =
                cliente.Observacao;

            Ativo =
                cliente.Ativo;

            ClienteSelecionado =
                cliente;

            Mensagem =
                string.Empty;

            MensagemEhErro =
                false;

            await SelecionarLocalidadeAsync(
                cliente.SiglaUf,
                cliente.CodigoIbgeCidade);
        });
    }

    [RelayCommand]
    private async Task AlterarStatusAsync(
        ClienteDto? cliente)
    {
        if (cliente is null)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado =
                await _clienteService.AlterarStatusAsync(
                    cliente.Id,
                    !cliente.Ativo);

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
        ClienteDto? cliente)
    {
        if (cliente is null)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado = await _clienteService.ExcluirAsync(
                cliente.Id);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            if (ClienteId == cliente.Id)
            {
                LimparFormulario(preservarMensagem: true);
            }

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

    private async Task CarregarEstadosAsync()
    {
        var estados =
            await _ibgeLocalidadeService
                .ListarEstadosAsync();

        Estados.Clear();

        foreach (var item in estados)
        {
            Estados.Add(item);
        }
    }

    private async Task CarregarMunicipiosAsync(
        string? codigoIbgeEstado,
        bool limparCidade)
    {
        if (limparCidade)
        {
            _atualizandoLocalidade = true;

            try
            {
                MunicipioSelecionado = null;
                Cidade = string.Empty;
                CodigoIbgeCidade = string.Empty;
            }
            finally
            {
                _atualizandoLocalidade = false;
            }
        }

        Municipios.Clear();

        var codigoEstado =
            MaskHelper.OnlyNumbers(
                codigoIbgeEstado);

        if (string.IsNullOrWhiteSpace(
                codigoEstado))
        {
            return;
        }

        try
        {
            var municipios =
                await _ibgeLocalidadeService
                    .ListarMunicipiosAsync(
                        codigoEstado);

            foreach (var item in municipios)
            {
                Municipios.Add(item);
            }

            if (municipios.Count == 0)
            {
                ExibirMensagem(
                    "Não foi possível carregar os municípios desta UF.",
                    true);
            }
        }
        catch (Exception)
        {
            ExibirMensagem(
                "Não foi possível carregar os municípios da UF selecionada.",
                true);
        }
    }

    private async Task SelecionarLocalidadeAsync(
        string? siglaUf,
        string? codigoIbgeCidade)
    {
        if (Estados.Count == 0)
        {
            await CarregarEstadosAsync();
        }

        var siglaNormalizada =
            siglaUf?
                .Trim()
                .ToUpperInvariant();

        var estadoEncontrado =
            Estados.FirstOrDefault(item =>
                string.Equals(
                    item.Sigla?.Trim(),
                    siglaNormalizada,
                    StringComparison.OrdinalIgnoreCase));

        if (estadoEncontrado is null)
        {
            ExibirMensagem(
                "A UF informada não foi localizada na lista de estados.",
                true);

            return;
        }

        _atualizandoLocalidade = true;

        try
        {
            EstadoSelecionado =
                estadoEncontrado;

            SiglaUf =
                estadoEncontrado.Sigla;

            Estado =
                estadoEncontrado.Nome;

            CodigoIbgeUf =
                MaskHelper.OnlyNumbers(
                    estadoEncontrado.CodigoIbge);

            MunicipioSelecionado =
                null;

            Cidade =
                string.Empty;

            CodigoIbgeCidade =
                string.Empty;
        }
        finally
        {
            _atualizandoLocalidade = false;
        }

        await CarregarMunicipiosAsync(
            estadoEncontrado.CodigoIbge,
            limparCidade: false);

        var codigoCidadeNormalizado =
            MaskHelper.OnlyNumbers(
                codigoIbgeCidade);

        var municipioEncontrado =
            Municipios.FirstOrDefault(item =>
                string.Equals(
                    MaskHelper.OnlyNumbers(
                        item.CodigoIbge),
                    codigoCidadeNormalizado,
                    StringComparison.Ordinal));

        if (municipioEncontrado is null)
        {
            ExibirMensagem(
                "O município informado não foi localizado na lista do IBGE.",
                true);

            return;
        }

        _atualizandoLocalidade = true;

        try
        {
            MunicipioSelecionado =
                municipioEncontrado;

            Cidade =
                NormalizarTexto(
                    municipioEncontrado.Nome);

            CodigoIbgeCidade =
                MaskHelper.OnlyNumbers(
                    municipioEncontrado.CodigoIbge);
        }
        finally
        {
            _atualizandoLocalidade = false;
        }
    }

    private async Task CarregarListaInternaAsync()
    {
        bool? ativoFiltro = FiltroStatus switch
        {
            "Ativos" => true,
            "Inativos" => false,
            _ => null
        };

        var clientes =
            await _clienteService.ListarAsync(
                Pesquisa?.Trim(),
                ativoFiltro);

        Clientes.Clear();

        foreach (var item in clientes)
        {
            Clientes.Add(item);
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
        NomeRazaoSocial =
            NormalizarTexto(
                NomeRazaoSocial);

        NomeFantasia =
            NormalizarTexto(
                NomeFantasia);

        RgInscricaoEstadual =
            NormalizarTexto(
                RgInscricaoEstadual);

        Logradouro =
            NormalizarTexto(
                Logradouro);

        Numero =
            NormalizarTexto(
                Numero);

        Complemento =
            NormalizarTexto(
                Complemento);

        Bairro =
            NormalizarTexto(
                Bairro);

        Cidade =
            MunicipioSelecionado is null
                ? NormalizarTexto(Cidade)
                : NormalizarTexto(
                    MunicipioSelecionado.Nome);

        Estado =
            EstadoSelecionado is null
                ? NormalizarTexto(Estado)
                : NormalizarTexto(
                    EstadoSelecionado.Nome);

        SiglaUf =
            EstadoSelecionado is null
                ? NormalizarTexto(SiglaUf)
                : EstadoSelecionado.Sigla
                    .Trim()
                    .ToUpperInvariant();

        CodigoIbgeCidade =
            MunicipioSelecionado is null
                ? MaskHelper.OnlyNumbers(
                    CodigoIbgeCidade)
                : MaskHelper.OnlyNumbers(
                    MunicipioSelecionado.CodigoIbge);

        CodigoIbgeUf =
            EstadoSelecionado is null
                ? MaskHelper.OnlyNumbers(
                    CodigoIbgeUf)
                : MaskHelper.OnlyNumbers(
                    EstadoSelecionado.CodigoIbge);

        PontoReferencia =
            NormalizarTexto(
                PontoReferencia);

        Observacao =
            NormalizarTexto(
                Observacao);

        Email =
            Email
                .Trim()
                .ToLowerInvariant();

        CpfCnpj =
            MaskHelper.OnlyNumbers(
                CpfCnpj);

        Telefone =
            MaskHelper.OnlyNumbers(
                Telefone);

        WhatsApp =
            MaskHelper.OnlyNumbers(
                WhatsApp);

        Cep =
            MaskHelper.OnlyNumbers(
                Cep);
    }

    private void LimparFormulario(
        bool preservarMensagem = false)
    {
        var mensagemAtual =
            Mensagem;

        var erroAtual =
            MensagemEhErro;

        _atualizandoLocalidade = true;

        try
        {
            ClienteId = null;
            TipoPessoa = TipoPessoa.Fisica;
            NomeRazaoSocial = string.Empty;
            NomeFantasia = string.Empty;
            CpfCnpj = string.Empty;
            RgInscricaoEstadual = string.Empty;
            Telefone = string.Empty;
            WhatsApp = string.Empty;
            Email = string.Empty;
            Cep = string.Empty;
            Logradouro = string.Empty;
            Numero = string.Empty;
            Complemento = string.Empty;
            Bairro = string.Empty;
            Cidade = string.Empty;
            CodigoIbgeCidade = string.Empty;
            SiglaUf = string.Empty;
            Estado = string.Empty;
            CodigoIbgeUf = string.Empty;
            EstadoSelecionado = null;
            MunicipioSelecionado = null;
            Municipios.Clear();
            PontoReferencia = string.Empty;
            Observacao = string.Empty;
            Ativo = true;
            ClienteSelecionado = null;
        }
        finally
        {
            _atualizandoLocalidade = false;
        }

        Mensagem = preservarMensagem
            ? mensagemAtual
            : string.Empty;

        MensagemEhErro =
            preservarMensagem &&
            erroAtual;
    }

    private static string NormalizarTexto(
        string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        return string.Join(
                " ",
                valor
                    .Trim()
                    .Split(
                        ' ',
                        StringSplitOptions
                            .RemoveEmptyEntries))
            .ToUpperInvariant();
    }

    private static string FormatCep(
        string? value)
    {
        var numeros =
            MaskHelper.OnlyNumbers(
                value);

        numeros = numeros[
            ..Math.Min(numeros.Length, 8)];

        return numeros.Length <= 5
            ? numeros
            : $"{numeros[..5]}-{numeros[5..]}";
    }

    private void ExibirMensagem(
        string texto,
        bool erro)
    {
        Mensagem = texto;
        MensagemEhErro = erro;
    }
}