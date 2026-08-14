using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }

    public TipoPessoa TipoPessoa { get; set; }

    public string NomeRazaoSocial { get; set; } = string.Empty;

    public string? NomeFantasia { get; set; }

    public string? CpfCnpj { get; set; }

    public string? RgInscricaoEstadual { get; set; }

    public DateTime? DataNascimentoAbertura { get; set; }

    public string? Telefone { get; set; }

    public string? WhatsApp { get; set; }

    public string? Email { get; set; }

    public string? Cep { get; set; }

    public string? Logradouro { get; set; }

    public string? Numero { get; set; }

    public string? Complemento { get; set; }

    public string? Bairro { get; set; }

    public string? Cidade { get; set; }

    public string? CodigoIbgeCidade { get; set; }

    public string? Estado { get; set; }

    public string? SiglaUf { get; set; }

    public string? CodigoIbgeUf { get; set; }

    public int? MunicipioId { get; set; }

    public Municipio? Municipio { get; set; }

    public string? PontoReferencia { get; set; }

    public string? Observacao { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.Now;

    public DateTime? DataAlteracao { get; set; }

    public ICollection<Lancamento> Lancamentos { get; set; } = new List<Lancamento>();
}
