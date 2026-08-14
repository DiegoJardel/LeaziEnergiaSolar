using System.Windows;
using System.Windows.Threading;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Services;
using LeaziEnergiaSolar.Domain.Interfaces;
using LeaziEnergiaSolar.Infrastructure.Data;
using LeaziEnergiaSolar.Infrastructure.Repositories;
using LeaziEnergiaSolar.Infrastructure.Services;
using LeaziEnergiaSolar.Wpf.Services;
using LeaziEnergiaSolar.Wpf.ViewModels;
using LeaziEnergiaSolar.Wpf.Views.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LeaziEnergiaSolar.Wpf;

public partial class App : System.Windows.Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override async void OnStartup(
        StartupEventArgs eventArgs)
    {
        base.OnStartup(eventArgs);

        AppPaths.PrepararPastas();

        Services = ConfigureServices();

        RegistrarTratamentoGlobalDeErros();

        try
        {
            await Services
                .GetRequiredService<IBackupService>()
                .CriarBackupAutomaticoAsync();

            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<LeaziDbContext>();

                await DbInitializer.InitializeAsync(
                    dbContext);
            }

            Services
                .GetRequiredService<LoginWindow>()
                .Show();
        }
        catch (Exception exception)
        {
            Services
                .GetRequiredService<ILogService>()
                .RegistrarErro(
                    exception,
                    "Inicialização do sistema");

            MessageBox.Show(
                "Não foi possível iniciar o sistema. " +
                "Consulte a pasta de logs.",
                "Leazi Energia Solar",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(-1);
        }
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddDbContext<LeaziDbContext>(options =>
            options.UseSqlite(
                $"Data Source={AppPaths.BancoDados}",
                sqliteOptions =>
                {
                    sqliteOptions.MigrationsAssembly(
                        typeof(LeaziDbContext)
                            .Assembly
                            .FullName);
                }));

        services.AddScoped<
            IUsuarioRepository,
            UsuarioRepository>();

        services.AddScoped<
            IVendedorRepository,
            VendedorRepository>();

        services.AddScoped<
            IClienteRepository,
            ClienteRepository>();

        services.AddScoped<
            ILocalidadeRepository,
            LocalidadeRepository>();

        services.AddScoped<
            ILancamentoRepository,
            LancamentoRepository>();

        services.AddScoped<
            IDashboardRepository,
            DashboardRepository>();

        services.AddScoped<
            IAutenticacaoService,
            AutenticacaoService>();

        services.AddScoped<
            IVendedorService,
            VendedorService>();

        services.AddScoped<
            IClienteService,
            ClienteService>();

        services.AddSingleton<ICepService, ViaCepService>();

        services.AddScoped<IIbgeLocalidadeService, IbgeLocalidadeService>();

        services.AddScoped<
            ILancamentoService,
            LancamentoService>();

        services.AddScoped<
            IDashboardService,
            DashboardService>();

        services.AddScoped<
            IControleMensalService,
            ControleMensalService>();

        services.AddScoped<
            IControleAnualService,
            ControleAnualService>();

        services.AddScoped<
            IUsuarioService,
            UsuarioService>();

        services.AddSingleton<
            IUsuarioSessaoService,
            UsuarioSessaoService>();

        services.AddSingleton<
            ILogService,
            LogService>();

        services.AddSingleton<
            IBackupService,
            BackupService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<VendedoresViewModel>();
        services.AddTransient<ClientesViewModel>();
        services.AddTransient<LancamentosViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ControleMensalViewModel>();
        services.AddTransient<ControleAnualViewModel>();
        services.AddTransient<UsuariosViewModel>();

        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<VendedoresView>();
        services.AddTransient<ClientesView>();
        services.AddTransient<LancamentosView>();
        services.AddTransient<DashboardView>();
        services.AddTransient<ControleMensalView>();
        services.AddTransient<ControleAnualView>();
        services.AddTransient<UsuariosView>();

        return services.BuildServiceProvider();
    }

    private void RegistrarTratamentoGlobalDeErros()
    {
        DispatcherUnhandledException +=
            OnDispatcherUnhandledException;

        AppDomain.CurrentDomain.UnhandledException +=
            OnUnhandledException;

        TaskScheduler.UnobservedTaskException +=
            OnUnobservedTaskException;
    }

    private static void OnDispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs eventArgs)
    {
        RegistrarErroGlobal(
            eventArgs.Exception,
            "Interface WPF");

        MessageBox.Show(
            "Ocorreu um erro inesperado. " +
            "O detalhe foi registrado no log.",
            "Leazi Energia Solar",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        eventArgs.Handled = true;
    }

    private static void OnUnhandledException(
        object? sender,
        UnhandledExceptionEventArgs eventArgs)
    {
        if (eventArgs.ExceptionObject is Exception exception)
        {
            RegistrarErroGlobal(
                exception,
                "Aplicação");
        }
    }

    private static void OnUnobservedTaskException(
        object? sender,
        UnobservedTaskExceptionEventArgs eventArgs)
    {
        RegistrarErroGlobal(
            eventArgs.Exception,
            "Tarefa assíncrona");

        eventArgs.SetObserved();
    }

    private static void RegistrarErroGlobal(
        Exception exception,
        string contexto)
    {
        try
        {
            Services
                .GetRequiredService<ILogService>()
                .RegistrarErro(
                    exception,
                    contexto);
        }
        catch
        {
        }
    }
}