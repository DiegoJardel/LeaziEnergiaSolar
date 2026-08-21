namespace LeaziEnergiaSolar.Domain.Entities;

public class ModeloEquipamento
{
    public int Id { get; set; }

    public int MarcaId { get; set; }

    public Marca Marca { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;

    public string? Observacao { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } =
        DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }
}