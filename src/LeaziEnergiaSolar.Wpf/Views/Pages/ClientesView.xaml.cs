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

    private async void Excluir_Click(
        object sender,
        RoutedEventArgs eventArgs)
    {
        if (sender is not Button button ||
            button.DataContext is not Application.DTOs.ClienteDto cliente)
        {
            return;
        }

        var resposta = MessageBox.Show(
            $"Deseja excluir o cliente {cliente.NomeRazaoSocial}?\n\n" +
            "O sistema não permitirá a exclusão se houver lançamentos vinculados.",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (resposta != MessageBoxResult.Yes)
        {
            return;
        }

        await _viewModel.ExcluirCommand.ExecuteAsync(cliente);
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