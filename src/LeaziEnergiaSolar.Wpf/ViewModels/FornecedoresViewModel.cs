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
    private readonly IFornecedorService _service;
    [ObservableProperty] private int? fornecedorId;
    [ObservableProperty] private TipoPessoa tipoPessoa = TipoPessoa.Juridica;
    [ObservableProperty] private string nomeRazaoSocial = string.Empty;
    [ObservableProperty] private string nomeFantasia = string.Empty;
    [ObservableProperty] private string cpfCnpj = string.Empty;
    [ObservableProperty] private string telefone = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string contatoResponsavel = string.Empty;
    [ObservableProperty] private string observacao = string.Empty;
    [ObservableProperty] private bool ativo = true;
    [ObservableProperty] private string pesquisa = string.Empty;
    [ObservableProperty] private string filtroStatus = "Ativos";
    [ObservableProperty] private FornecedorDto? fornecedorSelecionado;
    [ObservableProperty] private string mensagem = string.Empty;
    [ObservableProperty] private bool mensagemEhErro;
    [ObservableProperty] private bool estaCarregando;

    public ObservableCollection<FornecedorDto> Fornecedores { get; } = new();
    public IReadOnlyList<TipoPessoa> TiposPessoa { get; } = Enum.GetValues<TipoPessoa>();
    public IReadOnlyList<string> FiltrosStatus { get; } = new[] { "Todos", "Ativos", "Inativos" };
    public bool EstaEditando => FornecedorId.HasValue;
    public string TituloFormulario => EstaEditando ? $"Editar fornecedor {FornecedorId:D4}" : "Novo fornecedor";

    public FornecedoresViewModel(IFornecedorService service)=>_service=service;
    partial void OnFornecedorIdChanged(int? value){OnPropertyChanged(nameof(EstaEditando));OnPropertyChanged(nameof(TituloFormulario));}
    partial void OnCpfCnpjChanged(string value){var f=MaskHelper.FormatCpfCnpj(value);if(value!=f)CpfCnpj=f;}
    partial void OnTelefoneChanged(string value){var f=MaskHelper.FormatPhone(value);if(value!=f)Telefone=f;}

    [RelayCommand] private async Task CarregarAsync()=>await ExecutarAsync(CarregarListaAsync);
    [RelayCommand] private async Task PesquisarAsync()=>await ExecutarAsync(CarregarListaAsync);
    [RelayCommand] private async Task SalvarAsync()
    {
        await ExecutarAsync(async()=>{var r=await _service.SalvarAsync(new SalvarFornecedorDto{Id=FornecedorId,TipoPessoa=TipoPessoa,NomeRazaoSocial=NomeRazaoSocial,NomeFantasia=NomeFantasia,CpfCnpj=CpfCnpj,Telefone=Telefone,Email=Email,ContatoResponsavel=ContatoResponsavel,Observacao=Observacao,Ativo=Ativo});ExibirMensagem(r.Mensagem,!r.Sucesso);if(r.Sucesso){Limpar();await CarregarListaAsync();}});
    }
    [RelayCommand] private async Task EditarAsync(FornecedorDto x){FornecedorId=x.Id;TipoPessoa=x.TipoPessoa;NomeRazaoSocial=x.NomeRazaoSocial;NomeFantasia=x.NomeFantasia;CpfCnpj=x.CpfCnpj;Telefone=x.Telefone;Email=x.Email;ContatoResponsavel=x.ContatoResponsavel;Observacao=x.Observacao;Ativo=x.Ativo;await Task.CompletedTask;}
    [RelayCommand] private async Task AlterarStatusAsync(FornecedorDto x)=>await ExecutarAsync(async()=>{var r=await _service.AlterarStatusAsync(x.Id,!x.Ativo);ExibirMensagem(r.Mensagem,!r.Sucesso);if(r.Sucesso)await CarregarListaAsync();});
    [RelayCommand] private void Limpar(){FornecedorId=null;TipoPessoa=TipoPessoa.Juridica;NomeRazaoSocial=string.Empty;NomeFantasia=string.Empty;CpfCnpj=string.Empty;Telefone=string.Empty;Email=string.Empty;ContatoResponsavel=string.Empty;Observacao=string.Empty;Ativo=true;}
    private async Task CarregarListaAsync(){bool? a=FiltroStatus switch{"Ativos"=>true,"Inativos"=>false,_=>null};var lista=await _service.ListarAsync(Pesquisa,a);Fornecedores.Clear();foreach(var x in lista)Fornecedores.Add(x);}
    private async Task ExecutarAsync(Func<Task> acao){EstaCarregando=true;Mensagem=string.Empty;try{await acao();}catch(Exception ex){ExibirMensagem($"Não foi possível concluir a operação. {ex.Message}",true);}finally{EstaCarregando=false;}}
    private void ExibirMensagem(string texto,bool erro){Mensagem=texto;MensagemEhErro=erro;}
}
