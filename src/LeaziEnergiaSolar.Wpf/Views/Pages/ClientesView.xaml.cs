using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Wpf.ViewModels;

namespace LeaziEnergiaSolar.Wpf.Views.Pages;

public partial class ClientesView : UserControl
{
    private readonly ClientesViewModel _viewModel;
    private bool _carregado;

    public ClientesView(
        ClientesViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void ClientesView_Loaded(
        object sender,
        RoutedEventArgs eventArgs)
    {
        if (_carregado)
        {
            return;
        }

        _carregado = true;

        await _viewModel
            .CarregarCommand
            .ExecuteAsync(null);
    }
}