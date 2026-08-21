using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Wpf.Services;
using LeaziEnergiaSolar.Wpf.Views.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace LeaziEnergiaSolar.Wpf;

public partial class MainWindow : Window
{
    private readonly IUsuarioSessaoService
        _usuarioSessaoService;

    public MainWindow(
        IUsuarioSessaoService usuarioSessaoService)
    {
        InitializeComponent();

        _usuarioSessaoService =
            usuarioSessaoService
            ?? throw new ArgumentNullException(
                nameof(usuarioSessaoService));

        CarregarUsuarioAutenticado();

        AbrirDashboard();
    }

    private void CarregarUsuarioAutenticado()
    {
        var usuario =
            _usuarioSessaoService.UsuarioAtual;

        if (usuario is null)
        {
            FecharEVoltarAoLogin();

            return;
        }

        UsuarioNomeText.Text =
            usuario.Nome;

        UsuarioPerfilText.Text =
            usuario.Perfil.ToString();

        UsuariosButton.Visibility =
            usuario.Perfil ==
            PerfilUsuario.Administrador
                ? Visibility.Visible
                : Visibility.Collapsed;
    }

    private void Navigate_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        if (sender is not Button button ||
            button.Tag is not string nomeModulo)
        {
            return;
        }

        TituloText.Text =
            nomeModulo switch
            {
                "Dashboard" =>
                    "Dashboard",

                "Vendedores" =>
                    "Vendedores",

                "Clientes" =>
                    "Clientes",

                "Equipamentos" =>
                    "Equipamentos",

                "Fornecedores" =>
                    "Fornecedores",

                "Lancamentos" =>
                    "Lançamentos",

                "Mensal" =>
                    "Controle Mensal",

                "Anual" =>
                    "Controle Anual",

                "Relatorios" =>
                    "Relatórios",

                "Usuarios" =>
                    "Usuários",

                _ =>
                    nomeModulo
            };

        if (nomeModulo == "Dashboard")
        {
            AbrirDashboard();

            return;
        }

        if (nomeModulo == "Vendedores")
        {
            PageContent.Content =
                App.Services
                    .GetRequiredService<VendedoresView>();

            return;
        }

        if (nomeModulo == "Clientes")
        {
            PageContent.Content =
                App.Services
                    .GetRequiredService<ClientesView>();

            return;
        }

        if (nomeModulo == "Equipamentos")
        {
            PageContent.Content =
                App.Services
                    .GetRequiredService<EquipamentosView>();

            return;
        }

        if (nomeModulo == "Fornecedores")
        {
            PageContent.Content =
                App.Services
                    .GetRequiredService<FornecedoresView>();

            return;
        }

        if (nomeModulo == "Lancamentos")
        {
            PageContent.Content =
                App.Services
                    .GetRequiredService<LancamentosView>();

            return;
        }

        if (nomeModulo == "Mensal")
        {
            PageContent.Content =
                App.Services
                    .GetRequiredService<ControleMensalView>();

            return;
        }

        if (nomeModulo == "Anual")
        {
            PageContent.Content =
                App.Services
                    .GetRequiredService<ControleAnualView>();

            return;
        }

        if (nomeModulo == "Relatorios")
        {
            PageContent.Content =
                App.Services
                    .GetRequiredService<RelatoriosView>();

            return;
        }

        if (nomeModulo == "Usuarios")
        {
            AbrirUsuarios();

            return;
        }

        PageContent.Content =
            CriarModuloPendente(
                TituloText.Text);
    }

    private void AbrirDashboard()
    {
        TituloText.Text =
            "Dashboard";

        PageContent.Content =
            App.Services
                .GetRequiredService<DashboardView>();
    }

    private void AbrirUsuarios()
    {
        var usuario =
            _usuarioSessaoService.UsuarioAtual;

        if (usuario?.Perfil !=
            PerfilUsuario.Administrador)
        {
            MessageBox.Show(
                "Apenas administradores podem acessar este módulo.",
                "Leazi Energia Solar",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        TituloText.Text =
            "Usuários";

        PageContent.Content =
            App.Services
                .GetRequiredService<UsuariosView>();
    }

    private static Border CriarModuloPendente(
        string titulo)
    {
        return new Border
        {
            Background =
                System.Windows.Media.Brushes.White,

            CornerRadius =
                new CornerRadius(
                    10),

            Padding =
                new Thickness(
                    28),

            Child =
                new TextBlock
                {
                    Text =
                        $"Módulo {titulo}\n\n" +
                        "A estrutura visual está preparada. " +
                        "O CRUD será implementado na etapa específica.",

                    FontSize =
                        20,

                    TextWrapping =
                        TextWrapping.Wrap
                }
        };
    }

    private async void Backup_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        try
        {
            var backupService =
                App.Services
                    .GetRequiredService<IBackupService>();

            var caminho =
                await backupService
                    .CriarBackupAsync();

            MessageBox.Show(
                $"Backup criado com sucesso em:\n{caminho}",
                "Leazi Energia Solar",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            var logService =
                App.Services
                    .GetRequiredService<ILogService>();

            logService.RegistrarErro(
                exception,
                "Backup manual");

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
        var loginWindow =
            App.Services
                .GetRequiredService<LoginWindow>();

        loginWindow.Show();

        Close();
    }
}