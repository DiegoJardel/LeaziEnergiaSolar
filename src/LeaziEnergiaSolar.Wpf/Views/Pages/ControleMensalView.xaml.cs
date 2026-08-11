using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Wpf.ViewModels;

namespace LeaziEnergiaSolar.Wpf.Views.Pages;

public partial class ControleMensalView : UserControl
{
    private readonly ControleMensalViewModel _viewModel;
    private bool _carregado;

    public ControleMensalView(ControleMensalViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void ControleMensalView_Loaded(
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
