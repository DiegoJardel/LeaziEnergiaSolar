using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeaziEnergiaSolar.Wpf.Utils;

public static class CurrencyInputBehavior
{
    private static readonly CultureInfo CulturaBrasileira =
        CultureInfo.GetCultureInfo("pt-BR");

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(CurrencyInputBehavior),
            new PropertyMetadata(
                false,
                OnIsEnabledChanged));

    private static bool _estaFormatando;

    public static bool GetIsEnabled(
        DependencyObject element)
    {
        return (bool)element.GetValue(IsEnabledProperty);
    }

    public static void SetIsEnabled(
        DependencyObject element,
        bool value)
    {
        element.SetValue(
            IsEnabledProperty,
            value);
    }

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs eventArgs)
    {
        if (dependencyObject is not TextBox textBox)
        {
            return;
        }

        if ((bool)eventArgs.NewValue)
        {
            textBox.PreviewTextInput += TextBox_PreviewTextInput;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.TextChanged += TextBox_TextChanged;
            DataObject.AddPastingHandler(
                textBox,
                TextBox_Pasting);
        }
        else
        {
            textBox.PreviewTextInput -= TextBox_PreviewTextInput;
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.TextChanged -= TextBox_TextChanged;
            DataObject.RemovePastingHandler(
                textBox,
                TextBox_Pasting);
        }
    }

    private static void TextBox_PreviewTextInput(
        object sender,
        TextCompositionEventArgs eventArgs)
    {
        eventArgs.Handled = eventArgs.Text.Any(
            character => !char.IsDigit(character));
    }

    private static void TextBox_PreviewKeyDown(
        object sender,
        KeyEventArgs eventArgs)
    {
        if (eventArgs.Key == Key.Space)
        {
            eventArgs.Handled = true;
        }
    }

    private static void TextBox_TextChanged(
        object sender,
        TextChangedEventArgs eventArgs)
    {
        if (_estaFormatando ||
            sender is not TextBox textBox)
        {
            return;
        }

        try
        {
            _estaFormatando = true;

            var numeros = ApenasNumeros(textBox.Text);

            if (string.IsNullOrWhiteSpace(numeros))
            {
                textBox.Text = string.Empty;
                return;
            }

            if (!decimal.TryParse(
                    numeros,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var valorInteiro))
            {
                textBox.Text = string.Empty;
                return;
            }

            var valor = valorInteiro / 100m;

            textBox.Text = valor.ToString(
                "C2",
                CulturaBrasileira);

            textBox.CaretIndex = textBox.Text.Length;
        }
        finally
        {
            _estaFormatando = false;
        }
    }

    private static void TextBox_Pasting(
        object sender,
        DataObjectPastingEventArgs eventArgs)
    {
        if (!eventArgs.DataObject.GetDataPresent(
                DataFormats.Text))
        {
            eventArgs.CancelCommand();
            return;
        }

        var texto = eventArgs.DataObject.GetData(
            DataFormats.Text) as string;

        if (string.IsNullOrWhiteSpace(texto))
        {
            eventArgs.CancelCommand();
            return;
        }

        var numeros = ApenasNumeros(texto);

        if (string.IsNullOrWhiteSpace(numeros))
        {
            eventArgs.CancelCommand();
        }
    }

    private static string ApenasNumeros(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return new string(
            value
                .Where(char.IsDigit)
                .ToArray());
    }
}