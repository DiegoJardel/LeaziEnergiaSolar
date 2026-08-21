using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Wpf.ViewModels;
namespace LeaziEnergiaSolar.Wpf.Views.Pages;
public partial class FornecedoresView : UserControl
{
    private readonly FornecedoresViewModel _viewModel; private bool _carregado;
    public FornecedoresView(FornecedoresViewModel viewModel){InitializeComponent();_viewModel=viewModel;DataContext=viewModel;}
    private async void FornecedoresView_Loaded(object sender,RoutedEventArgs e){if(_carregado)return;_carregado=true;await _viewModel.CarregarCommand.ExecuteAsync(null);}
}
