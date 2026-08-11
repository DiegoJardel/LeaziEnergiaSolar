using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Wpf.Services;

public sealed class UsuarioSessaoService : IUsuarioSessaoService
{
    public UsuarioAutenticadoDto? UsuarioAtual { get; private set; }

    public bool EstaAutenticado => UsuarioAtual is not null;

    public void Iniciar(UsuarioAutenticadoDto usuario)
    {
        UsuarioAtual = usuario ??
            throw new ArgumentNullException(nameof(usuario));
    }

    public void Encerrar()
    {
        UsuarioAtual = null;
    }
}
