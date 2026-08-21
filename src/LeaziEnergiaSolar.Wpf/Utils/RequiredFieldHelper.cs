using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
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

    public static bool GetIsRequired(
        DependencyObject element)
    {
        return (bool)element.GetValue(
            IsRequiredProperty);
    }

    public static void SetIsRequired(
        DependencyObject element,
        bool value)
    {
        element.SetValue(
            IsRequiredProperty,
            value);
    }

    private static void OnIsRequiredChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs eventArgs)
    {
        if (dependencyObject is not Control control)
        {
            return;
        }

        if (eventArgs.NewValue is not bool isRequired ||
            !isRequired)
        {
            return;
        }

        if (control.IsLoaded)
        {
            AplicarIndicador(control);
            return;
        }

        control.Loaded += Control_Loaded;
    }

    private static void Control_Loaded(
        object sender,
        RoutedEventArgs eventArgs)
    {
        if (sender is not Control control)
        {
            return;
        }

        control.Loaded -= Control_Loaded;

        AplicarIndicador(control);
    }

    private static void AplicarIndicador(
        Control control)
    {
        var hintAtual = HintAssist.GetHint(control);

        if (hintAtual is not string hint ||
            string.IsNullOrWhiteSpace(hint))
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
                    Color.FromRgb(
                        198,
                        40,
                        40)),
                FontWeight = FontWeights.SemiBold
            });

        HintAssist.SetHint(
            control,
            texto);
    }
}