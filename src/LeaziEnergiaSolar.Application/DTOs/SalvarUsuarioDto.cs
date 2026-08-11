using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class SalvarUsuarioDto
{
    public int? Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Login { get; init; } = string.Empty;

    public string Senha { get; init; } = string.Empty;

    public PerfilUsuario Perfil { get; init; }

    public bool Ativo { get; init; }
}
