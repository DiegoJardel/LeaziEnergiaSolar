namespace LeaziEnergiaSolar.Domain.Entities;

public class CategoriaEquipamento
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public DateTime? DataAtualizacao { get; set; }
    public ICollection<Equipamento> Equipamentos { get; set; } = new List<Equipamento>();
}
