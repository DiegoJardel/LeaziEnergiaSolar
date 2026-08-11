namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class VendedorDto
{
    public int Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string CpfCnpj { get; init; } = string.Empty;

    public string Telefone { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public decimal PercentualComissao { get; init; }

    public bool Ativo { get; init; }

    public string Status => Ativo ? "Ativo" : "Inativo";
}
