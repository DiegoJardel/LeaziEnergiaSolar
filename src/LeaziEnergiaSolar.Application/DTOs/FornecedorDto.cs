using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class FornecedorDto
{
    public int Id { get; init; }
    public string Codigo => Id.ToString("D4");
    public TipoPessoa TipoPessoa { get; init; }
    public string NomeRazaoSocial { get; init; } = string.Empty;
    public string NomeFantasia { get; init; } = string.Empty;
    public string CpfCnpj { get; init; } = string.Empty;
    public string Telefone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string ContatoResponsavel { get; init; } = string.Empty;
    public string Observacao { get; init; } = string.Empty;
    public bool Ativo { get; init; }
    public DateTime DataCadastro { get; init; }
    public DateTime? DataAtualizacao { get; init; }
    public string Status => Ativo ? "Ativo" : "Inativo";
}
