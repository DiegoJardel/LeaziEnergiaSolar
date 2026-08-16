using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class SalvarClienteDto
{
    public int? Id { get; init; }

    public TipoPessoa TipoPessoa { get; init; }

    public string NomeRazaoSocial { get; init; } = string.Empty;

    public string NomeFantasia { get; init; } = string.Empty;

    public string CpfCnpj { get; init; } = string.Empty;

    public string RgInscricaoEstadual { get; init; } = string.Empty;

    public string Telefone { get; init; } = string.Empty;

    public string WhatsApp { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Cep { get; init; } = string.Empty;

    public string Logradouro { get; init; } = string.Empty;

    public string Numero { get; init; } = string.Empty;

    public string Complemento { get; init; } = string.Empty;

    public string Bairro { get; init; } = string.Empty;

    public string Cidade { get; init; } = string.Empty;

    public string CodigoIbgeCidade { get; init; } = string.Empty;

    public string Estado { get; init; } = string.Empty;

    public string SiglaUf { get; init; } = string.Empty;

    public string CodigoIbgeUf { get; init; } = string.Empty;

    public int? MunicipioId { get; init; }

    public string PontoReferencia { get; init; } = string.Empty;

    public string Observacao { get; init; } = string.Empty;

    public bool Ativo { get; init; } = true;
}
