using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace LeaziEnergiaSolar.Wpf.Utils;

public static class EmailAutoCompleteBehavior
{
    private static readonly string[] DominiosPadrao =
    {
        "gmail.com",
        "hotmail.com",
        "outlook.com",
        "outlook.com.br",
        "yahoo.com",
        "yahoo.com.br",
        "icloud.com",
        "live.com",
        "uol.com.br",
        "bol.com.br"
    };

    private static readonly DependencyProperty EstadoProperty =
        DependencyProperty.RegisterAttached(
            "Estado",
            typeof(EmailAutoCompleteEstado),
            typeof(EmailAutoCompleteBehavior));

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(EmailAutoCompleteBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static bool GetIsEnabled(
        DependencyObject elemento) =>
        (bool)elemento.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(
        DependencyObject elemento,
        bool value) =>
        elemento.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TextBox textBox)
        {
            return;
        }

        if ((bool)args.NewValue)
        {
            Ativar(textBox);
            return;
        }

        Desativar(textBox);
    }

    private static void Ativar(
        TextBox textBox)
    {
        if (textBox.GetValue(EstadoProperty) is EmailAutoCompleteEstado)
        {
            return;
        }

        var sugestoes =
            new ObservableCollection<string>();

        var lista =
            new ListBox
            {
                ItemsSource = sugestoes,
                MaxHeight = 190,
                MinWidth = 220,
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(208, 213, 221)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(4)
            };

        var popup =
            new Popup
            {
                PlacementTarget = textBox,
                Placement = PlacementMode.Bottom,
                StaysOpen = false,
                AllowsTransparency = true,
                Child = new Border
                {
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(6),
                    BorderBrush = new SolidColorBrush(
                        Color.FromRgb(208, 213, 221)),
                    BorderThickness = new Thickness(1),
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        BlurRadius = 10,
                        ShadowDepth = 2,
                        Opacity = 0.18
                    },
                    Child = lista
                }
            };

        lista.Tag = textBox;

        var estado =
            new EmailAutoCompleteEstado(
                sugestoes,
                lista,
                popup);

        textBox.SetValue(
            EstadoProperty,
            estado);

        textBox.TextChanged += TextBox_TextChanged;
        textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
        textBox.LostKeyboardFocus += TextBox_LostKeyboardFocus;
        textBox.Unloaded += TextBox_Unloaded;
        lista.PreviewMouseLeftButtonUp += Lista_PreviewMouseLeftButtonUp;
    }

    private static void Desativar(
        TextBox textBox)
    {
        if (textBox.GetValue(EstadoProperty) is not EmailAutoCompleteEstado estado)
        {
            return;
        }

        estado.Popup.IsOpen = false;

        textBox.TextChanged -= TextBox_TextChanged;
        textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
        textBox.LostKeyboardFocus -= TextBox_LostKeyboardFocus;
        textBox.Unloaded -= TextBox_Unloaded;
        estado.Lista.PreviewMouseLeftButtonUp -= Lista_PreviewMouseLeftButtonUp;

        textBox.ClearValue(
            EstadoProperty);
    }

    private static void TextBox_TextChanged(
        object sender,
        TextChangedEventArgs args)
    {
        if (sender is not TextBox textBox ||
            textBox.GetValue(EstadoProperty) is not EmailAutoCompleteEstado estado)
        {
            return;
        }

        AtualizarSugestoes(
            textBox,
            estado);
    }

    private static void AtualizarSugestoes(
        TextBox textBox,
        EmailAutoCompleteEstado estado)
    {
        var texto =
            textBox.Text?.Trim()
            ?? string.Empty;

        var indiceArroba =
            texto.LastIndexOf('@');

        if (indiceArroba <= 0 ||
            texto.IndexOf('@') != indiceArroba)
        {
            Fechar(estado);
            return;
        }

        var dominioDigitado =
            texto[(indiceArroba + 1)..]
                .Trim()
                .ToLowerInvariant();

        var prefixo =
            texto[..(indiceArroba + 1)];

        var dominios =
            DominiosPadrao
                .Where(dominio =>
                    string.IsNullOrEmpty(dominioDigitado) ||
                    dominio.StartsWith(
                        dominioDigitado,
                        StringComparison.OrdinalIgnoreCase))
                .Where(dominio =>
                    !string.Equals(
                        dominio,
                        dominioDigitado,
                        StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .Select(dominio =>
                    prefixo + dominio)
                .ToList();

        estado.Sugestoes.Clear();

        foreach (var dominio in dominios)
        {
            estado.Sugestoes.Add(
                dominio);
        }

        estado.Lista.SelectedIndex =
            dominios.Count > 0
                ? 0
                : -1;

        estado.Popup.Width =
            Math.Max(
                textBox.ActualWidth,
                220);

        estado.Popup.IsOpen =
            dominios.Count > 0 &&
            textBox.IsKeyboardFocusWithin;
    }

    private static void TextBox_PreviewKeyDown(
        object sender,
        KeyEventArgs args)
    {
        if (sender is not TextBox textBox ||
            textBox.GetValue(EstadoProperty) is not EmailAutoCompleteEstado estado ||
            !estado.Popup.IsOpen)
        {
            return;
        }

        if (args.Key == Key.Down)
        {
            estado.Lista.SelectedIndex =
                Math.Min(
                    estado.Lista.SelectedIndex + 1,
                    estado.Sugestoes.Count - 1);

            estado.Lista.ScrollIntoView(
                estado.Lista.SelectedItem);

            args.Handled = true;
            return;
        }

        if (args.Key == Key.Up)
        {
            estado.Lista.SelectedIndex =
                Math.Max(
                    estado.Lista.SelectedIndex - 1,
                    0);

            estado.Lista.ScrollIntoView(
                estado.Lista.SelectedItem);

            args.Handled = true;
            return;
        }

        if (args.Key is Key.Enter or Key.Tab)
        {
            AplicarSugestao(
                textBox,
                estado);

            args.Handled =
                args.Key == Key.Enter;

            return;
        }

        if (args.Key == Key.Escape)
        {
            Fechar(estado);
            args.Handled = true;
        }
    }

    private static void Lista_PreviewMouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs args)
    {
        if (sender is not ListBox lista ||
            lista.SelectedItem is not string sugestao ||
            lista.Tag is not TextBox textBox ||
            textBox.GetValue(EstadoProperty) is not EmailAutoCompleteEstado estado)
        {
            return;
        }

        textBox.Text = sugestao;
        textBox.CaretIndex = textBox.Text.Length;
        textBox.Focus();
        Fechar(estado);
        args.Handled = true;
    }

    private static void AplicarSugestao(
        TextBox textBox,
        EmailAutoCompleteEstado estado)
    {
        if (estado.Lista.SelectedItem is not string sugestao)
        {
            return;
        }

        textBox.Text = sugestao;
        textBox.CaretIndex = textBox.Text.Length;
        Fechar(estado);
    }

    private static void TextBox_LostKeyboardFocus(
        object sender,
        KeyboardFocusChangedEventArgs args)
    {
        if (sender is not TextBox textBox ||
            textBox.GetValue(EstadoProperty) is not EmailAutoCompleteEstado estado)
        {
            return;
        }

        textBox.Dispatcher.BeginInvoke(
            () =>
            {
                if (!textBox.IsKeyboardFocusWithin &&
                    !estado.Lista.IsKeyboardFocusWithin)
                {
                    Fechar(estado);
                }
            },
            DispatcherPriority.Background);
    }

    private static void TextBox_Unloaded(
        object sender,
        RoutedEventArgs args)
    {
        if (sender is TextBox textBox)
        {
            Desativar(textBox);
        }
    }

    private static void Fechar(
        EmailAutoCompleteEstado estado)
    {
        estado.Popup.IsOpen = false;
        estado.Sugestoes.Clear();
        estado.Lista.SelectedIndex = -1;
    }

    private sealed class EmailAutoCompleteEstado
    {
        public ObservableCollection<string> Sugestoes { get; }
        public ListBox Lista { get; }
        public Popup Popup { get; }

        public EmailAutoCompleteEstado(
            ObservableCollection<string> sugestoes,
            ListBox lista,
            Popup popup)
        {
            Sugestoes = sugestoes;
            Lista = lista;
            Popup = popup;
        }
    }
}
