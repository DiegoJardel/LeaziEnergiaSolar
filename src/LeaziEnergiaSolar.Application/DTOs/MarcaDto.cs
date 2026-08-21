namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class MarcaDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Observacao { get; init; } = string.Empty;
    public bool Ativo { get; init; }
    public DateTime DataCadastro { get; init; }
    public DateTime? DataAtualizacao { get; init; }
    public string Status => Ativo ? "Ativo" : "Inativo";
}
