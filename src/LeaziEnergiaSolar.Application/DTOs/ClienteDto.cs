namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class ClienteDto
{
    public int Id { get; init; }

    public string TipoPessoa { get; init; } = string.Empty;

    public string NomeRazaoSocial { get; init; } = string.Empty;

    public string NomeFantasia { get; init; } = string.Empty;

    public string CpfCnpj { get; init; } = string.Empty;

    public string RgInscricaoEstadual { get; init; } = string.Empty;

    public DateTime? DataNascimentoAbertura { get; init; }

    public string Telefone { get; init; } = string.Empty;

    public string WhatsApp { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Cep { get; init; } = string.Empty;

    public string Logradouro { get; init; } = string.Empty;

    public string Numero { get; init; } = string.Empty;

    public string Complemento { get; init; } = string.Empty;

    public string Bairro { get; init; } = string.Empty;

    public string Cidade { get; init; } = string.Empty;

    public string SiglaUf { get; init; } = string.Empty;

    public string CodigoIbgeCidade { get; init; } = string.Empty;

    public string CodigoIbgeUf { get; init; } = string.Empty;

    public int? MunicipioId { get; init; }

    public string PontoReferencia { get; init; } = string.Empty;

    public string Observacao { get; init; } = string.Empty;

    public string EnderecoCompleto { get; init; } = string.Empty;

    public string CidadeUf { get; init; } = string.Empty;

    public bool Ativo { get; init; }

    public string Status => Ativo ? "Ativo" : "Inativo";
}
