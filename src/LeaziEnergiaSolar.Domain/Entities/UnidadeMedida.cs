namespace LeaziEnergiaSolar.Domain.Entities;

public class UnidadeMedida
{
    public int Id { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool PermiteQuantidadeDecimal { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public DateTime? DataAtualizacao { get; set; }
    public ICollection<Equipamento> Equipamentos { get; set; } = new List<Equipamento>();
}
