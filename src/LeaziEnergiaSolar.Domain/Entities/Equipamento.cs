namespace LeaziEnergiaSolar.Domain.Entities;

public class Equipamento
{
    public int Id { get; set; }
    public int CategoriaEquipamentoId { get; set; }
    public CategoriaEquipamento CategoriaEquipamento { get; set; } = null!;
    public int? MarcaId { get; set; }
    public Marca? Marca { get; set; }
    public string? Modelo { get; set; }
    public int UnidadeMedidaId { get; set; }
    public UnidadeMedida UnidadeMedida { get; set; } = null!;
    public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public DateTime? DataAtualizacao { get; set; }
}
