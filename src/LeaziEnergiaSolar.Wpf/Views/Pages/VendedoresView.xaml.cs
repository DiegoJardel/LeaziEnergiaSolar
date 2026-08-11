using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Wpf.ViewModels;

namespace LeaziEnergiaSolar.Wpf.Views.Pages;

public partial class VendedoresView : UserControl
{
    private readonly VendedoresViewModel _viewModel;
    private bool _carregado;

    public VendedoresView(VendedoresViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void VendedoresView_Loaded(
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
