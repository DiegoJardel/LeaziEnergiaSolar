namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class MunicipioDto
{
    public string CodigoIbge { get; init; } = string.Empty;

    public string Nome { get; init; } = string.Empty;

    public string CodigoIbgeEstado { get; init; } = string.Empty;
    public int Id { get; set; }

    public override string ToString() => Nome;
}
