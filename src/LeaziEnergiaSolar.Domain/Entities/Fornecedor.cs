using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Domain.Entities;

public class Fornecedor
{
    public int Id { get; set; }

    public TipoPessoa TipoPessoa { get; set; } =
        TipoPessoa.Juridica;

    public string NomeRazaoSocial { get; set; } =
        string.Empty;

    public string? NomeFantasia { get; set; }

    public string? CpfCnpj { get; set; }

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public string? ContatoResponsavel { get; set; }

    public string? Observacao { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } =
        DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }

    public ICollection<Equipamento> Equipamentos { get; set; } =
        new List<Equipamento>();
}