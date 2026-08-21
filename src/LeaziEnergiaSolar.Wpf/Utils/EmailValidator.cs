using ApplicationEmailValidator = LeaziEnergiaSolar.Application.Validators.EmailValidator;

namespace LeaziEnergiaSolar.Wpf.Utils;

public static class EmailValidator
{
    public static bool IsValid(string? value) =>
        ApplicationEmailValidator.IsValid(value);

    public static string Normalize(string? value) =>
        ApplicationEmailValidator.Normalize(value);
}
