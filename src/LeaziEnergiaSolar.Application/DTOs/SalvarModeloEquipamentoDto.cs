namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class SalvarModeloEquipamentoDto
{
    public int? Id { get; init; }

    public int MarcaId { get; init; }

    public string Nome { get; init; } =
        string.Empty;

    public string Observacao { get; init; } =
        string.Empty;

    public bool Ativo { get; init; } = true;
}