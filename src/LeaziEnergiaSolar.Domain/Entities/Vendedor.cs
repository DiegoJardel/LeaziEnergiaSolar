namespace LeaziEnergiaSolar.Domain.Entities;

public class Vendedor
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? CpfCnpj { get; set; }

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public decimal PercentualComissao { get; set; } = 5m;

    public bool Ativo { get; set; } = true;

    public ICollection<Lancamento> Lancamentos { get; set; }
        = new List<Lancamento>();
}