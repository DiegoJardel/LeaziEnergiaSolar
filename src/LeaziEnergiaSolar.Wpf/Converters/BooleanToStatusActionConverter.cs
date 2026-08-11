using System.Globalization;
using System.Windows.Data;

namespace LeaziEnergiaSolar.Wpf.Converters;

public sealed class BooleanToStatusActionConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value is true ? "Inativar" : "Ativar";
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
