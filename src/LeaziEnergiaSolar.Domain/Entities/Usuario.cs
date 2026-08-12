using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Login { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Operador;

    public bool Ativo { get; set; } = true;
}