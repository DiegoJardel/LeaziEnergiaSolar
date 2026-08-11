using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class UsuarioAutenticadoDto
{
    public int Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Login { get; init; } = string.Empty;

    public PerfilUsuario Perfil { get; init; }
}
