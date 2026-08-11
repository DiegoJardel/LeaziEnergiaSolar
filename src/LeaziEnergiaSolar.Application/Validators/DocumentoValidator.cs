using System.Text.RegularExpressions;

namespace LeaziEnergiaSolar.Application.Validators;

public static class DocumentoValidator
{
    public static string SomenteNumeros(string? valor)
    {
        return Regex.Replace(valor ?? string.Empty, @"\D", string.Empty);
    }

    public static bool ValidarCpfCnpj(string? valor)
    {
        var documento = SomenteNumeros(valor);

        return documento.Length switch
        {
            11 => ValidarCpf(documento),
            14 => ValidarCnpj(documento),
            _ => false
        };
    }

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Distinct().Count() == 1)
        {
            return false;
        }

        var primeiroDigito = CalcularDigitoCpf(cpf, 9, 10);
        var segundoDigito = CalcularDigitoCpf(cpf, 10, 11);

        return primeiroDigito == cpf[9] - '0' &&
               segundoDigito == cpf[10] - '0';
    }

    private static int CalcularDigitoCpf(
        string cpf,
        int quantidade,
        int pesoInicial)
    {
        var soma = 0;

        for (var indice = 0; indice < quantidade; indice++)
        {
            soma += (cpf[indice] - '0') * (pesoInicial - indice);
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Distinct().Count() == 1)
        {
            return false;
        }

        var primeiroDigito = CalcularDigitoCnpj(cnpj, 12);
        var segundoDigito = CalcularDigitoCnpj(cnpj, 13);

        return primeiroDigito == cnpj[12] - '0' &&
               segundoDigito == cnpj[13] - '0';
    }

    private static int CalcularDigitoCnpj(string cnpj, int quantidade)
    {
        var pesos = quantidade == 12
            ? new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }
            : new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var soma = 0;

        for (var indice = 0; indice < quantidade; indice++)
        {
            soma += (cnpj[indice] - '0') * pesos[indice];
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
