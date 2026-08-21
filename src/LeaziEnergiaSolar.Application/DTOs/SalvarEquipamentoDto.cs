namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class SalvarEquipamentoDto
{
    public int? Id { get; init; }

    public int CategoriaEquipamentoId { get; init; }

    public int MarcaId { get; init; }

    public string Modelo { get; init; } =
        string.Empty;

    public int UnidadeMedidaId { get; init; }

    public int FornecedorId { get; init; }

    public string Observacao { get; init; } =
        string.Empty;

    public bool Ativo { get; init; } = true;
}