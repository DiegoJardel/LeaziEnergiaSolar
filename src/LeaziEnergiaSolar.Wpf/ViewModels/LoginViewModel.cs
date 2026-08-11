using CommunityToolkit.Mvvm.ComponentModel;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Wpf.Services;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAutenticacaoService _autenticacaoService;
    private readonly IUsuarioSessaoService _usuarioSessaoService;

    [ObservableProperty]
    private string login = string.Empty;

    [ObservableProperty]
    private string mensagemErro = string.Empty;

    [ObservableProperty]
    private bool estaCarregando;

    public LoginViewModel(
        IAutenticacaoService autenticacaoService,
        IUsuarioSessaoService usuarioSessaoService)
    {
        _autenticacaoService = autenticacaoService;
        _usuarioSessaoService = usuarioSessaoService;
    }

    public async Task<bool> AutenticarAsync(
        string senha,
        CancellationToken cancellationToken = default)
    {
        if (EstaCarregando)
        {
            return false;
        }

        try
        {
            EstaCarregando = true;
            MensagemErro = string.Empty;

            var resultado = await _autenticacaoService.AutenticarAsync(
                Login,
                senha,
                cancellationToken);

            if (!resultado.Sucesso || resultado.Usuario is null)
            {
                MensagemErro = resultado.Mensagem;
                return false;
            }

            _usuarioSessaoService.Iniciar(resultado.Usuario);
            return true;
        }
        finally
        {
            EstaCarregando = false;
        }
    }
}
