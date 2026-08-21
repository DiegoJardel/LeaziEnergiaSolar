namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class UnidadeMedidaDto
{
    public int Id { get; init; }
    public string Sigla { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public bool PermiteQuantidadeDecimal { get; init; }
    public bool Ativo { get; init; }
    public DateTime DataCadastro { get; init; }
    public DateTime? DataAtualizacao { get; init; }
    public string Status => Ativo ? "Ativo" : "Inativo";
}
