namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class EquipamentoDto
{
    public int Id { get; init; }
    public string Codigo => Id.ToString("D4");
    public string Descricao { get; init; } = string.Empty;
    public int CategoriaEquipamentoId { get; init; }
    public string Categoria { get; init; } = string.Empty;
    public int? MarcaId { get; init; }
    public string Marca { get; init; } = string.Empty;
    public string Modelo { get; init; } = string.Empty;
    public int UnidadeMedidaId { get; init; }
    public string UnidadeMedida { get; init; } = string.Empty;
    public decimal ValorCusto { get; init; }
    public decimal EstoqueMinimo { get; init; }
    public string Observacao { get; init; } = string.Empty;
    public bool Ativo { get; init; }
    public DateTime DataCadastro { get; init; }
    public DateTime? DataAtualizacao { get; init; }
    public string Status => Ativo ? "Ativo" : "Inativo";
}
