namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class EnderecoCepDto
{
    public string Cep { get; init; } = string.Empty;

    public string Logradouro { get; init; } = string.Empty;

    public string Complemento { get; init; } = string.Empty;

    public string Bairro { get; init; } = string.Empty;

    public string Cidade { get; init; } = string.Empty;

    public string CodigoIbgeCidade { get; init; } = string.Empty;

    public string SiglaUf { get; init; } = string.Empty;

    public bool Encontrado { get; init; }
}
