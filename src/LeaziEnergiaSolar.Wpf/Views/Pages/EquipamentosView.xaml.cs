using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Wpf.ViewModels;

namespace LeaziEnergiaSolar.Wpf.Views.Pages;

public partial class EquipamentosView : UserControl
{
    private readonly EquipamentosViewModel _viewModel;
    private bool _carregado;
    public EquipamentosView(EquipamentosViewModel viewModel){InitializeComponent();_viewModel=viewModel;DataContext=viewModel;}
    private async void EquipamentosView_Loaded(object sender,RoutedEventArgs e){if(_carregado)return;_carregado=true;await _viewModel.CarregarCommand.ExecuteAsync(null);}
}
