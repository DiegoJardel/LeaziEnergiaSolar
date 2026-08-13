namespace LeaziEnergiaSolar.Domain.Entities;

public class Municipio
{
    public int Id { get; set; }

    public string CodigoIbge { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public int EstadoId { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataAtualizacao { get; set; } = DateTime.Now;

    public Estado Estado { get; set; } = null!;

    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
