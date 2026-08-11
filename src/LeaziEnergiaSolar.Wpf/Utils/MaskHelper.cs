using System.Text.RegularExpressions;

namespace LeaziEnergiaSolar.Wpf.Utils;

public static class MaskHelper
{
    public static string OnlyNumbers(string? value)
    {
        return Regex.Replace(value ?? string.Empty, @"\D", string.Empty);
    }

    public static string FormatCpfCnpj(string? value)
    {
        var numbers = OnlyNumbers(value);

        if (numbers.Length <= 11)
        {
            numbers = numbers[..Math.Min(numbers.Length, 11)];

            return numbers.Length switch
            {
                <= 3 => numbers,
                <= 6 => $"{numbers[..3]}.{numbers[3..]}",
                <= 9 => $"{numbers[..3]}.{numbers[3..6]}.{numbers[6..]}",
                _ => $"{numbers[..3]}.{numbers[3..6]}.{numbers[6..9]}-{numbers[9..]}"
            };
        }

        numbers = numbers[..Math.Min(numbers.Length, 14)];

        return numbers.Length switch
        {
            <= 2 => numbers,
            <= 5 => $"{numbers[..2]}.{numbers[2..]}",
            <= 8 => $"{numbers[..2]}.{numbers[2..5]}.{numbers[5..]}",
            <= 12 => $"{numbers[..2]}.{numbers[2..5]}.{numbers[5..8]}/{numbers[8..]}",
            _ => $"{numbers[..2]}.{numbers[2..5]}.{numbers[5..8]}/{numbers[8..12]}-{numbers[12..]}"
        };
    }

    public static string FormatPhone(string? value)
    {
        var numbers = OnlyNumbers(value);
        numbers = numbers[..Math.Min(numbers.Length, 11)];

        return numbers.Length switch
        {
            <= 2 => numbers,
            <= 6 => $"({numbers[..2]}) {numbers[2..]}",
            <= 10 => $"({numbers[..2]}) {numbers[2..6]}-{numbers[6..]}",
            _ => $"({numbers[..2]}) {numbers[2..7]}-{numbers[7..]}"
        };
    }
}
