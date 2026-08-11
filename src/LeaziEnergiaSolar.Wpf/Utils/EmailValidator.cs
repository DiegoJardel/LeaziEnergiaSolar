using System.Net.Mail;
using System.Text.RegularExpressions;

namespace LeaziEnergiaSolar.Wpf.Utils;

public static class EmailValidator
{
    private const int MaximumLength = 150;

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
        RegexOptions.Compiled |
        RegexOptions.CultureInvariant |
        RegexOptions.IgnoreCase);

    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var email = Normalize(value);

        if (email.Length > MaximumLength)
        {
            return false;
        }

        if (!EmailRegex.IsMatch(email))
        {
            return false;
        }

        try
        {
            var address = new MailAddress(email);

            return address.Address.Equals(
                email,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value
            .Trim()
            .ToLowerInvariant();
    }
}