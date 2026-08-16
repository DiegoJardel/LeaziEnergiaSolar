using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class SalvarFornecedorDto
{
    public int? Id { get; init; }
    public TipoPessoa TipoPessoa { get; init; } = TipoPessoa.Juridica;
    public string NomeRazaoSocial { get; init; } = string.Empty;
    public string NomeFantasia { get; init; } = string.Empty;
    public string CpfCnpj { get; init; } = string.Empty;
    public string Telefone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string ContatoResponsavel { get; init; } = string.Empty;
    public string Observacao { get; init; } = string.Empty;
    public bool Ativo { get; init; } = true;
}
