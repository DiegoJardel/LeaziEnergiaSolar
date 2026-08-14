namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class EstadoDto
{
    public string CodigoIbge { get; init; } = string.Empty;

    public string Nome { get; init; } = string.Empty;

    public string Sigla { get; init; } = string.Empty;

    public override string ToString() => Sigla + " - " + Nome;
}
