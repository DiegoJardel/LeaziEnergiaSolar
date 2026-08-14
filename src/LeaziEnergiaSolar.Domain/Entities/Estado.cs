namespace LeaziEnergiaSolar.Domain.Entities;

public class Estado
{
    public int Id { get; set; }

    public string CodigoIbge { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Sigla { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public DateTime DataAtualizacao { get; set; } = DateTime.Now;

    public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}
