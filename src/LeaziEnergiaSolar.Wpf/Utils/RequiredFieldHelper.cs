using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

namespace LeaziEnergiaSolar.Wpf.Utils;

public static class RequiredFieldHelper
{
    public static readonly DependencyProperty IsRequiredProperty =
        DependencyProperty.RegisterAttached(
            "IsRequired",
            typeof(bool),
            typeof(RequiredFieldHelper),
            new PropertyMetadata(
                false,
                OnIsRequiredChanged));

    public static void SetIsRequired(
        DependencyObject element,
        bool value) =>
        element.SetValue(
            IsRequiredProperty,
            value);

    public static bool GetIsRequired(
        DependencyObject element) =>
        (bool)element.GetValue(IsRequiredProperty);

    private static void OnIsRequiredChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not Control control ||
            e.NewValue is not true)
        {
            return;
        }

        control.Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded,
            new Action(() => AplicarIndicador(control)));
    }

    private static void AplicarIndicador(Control control)
    {
        if (HintAssist.GetHint(control) is not string hint ||
            string.IsNullOrWhiteSpace(hint) ||
            hint.EndsWith(" *", StringComparison.Ordinal))
        {
            return;
        }

        var texto = new TextBlock();
        texto.Inlines.Add(
            new Run(hint));
        texto.Inlines.Add(
            new Run(" *")
            {
                Foreground = new SolidColorBrush(
                    Color.FromRgb(198, 40, 40)),
                FontWeight = FontWeights.SemiBold
            });

        HintAssist.SetHint(
            control,
            texto);
    }
}
