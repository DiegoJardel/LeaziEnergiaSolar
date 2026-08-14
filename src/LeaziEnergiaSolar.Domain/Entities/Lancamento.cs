using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Domain.Entities;

public class Lancamento
{
    public int Id { get; set; }

    public DateTime DataVenda { get; set; } = DateTime.Today;

    public string Cliente { get; set; } = string.Empty;

    public string? CpfCnpjCliente { get; set; }

    public int? ClienteId { get; set; }

    public Cliente? ClienteCadastro { get; set; }

    public int VendedorId { get; set; }

    public Vendedor Vendedor { get; set; } = null!;

    public int? UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }

    public decimal ValorVenda { get; set; }

    public decimal PercentualComissao { get; set; }

    public decimal ValorComissao { get; set; }

    public StatusLancamento Status { get; set; }
        = StatusLancamento.Pendente;

    public string? Observacao { get; set; }
}