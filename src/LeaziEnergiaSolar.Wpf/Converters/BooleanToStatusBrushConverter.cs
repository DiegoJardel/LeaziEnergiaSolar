using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LeaziEnergiaSolar.Wpf.Converters;

public sealed class BooleanToStatusBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value is true
            ? new SolidColorBrush(Color.FromRgb(2, 122, 72))
            : new SolidColorBrush(Color.FromRgb(217, 45, 32));
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
