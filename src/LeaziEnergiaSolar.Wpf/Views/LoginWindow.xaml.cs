using System.Windows;
using System.Windows.Input;
using LeaziEnergiaSolar.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LeaziEnergiaSolar.Wpf;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void Entrar_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        await EntrarAsync();
    }

    private async void SenhaPasswordBox_KeyDown(
        object sender,
        KeyEventArgs eventArgs)
    {
        if (eventArgs.Key != Key.Enter)
        {
            return;
        }

        await EntrarAsync();
    }

    private async Task EntrarAsync()
    {
        var autenticado = await _viewModel.AutenticarAsync(
            SenhaPasswordBox.Password);

        SenhaPasswordBox.Clear();

        if (!autenticado)
        {
            SenhaPasswordBox.Focus();
            return;
        }

        var mainWindow = App.Services
            .GetRequiredService<MainWindow>();

        mainWindow.Show();
        Close();
    }
}
