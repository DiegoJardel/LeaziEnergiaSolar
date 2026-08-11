using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Wpf.Services;
using LeaziEnergiaSolar.Wpf.Views.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace LeaziEnergiaSolar.Wpf;

public partial class MainWindow : Window
{
    private readonly IUsuarioSessaoService _usuarioSessaoService;

    public MainWindow(IUsuarioSessaoService usuarioSessaoService)
    {
        InitializeComponent();

        _usuarioSessaoService = usuarioSessaoService;
        CarregarUsuarioAutenticado();
        AbrirDashboard();
    }

    private void CarregarUsuarioAutenticado()
    {
        var usuario = _usuarioSessaoService.UsuarioAtual;

        if (usuario is null)
        {
            FecharEVoltarAoLogin();
            return;
        }

        UsuarioNomeText.Text = usuario.Nome;
        UsuarioPerfilText.Text = usuario.Perfil.ToString();

        UsuariosButton.Visibility = usuario.Perfil == PerfilUsuario.Administrador
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void Navigate_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        var name = (string)((Button)sender).Tag;

        TituloText.Text = name switch
        {
            "Lancamentos" => "Lançamentos",
            "Mensal" => "Controle Mensal",
            "Anual" => "Controle Anual",
            _ => name
        };

        if (name == "Dashboard")
        {
            AbrirDashboard();
            return;
        }

        if (name == "Vendedores")
        {
            PageContent.Content = App.Services
                .GetRequiredService<VendedoresView>();

            return;
        }

        if (name == "Lancamentos")
        {
            PageContent.Content = App.Services
                .GetRequiredService<LancamentosView>();

            return;
        }

        if (name == "Mensal")
        {
            PageContent.Content = App.Services
                .GetRequiredService<ControleMensalView>();

            return;
        }

        if (name == "Anual")
        {
            PageContent.Content = App.Services
                .GetRequiredService<ControleAnualView>();

            return;
        }

        if (name == "Usuarios")
        {
            var usuario = _usuarioSessaoService.UsuarioAtual;

            if (usuario?.Perfil != PerfilUsuario.Administrador)
            {
                MessageBox.Show(
                    "Apenas administradores podem acessar este módulo.",
                    "Leazi Energia Solar",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            PageContent.Content = App.Services
                .GetRequiredService<UsuariosView>();

            return;
        }

        PageContent.Content = CriarModuloPendente(TituloText.Text);
    }

    private void AbrirDashboard()
    {
        TituloText.Text = "Dashboard";
        PageContent.Content = App.Services
            .GetRequiredService<DashboardView>();
    }

    private static Border CriarModuloPendente(string titulo)
    {
        return new Border
        {
            Background = System.Windows.Media.Brushes.White,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(28),
            Child = new TextBlock
            {
                Text = $"Módulo {titulo}\n\n" +
                       "A estrutura visual está preparada. " +
                       "O CRUD será implementado na etapa específica.",
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap
            }
        };
    }

    private async void Backup_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        try
        {
            var caminho = await App.Services
                .GetRequiredService<IBackupService>()
                .CriarBackupAsync();

            MessageBox.Show(
                $"Backup criado com sucesso em:{caminho}",
                "Leazi Energia Solar",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            App.Services
                .GetRequiredService<ILogService>()
                .RegistrarErro(exception, "Backup manual");

            MessageBox.Show(
                "Não foi possível criar o backup.",
                "Leazi Energia Solar",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Sair_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        _usuarioSessaoService.Encerrar();
        FecharEVoltarAoLogin();
    }

    private void FecharEVoltarAoLogin()
    {
        var loginWindow = App.Services
            .GetRequiredService<LoginWindow>();

        loginWindow.Show();
        Close();
    }
}
