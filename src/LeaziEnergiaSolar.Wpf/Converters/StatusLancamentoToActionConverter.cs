using System.Globalization;
using System.Windows.Data;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Wpf.Converters;

public sealed class StatusLancamentoToActionConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value is StatusLancamento.Pago
            ? "Pendente"
            : "Pagar";
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
