using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Wpf.Converters;

public sealed class StatusLancamentoToBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value is StatusLancamento.Pago
            ? new SolidColorBrush(Color.FromRgb(2, 122, 72))
            : new SolidColorBrush(Color.FromRgb(181, 71, 8));
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
