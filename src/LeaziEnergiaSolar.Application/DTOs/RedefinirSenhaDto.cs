namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class RedefinirSenhaDto
{
    public int UsuarioId { get; init; }

    public string NovaSenha { get; init; } = string.Empty;
}
