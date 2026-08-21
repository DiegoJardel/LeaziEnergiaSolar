namespace LeaziEnergiaSolar.Application.DTOs;
public sealed class SalvarUnidadeMedidaDto
{
    public int? Id { get; init; }
    public string Sigla { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public bool PermiteQuantidadeDecimal { get; init; }
    public bool Ativo { get; init; } = true;
}
