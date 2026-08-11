using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Wpf.Services;

public interface IUsuarioSessaoService
{
    UsuarioAutenticadoDto? UsuarioAtual { get; }

    bool EstaAutenticado { get; }

    void Iniciar(UsuarioAutenticadoDto usuario);

    void Encerrar();
}
