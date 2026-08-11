using System.Net.Mail;
using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Validators;

public static class VendedorValidator
{
    public static IReadOnlyList<string> Validar(SalvarVendedorDto vendedor)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(vendedor.Nome))
        {
            erros.Add("Informe o nome do vendedor.");
        }
        else if (vendedor.Nome.Trim().Length < 3)
        {
            erros.Add("O nome deve possuir no mínimo 3 caracteres.");
        }

        if (!DocumentoValidator.ValidarCpfCnpj(vendedor.CpfCnpj))
        {
            erros.Add("Informe um CPF ou CNPJ válido.");
        }

        var telefone = DocumentoValidator.SomenteNumeros(vendedor.Telefone);

        if (telefone.Length is < 10 or > 11)
        {
            erros.Add("Informe um telefone com DDD válido.");
        }

        if (!EmailValido(vendedor.Email))
        {
            erros.Add("Informe um e-mail válido.");
        }

        if (vendedor.PercentualComissao is <= 0 or > 100)
        {
            erros.Add("A comissão deve ser maior que 0 e menor ou igual a 100%.");
        }

        return erros;
    }

    private static bool EmailValido(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            return new MailAddress(email.Trim()).Address == email.Trim();
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
