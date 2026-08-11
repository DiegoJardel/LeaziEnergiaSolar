using System.Windows;
using System.Windows.Controls;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Wpf.ViewModels;

namespace LeaziEnergiaSolar.Wpf.Views.Pages;

public partial class LancamentosView : UserControl
{
    private readonly LancamentosViewModel _viewModel;
    private bool _carregado;

    public LancamentosView(LancamentosViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private async void LancamentosView_Loaded(
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

    private async void Excluir_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        if (sender is not Button button ||
            button.DataContext is not LancamentoDto lancamento)
        {
            return;
        }

        var resposta = MessageBox.Show(
            $"Deseja excluir o lançamento do cliente {lancamento.Cliente}?\n\n" +
            "Esta ação não poderá ser desfeita.",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (resposta != MessageBoxResult.Yes)
        {
            return;
        }

        await _viewModel.ExcluirCommand.ExecuteAsync(lancamento);
    }
}
