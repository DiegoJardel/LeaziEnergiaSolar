namespace LeaziEnergiaSolar.Wpf.Utils;

public static class DocumentValidator
{
    public static bool IsValidCpfCnpj(string? value)
    {
        var numbers = MaskHelper.OnlyNumbers(value);

        return numbers.Length switch
        {
            11 => IsValidCpf(numbers),
            14 => IsValidCnpj(numbers),
            _ => false
        };
    }

    private static bool IsValidCpf(string cpf)
    {
        if (HasAllEqualDigits(cpf))
        {
            return false;
        }

        var firstDigit = CalculateCpfDigit(
            cpf,
            length: 9,
            initialWeight: 10);

        var secondDigit = CalculateCpfDigit(
            cpf,
            length: 10,
            initialWeight: 11);

        return firstDigit == cpf[9] - '0' &&
               secondDigit == cpf[10] - '0';
    }

    private static int CalculateCpfDigit(
        string cpf,
        int length,
        int initialWeight)
    {
        var sum = 0;
        var weight = initialWeight;

        for (var index = 0; index < length; index++)
        {
            var digit = cpf[index] - '0';

            sum += digit * weight;
            weight--;
        }

        var remainder = sum % 11;

        return remainder < 2
            ? 0
            : 11 - remainder;
    }

    private static bool IsValidCnpj(string cnpj)
    {
        if (HasAllEqualDigits(cnpj))
        {
            return false;
        }

        var firstDigit = CalculateCnpjDigit(
            cnpj,
            length: 12,
            initialWeight: 5);

        var secondDigit = CalculateCnpjDigit(
            cnpj,
            length: 13,
            initialWeight: 6);

        return firstDigit == cnpj[12] - '0' &&
               secondDigit == cnpj[13] - '0';
    }

    private static int CalculateCnpjDigit(
        string cnpj,
        int length,
        int initialWeight)
    {
        var sum = 0;
        var weight = initialWeight;

        for (var index = 0; index < length; index++)
        {
            var digit = cnpj[index] - '0';

            sum += digit * weight;
            weight--;

            if (weight == 1)
            {
                weight = 9;
            }
        }

        var remainder = sum % 11;

        return remainder < 2
            ? 0
            : 11 - remainder;
    }

    private static bool HasAllEqualDigits(string value)
    {
        return value.Distinct().Count() == 1;
    }
}