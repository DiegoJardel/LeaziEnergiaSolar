using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.Validators;

public static class LancamentoValidator
{
    public static IReadOnlyList<string> Validar(SalvarLancamentoDto lancamento)
    {
        var erros = new List<string>();

        if (lancamento.DataVenda == default)
        {
            erros.Add("Informe a data da venda.");
        }

        if (string.IsNullOrWhiteSpace(lancamento.Cliente))
        {
            erros.Add("Informe o nome do cliente.");
        }
        else if (lancamento.Cliente.Trim().Length < 3)
        {
            erros.Add("O nome do cliente deve possuir no mínimo 3 caracteres.");
        }

        if (!string.IsNullOrWhiteSpace(lancamento.CpfCnpjCliente) &&
            !DocumentoValidator.ValidarCpfCnpj(lancamento.CpfCnpjCliente))
        {
            erros.Add("O CPF ou CNPJ do cliente é inválido.");
        }

        if (lancamento.VendedorId <= 0)
        {
            erros.Add("Selecione um vendedor.");
        }

        if (lancamento.ValorVenda <= 0)
        {
            erros.Add("O valor da venda deve ser maior que zero.");
        }

        if (lancamento.PercentualComissao is <= 0 or > 100)
        {
            erros.Add("A comissão deve ser maior que 0 e menor ou igual a 100%.");
        }

        if (!Enum.IsDefined(typeof(StatusLancamento), lancamento.Status))
        {
            erros.Add("Selecione um status válido.");
        }

        if (lancamento.Observacao.Length > 500)
        {
            erros.Add("A observação deve possuir no máximo 500 caracteres.");
        }

        return erros;
    }
}
