using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Wpf.ViewModels;

namespace LeaziEnergiaSolar.Wpf.Views.Pages;

public partial class RelatoriosView : UserControl
{
    public RelatoriosView(
        RelatoriosViewModel viewModel)
    {
        InitializeComponent();

        DataContext =
            viewModel;
    }

    private async void RelatoriosView_Loaded(
        object sender,
        RoutedEventArgs eventArgs)
    {
        if (DataContext is not RelatoriosViewModel viewModel)
        {
            return;
        }

        await viewModel
            .CarregarCommand
            .ExecuteAsync(
                null);
    }
}