using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Wpf.ViewModels;

namespace LeaziEnergiaSolar.Wpf.Views.Pages;

public partial class ControleAnualView : UserControl
{
    private readonly ControleAnualViewModel _viewModel;
    private bool _carregado;

    public ControleAnualView(ControleAnualViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void ControleAnualView_Loaded(
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
