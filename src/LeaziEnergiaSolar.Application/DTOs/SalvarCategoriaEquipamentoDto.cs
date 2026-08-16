namespace LeaziEnergiaSolar.Application.DTOs;
public sealed class SalvarCategoriaEquipamentoDto
{
    public int? Id { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public string Observacao { get; init; } = string.Empty;
    public bool Ativo { get; init; } = true;
}
