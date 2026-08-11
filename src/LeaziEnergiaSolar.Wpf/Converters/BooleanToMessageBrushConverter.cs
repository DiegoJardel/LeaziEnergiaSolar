using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LeaziEnergiaSolar.Wpf.Converters;

public sealed class BooleanToMessageBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value is true
            ? new SolidColorBrush(Color.FromRgb(217, 45, 32))
            : new SolidColorBrush(Color.FromRgb(2, 122, 72));
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
