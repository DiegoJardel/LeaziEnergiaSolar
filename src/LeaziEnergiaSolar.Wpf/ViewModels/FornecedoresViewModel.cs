using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Wpf.Utils;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class FornecedoresViewModel : ObservableObject
{
    private readonly IFornecedorService
        _service;

    [ObservableProperty]
    private int?
        fornecedorId;

    [ObservableProperty]
    private TipoPessoa tipoPessoa =
        TipoPessoa.Juridica;

    [ObservableProperty]
    private string nomeRazaoSocial =
        string.Empty;

    [ObservableProperty]
    private string nomeFantasia =
        string.Empty;

    [ObservableProperty]
    private string cpfCnpj =
        string.Empty;

    [ObservableProperty]
    private string telefone =
        string.Empty;

    [ObservableProperty]
    private string email =
        string.Empty;

    [ObservableProperty]
    private string contatoResponsavel =
        string.Empty;

    [ObservableProperty]
    private string observacao =
        string.Empty;

    [ObservableProperty]
    private bool ativo =
        true;

    [ObservableProperty]
    private string pesquisa =
        string.Empty;

    [ObservableProperty]
    private string filtroStatus =
        "Ativos";

    [ObservableProperty]
    private FornecedorDto?
        fornecedorSelecionado;

    [ObservableProperty]
    private string mensagem =
        string.Empty;

    [ObservableProperty]
    private bool mensagemEhErro;

    [ObservableProperty]
    private bool estaCarregando;

    public ObservableCollection<FornecedorDto>
        Fornecedores
    { get; } =
        new();

    public IReadOnlyList<TipoPessoa>
        TiposPessoa
    { get; } =
        Enum.GetValues<TipoPessoa>();

    public IReadOnlyList<string>
        FiltrosStatus
    { get; } =
        new[]
        {
            "Todos",
            "Ativos",
            "Inativos"
        };

    public bool EstaEditando =>
        FornecedorId.HasValue;

    public string TituloFormulario =>
        EstaEditando
            ? $"Editar fornecedor {FornecedorId:D4}"
            : "Novo fornecedor";

    public FornecedoresViewModel(
        IFornecedorService service)
    {
        _service =
            service
            ?? throw new ArgumentNullException(
                nameof(service));
    }

    partial void OnFornecedorIdChanged(
        int? value)
    {
        OnPropertyChanged(
            nameof(EstaEditando));

        OnPropertyChanged(
            nameof(TituloFormulario));
    }

    partial void OnCpfCnpjChanged(
        string value)
    {
        var valorFormatado =
            MaskHelper.FormatCpfCnpj(
                value);

        if (value != valorFormatado)
        {
            CpfCnpj =
                valorFormatado;
        }
    }

    partial void OnTelefoneChanged(
        string value)
    {
        var valorFormatado =
            MaskHelper.FormatPhone(
                value);

        if (value != valorFormatado)
        {
            Telefone =
                valorFormatado;
        }
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        await ExecutarAsync(
            CarregarListaAsync);
    }

    [RelayCommand]
    private async Task PesquisarAsync()
    {
        await ExecutarAsync(
            CarregarListaAsync);
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _service.SalvarAsync(
                    new SalvarFornecedorDto
                    {
                        Id =
                            FornecedorId,

                        TipoPessoa =
                            TipoPessoa,

                        NomeRazaoSocial =
                            NomeRazaoSocial,

                        NomeFantasia =
                            NomeFantasia,

                        CpfCnpj =
                            CpfCnpj,

                        Telefone =
                            Telefone,

                        Email =
                            Email,

                        ContatoResponsavel =
                            ContatoResponsavel,

                        Observacao =
                            Observacao,

                        Ativo =
                            Ativo
                    });

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            LimparFormulario();

            await CarregarListaAsync();
        });
    }

    [RelayCommand]
    private void Editar(
        FornecedorDto? item)
    {
        if (item is null)
        {
            return;
        }

        FornecedorId =
            item.Id;

        TipoPessoa =
            item.TipoPessoa;

        NomeRazaoSocial =
            item.NomeRazaoSocial;

        NomeFantasia =
            item.NomeFantasia;

        CpfCnpj =
            item.CpfCnpj;

        Telefone =
            item.Telefone;

        Email =
            item.Email;

        ContatoResponsavel =
            item.ContatoResponsavel;

        Observacao =
            item.Observacao;

        Ativo =
            item.Ativo;

        FornecedorSelecionado =
            item;
    }

    [RelayCommand]
    private async Task AlterarStatusAsync(
        FornecedorDto? item)
    {
        if (item is null)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado =
                await _service.AlterarStatusAsync(
                    item.Id,
                    !item.Ativo);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            if (FornecedorId ==
                item.Id)
            {
                Ativo =
                    !item.Ativo;
            }

            await CarregarListaAsync();
        });
    }

    [RelayCommand]
    private async Task ExcluirFornecedorAsync(
        FornecedorDto? item)
    {
        if (item is null)
        {
            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado =
                await _service.ExcluirAsync(
                    item.Id);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            if (FornecedorId ==
                item.Id)
            {
                LimparFormulario();
            }

            if (FornecedorSelecionado?.Id ==
                item.Id)
            {
                FornecedorSelecionado =
                    null;
            }

            await CarregarListaAsync();
        });
    }

    [RelayCommand]
    private void Limpar()
    {
        LimparFormulario();

        Mensagem =
            string.Empty;

        MensagemEhErro =
            false;
    }

    private void LimparFormulario()
    {
        FornecedorId =
            null;

        TipoPessoa =
            TipoPessoa.Juridica;

        NomeRazaoSocial =
            string.Empty;

        NomeFantasia =
            string.Empty;

        CpfCnpj =
            string.Empty;

        Telefone =
            string.Empty;

        Email =
            string.Empty;

        ContatoResponsavel =
            string.Empty;

        Observacao =
            string.Empty;

        Ativo =
            true;

        FornecedorSelecionado =
            null;
    }

    private async Task CarregarListaAsync()
    {
        bool? ativoFiltro =
            FiltroStatus switch
            {
                "Ativos" =>
                    true,

                "Inativos" =>
                    false,

                _ =>
                    null
            };

        var fornecedores =
            await _service.ListarAsync(
                Pesquisa,
                ativoFiltro);

        Fornecedores.Clear();

        foreach (var fornecedor in fornecedores)
        {
            Fornecedores.Add(
                fornecedor);
        }
    }

    private async Task ExecutarAsync(
        Func<Task> acao)
    {
        if (EstaCarregando)
        {
            return;
        }

        try
        {
            EstaCarregando =
                true;

            Mensagem =
                string.Empty;

            MensagemEhErro =
                false;

            await acao();
        }
        catch (Exception exception)
        {
            var mensagemRaiz =
                exception
                    .GetBaseException()
                    .Message;

            ExibirMensagem(
                "Não foi possível concluir a operação. " +
                mensagemRaiz,
                true);
        }
        finally
        {
            EstaCarregando =
                false;
        }
    }

    private void ExibirMensagem(
        string texto,
        bool erro)
    {
        Mensagem =
            texto;

        MensagemEhErro =
            erro;
    }
}