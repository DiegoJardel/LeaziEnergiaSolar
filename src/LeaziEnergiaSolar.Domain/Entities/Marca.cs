namespace LeaziEnergiaSolar.Domain.Entities;

public class Marca
{
    public int Id { get; set; }

    public string Nome { get; set; } =
        string.Empty;

    public string? Observacao { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } =
        DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }

    public ICollection<ModeloEquipamento> Modelos { get; set; } =
        new List<ModeloEquipamento>();

    public ICollection<Equipamento> Equipamentos { get; set; } =
        new List<Equipamento>();
}