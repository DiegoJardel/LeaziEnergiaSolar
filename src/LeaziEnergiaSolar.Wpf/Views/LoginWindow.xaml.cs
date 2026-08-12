using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using LeaziEnergiaSolar.Wpf.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;

namespace LeaziEnergiaSolar.Wpf;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    private bool _senhaVisivel;

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

    private async void SenhaCampo_KeyDown(
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
        var senha = ObterSenhaInformada();

        var autenticado = await _viewModel.AutenticarAsync(
            senha);

        if (!autenticado)
        {
            LimparCamposDeSenha();
            SenhaPasswordBox.Focus();
            return;
        }

        LimparCamposDeSenha();

        var mainWindow = App.Services
            .GetRequiredService<MainWindow>();

        mainWindow.Show();
        Close();
    }

    private void AlternarSenha_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        if (_senhaVisivel)
        {
            OcultarSenha();
            return;
        }

        MostrarSenha();
    }

    private void MostrarSenha()
    {
        SenhaVisivelTextBox.Text =
            SenhaPasswordBox.Password;

        _senhaVisivel = true;

        SenhaPasswordBox.Visibility =
            Visibility.Collapsed;

        SenhaVisivelTextBox.Visibility =
            Visibility.Visible;

        IconeSenha.Kind =
            PackIconKind.EyeOff;

        SenhaVisivelTextBox.Focus();

        SenhaVisivelTextBox.CaretIndex =
            SenhaVisivelTextBox.Text.Length;
    }

    private void OcultarSenha()
    {
        SenhaPasswordBox.Password =
            SenhaVisivelTextBox.Text;

        _senhaVisivel = false;

        SenhaVisivelTextBox.Visibility =
            Visibility.Collapsed;

        SenhaPasswordBox.Visibility =
            Visibility.Visible;

        IconeSenha.Kind =
            PackIconKind.Eye;

        SenhaPasswordBox.Focus();
    }

    private string ObterSenhaInformada()
    {
        return _senhaVisivel
            ? SenhaVisivelTextBox.Text
            : SenhaPasswordBox.Password;
    }

    private void LimparCamposDeSenha()
    {
        SenhaPasswordBox.Clear();
        SenhaVisivelTextBox.Clear();

        _senhaVisivel = false;

        SenhaPasswordBox.Visibility =
            Visibility.Visible;

        SenhaVisivelTextBox.Visibility =
            Visibility.Collapsed;

        IconeSenha.Kind =
            PackIconKind.Eye;
    }

    private void Instagram_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        AbrirEndereco(
            "https://www.instagram.com/leazienergiasolar/");
    }

    private void WhatsApp_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        AbrirEndereco(
            "https://wa.me/5581973191935");
    }

    private static void AbrirEndereco(string endereco)
    {
        try
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = endereco,
                    UseShellExecute = true
                });
        }
        catch
        {
            MessageBox.Show(
                "Não foi possível abrir o endereço.",
                "Leazi Energia Solar",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}