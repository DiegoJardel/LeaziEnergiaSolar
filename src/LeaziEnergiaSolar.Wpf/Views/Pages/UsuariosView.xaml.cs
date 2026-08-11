using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Wpf.ViewModels;

namespace LeaziEnergiaSolar.Wpf.Views.Pages;

public partial class UsuariosView : UserControl
{
    private readonly UsuariosViewModel _viewModel;
    private bool _carregado;

    public UsuariosView(UsuariosViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void UsuariosView_Loaded(
        object sender,
        RoutedEventArgs eventArgs)
    {
        if (_carregado)
        {
            return;
        }

        _carregado = true;
        await _viewModel.CarregarCommand.ExecuteAsync(null);
    }

    private async void Salvar_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        await _viewModel.SalvarAsync(SenhaPasswordBox.Password);
        SenhaPasswordBox.Clear();
    }

    private async void RedefinirSenha_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        if (sender is not Button button ||
            button.DataContext is not UsuarioDto usuario)
        {
            return;
        }

        var dialog = new Window
        {
            Title = "Redefinir senha",
            Width = 390,
            Height = 230,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Window.GetWindow(this),
            ResizeMode = ResizeMode.NoResize,
            Background = System.Windows.Media.Brushes.White
        };

        var senha = new PasswordBox
        {
            Margin = new Thickness(0, 12, 0, 18),
            Height = 38
        };

        var confirmar = new Button
        {
            Content = "REDEFINIR",
            Width = 110,
            Height = 38,
            IsDefault = true
        };

        confirmar.Click += (_, _) =>
        {
            dialog.DialogResult = true;
            dialog.Close();
        };

        dialog.Content = new StackPanel
        {
            Margin = new Thickness(28),
            Children =
            {
                new TextBlock
                {
                    Text = $"Nova senha para {usuario.Nome}",
                    FontSize = 18,
                    FontWeight = FontWeights.SemiBold
                },
                new TextBlock
                {
                    Text = "Use ao menos 8 caracteres, com letra maiúscula, minúscula e número.",
                    Margin = new Thickness(0, 5, 0, 0),
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = System.Windows.Media.Brushes.DimGray
                },
                senha,
                confirmar
            }
        };

        if (dialog.ShowDialog() == true)
        {
            await _viewModel.RedefinirSenhaAsync(
                usuario,
                senha.Password);
        }
    }
}
