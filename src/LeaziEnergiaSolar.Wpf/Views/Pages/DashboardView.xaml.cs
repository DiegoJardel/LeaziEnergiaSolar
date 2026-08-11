using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Wpf.ViewModels;

namespace LeaziEnergiaSolar.Wpf.Views.Pages;

public partial class DashboardView : UserControl
{
    private readonly DashboardViewModel _viewModel;
    private bool _carregado;

    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void DashboardView_Loaded(
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
}
