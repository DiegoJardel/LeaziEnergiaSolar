using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Wpf.Services;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class UsuariosViewModel : ObservableObject
{
    private readonly IUsuarioService _usuarioService;
    private readonly IUsuarioSessaoService _sessaoService;

    [ObservableProperty]
    private int? usuarioId;

    [ObservableProperty]
    private string nome = string.Empty;

    [ObservableProperty]
    private string login = string.Empty;

    [ObservableProperty]
    private PerfilUsuario perfilSelecionado = PerfilUsuario.Operador;

    [ObservableProperty]
    private bool ativo = true;

    [ObservableProperty]
    private string pesquisa = string.Empty;

    [ObservableProperty]
    private UsuarioDto? usuarioSelecionado;

    [ObservableProperty]
    private string mensagem = string.Empty;

    [ObservableProperty]
    private bool mensagemEhErro;

    [ObservableProperty]
    private bool estaCarregando;

    public ObservableCollection<UsuarioDto> Usuarios { get; } = new();

    public IReadOnlyList<PerfilUsuario> PerfisDisponiveis { get; } =
        Enum.GetValues<PerfilUsuario>();

    public bool EstaEditando => UsuarioId.HasValue;

    public string TituloFormulario => EstaEditando
        ? "Editar usuário"
        : "Novo usuário";

    public UsuariosViewModel(
        IUsuarioService usuarioService,
        IUsuarioSessaoService sessaoService)
    {
        _usuarioService = usuarioService;
        _sessaoService = sessaoService;
    }

    partial void OnUsuarioIdChanged(int? value)
    {
        OnPropertyChanged(nameof(EstaEditando));
        OnPropertyChanged(nameof(TituloFormulario));
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        await ExecutarAsync(CarregarListaAsync);
    }

    [RelayCommand]
    private async Task PesquisarAsync()
    {
        await ExecutarAsync(CarregarListaAsync);
    }

    public async Task SalvarAsync(string senha)
    {
        await ExecutarAsync(async () =>
        {
            var resultado = await _usuarioService.SalvarAsync(
                ObterUsuarioLogadoId(),
                new SalvarUsuarioDto
                {
                    Id = UsuarioId,
                    Nome = Nome.Trim().ToUpperInvariant(),
                    Login = Login.Trim(),
                    Senha = senha,
                    Perfil = PerfilSelecionado,
                    Ativo = Ativo
                });

            ExibirMensagem(resultado.Mensagem, !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            LimparFormulario(preservarMensagem: true);
            await CarregarListaAsync();
        });
    }

    public async Task RedefinirSenhaAsync(
        UsuarioDto usuario,
        string novaSenha)
    {
        await ExecutarAsync(async () =>
        {
            var resultado = await _usuarioService.RedefinirSenhaAsync(
                ObterUsuarioLogadoId(),
                new RedefinirSenhaDto
                {
                    UsuarioId = usuario.Id,
                    NovaSenha = novaSenha
                });

            ExibirMensagem(resultado.Mensagem, !resultado.Sucesso);
        });
    }

    [RelayCommand]
    private void Editar(UsuarioDto? usuario)
    {
        if (usuario is null)
        {
            return;
        }

        UsuarioId = usuario.Id;
        Nome = usuario.Nome;
        Login = usuario.Login;
        PerfilSelecionado = usuario.Perfil;
        Ativo = usuario.Ativo;
        UsuarioSelecionado = usuario;
        Mensagem = string.Empty;
    }

    [RelayCommand]
    private async Task AlterarStatusAsync(UsuarioDto? usuario)
    {
        if (usuario is null)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado = await _usuarioService.AlterarStatusAsync(
                ObterUsuarioLogadoId(),
                usuario.Id,
                !usuario.Ativo);

            ExibirMensagem(resultado.Mensagem, !resultado.Sucesso);

            if (resultado.Sucesso)
            {
                LimparFormulario(preservarMensagem: true);
                await CarregarListaAsync();
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

    private async Task CarregarListaAsync()
    {
        var usuarios = await _usuarioService.ListarAsync(
            ObterUsuarioLogadoId(),
            Pesquisa);

        Usuarios.Clear();

        foreach (var usuario in usuarios)
        {
            Usuarios.Add(usuario);
        }
    }

    private int ObterUsuarioLogadoId()
    {
        return _sessaoService.UsuarioAtual?.Id ??
            throw new UnauthorizedAccessException(
                "Sessão do usuário não encontrada.");
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
        catch (UnauthorizedAccessException exception)
        {
            ExibirMensagem(exception.Message, true);
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
        UsuarioId = null;
        Nome = string.Empty;
        Login = string.Empty;
        PerfilSelecionado = PerfilUsuario.Operador;
        Ativo = true;
        UsuarioSelecionado = null;

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