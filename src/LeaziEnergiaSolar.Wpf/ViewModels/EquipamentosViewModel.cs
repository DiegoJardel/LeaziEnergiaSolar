using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace LeaziEnergiaSolar.Wpf.ViewModels;

public partial class EquipamentosViewModel : ObservableObject
{
    private readonly IEquipamentoService _equipamentoService;
    private readonly ICategoriaEquipamentoService _categoriaService;
    private readonly IMarcaService _marcaService;
    private readonly IModeloEquipamentoService _modeloService;
    private readonly IUnidadeMedidaService _unidadeService;

    /*
     * EQUIPAMENTO
     */

    [ObservableProperty]
    private int? equipamentoId;

    [ObservableProperty]
    private string descricao = string.Empty;

    [ObservableProperty]
    private CategoriaEquipamentoDto? categoriaSelecionada;

    [ObservableProperty]
    private MarcaDto? marcaSelecionada;

    /*
     * Mantido temporariamente como texto.
     * Posteriormente será substituído por ModeloEquipamentoDto.
     */
    [ObservableProperty]
    private string modelo = string.Empty;

    [ObservableProperty]
    private UnidadeMedidaDto? unidadeSelecionada;

    [ObservableProperty]
    private string observacao = string.Empty;

    [ObservableProperty]
    private bool ativo = true;

    [ObservableProperty]
    private string pesquisa = string.Empty;

    [ObservableProperty]
    private string filtroStatus = "Ativos";

    [ObservableProperty]
    private CategoriaEquipamentoDto? filtroCategoria;

    [ObservableProperty]
    private MarcaDto? filtroMarca;

    [ObservableProperty]
    private EquipamentoDto? equipamentoSelecionado;

    /*
     * CATEGORIA
     */

    [ObservableProperty]
    private int? categoriaId;

    [ObservableProperty]
    private string categoriaDescricao = string.Empty;

    [ObservableProperty]
    private string categoriaObservacao = string.Empty;

    [ObservableProperty]
    private bool categoriaAtivo = true;

    [ObservableProperty]
    private CategoriaEquipamentoDto? categoriaSelecionadaAux;

    [ObservableProperty]
    private string pesquisaCategoria = string.Empty;

    [ObservableProperty]
    private string filtroStatusCategoria = "Todos";

    /*
     * MARCA
     */

    [ObservableProperty]
    private int? marcaId;

    [ObservableProperty]
    private string marcaNome = string.Empty;

    [ObservableProperty]
    private string marcaObservacao = string.Empty;

    [ObservableProperty]
    private bool marcaAtivo = true;

    [ObservableProperty]
    private MarcaDto? marcaSelecionadaAux;

    [ObservableProperty]
    private string pesquisaMarca = string.Empty;

    [ObservableProperty]
    private string filtroStatusMarca = "Todos";

    /*
     * MODELO DA MARCA
     */

    [ObservableProperty]
    private int? modeloId;

    [ObservableProperty]
    private string modeloNome = string.Empty;

    [ObservableProperty]
    private string modeloObservacao = string.Empty;

    [ObservableProperty]
    private bool modeloAtivo = true;

    [ObservableProperty]
    private ModeloEquipamentoDto? modeloSelecionadoAux;

    [ObservableProperty]
    private MarcaDto? marcaSelecionadaParaModelos;

    [ObservableProperty]
    private string pesquisaModelo = string.Empty;

    [ObservableProperty]
    private string filtroStatusModelo = "Todos";

    /*
     * UNIDADE
     */

    [ObservableProperty]
    private int? unidadeId;

    [ObservableProperty]
    private string unidadeSigla = string.Empty;

    [ObservableProperty]
    private string unidadeDescricao = string.Empty;

    [ObservableProperty]
    private bool unidadePermiteQuantidadeDecimal;

    [ObservableProperty]
    private bool unidadeAtivo = true;

    [ObservableProperty]
    private UnidadeMedidaDto? unidadeSelecionadaAux;

    [ObservableProperty]
    private string pesquisaUnidade = string.Empty;

    [ObservableProperty]
    private string filtroStatusUnidade = "Todos";

    /*
     * MENSAGENS E CARREGAMENTO
     */

    [ObservableProperty]
    private string mensagem = string.Empty;

    [ObservableProperty]
    private bool mensagemEhErro;

    [ObservableProperty]
    private bool estaCarregando;

    /*
     * COLEÇÕES
     */

    public ObservableCollection<EquipamentoDto> Equipamentos { get; } =
        new();

    public ObservableCollection<CategoriaEquipamentoDto> Categorias { get; } =
        new();

    public ObservableCollection<CategoriaEquipamentoDto>
        CategoriasDisponiveis
    { get; } = new();

    public ObservableCollection<MarcaDto> Marcas { get; } =
        new();

    public ObservableCollection<MarcaDto> MarcasDisponiveis { get; } =
        new();

    public ObservableCollection<ModeloEquipamentoDto> Modelos { get; } =
        new();

    public ObservableCollection<UnidadeMedidaDto> Unidades { get; } =
        new();

    public ObservableCollection<UnidadeMedidaDto>
        UnidadesDisponiveis
    { get; } = new();

    public IReadOnlyList<string> FiltrosStatus { get; } =
        new[]
        {
            "Todos",
            "Ativos",
            "Inativos"
        };

    /*
     * PROPRIEDADES CALCULADAS
     */

    public bool EstaEditando =>
        EquipamentoId.HasValue;

    public bool EditandoCategoria =>
        CategoriaId.HasValue;

    public bool EditandoMarca =>
        MarcaId.HasValue;

    public bool EditandoModelo =>
        ModeloId.HasValue;

    public bool EditandoUnidade =>
        UnidadeId.HasValue;

    public bool PossuiMarcaSelecionadaParaModelos =>
        MarcaSelecionadaParaModelos is not null;

    public string TituloFormulario =>
        EstaEditando
            ? $"Editar equipamento {EquipamentoId:D4}"
            : "Novo equipamento";

    public string TituloCategoria =>
        EditandoCategoria
            ? "Editar categoria"
            : "Nova categoria";

    public string TituloMarca =>
        EditandoMarca
            ? "Editar marca"
            : "Nova marca";

    public string TituloModelo =>
        EditandoModelo
            ? "Editar modelo"
            : "Novo modelo";

    public string TituloUnidade =>
        EditandoUnidade
            ? "Editar unidade"
            : "Nova unidade";

    public string NomeMarcaModelos =>
        MarcaSelecionadaParaModelos?.Nome
        ?? "Nenhuma marca selecionada";

    public EquipamentosViewModel(
        IEquipamentoService equipamentoService,
        ICategoriaEquipamentoService categoriaService,
        IMarcaService marcaService,
        IModeloEquipamentoService modeloService,
        IUnidadeMedidaService unidadeService)
    {
        _equipamentoService = equipamentoService;
        _categoriaService = categoriaService;
        _marcaService = marcaService;
        _modeloService = modeloService;
        _unidadeService = unidadeService;
    }

    /*
     * NOTIFICAÇÕES
     */

    partial void OnEquipamentoIdChanged(
        int? value)
    {
        OnPropertyChanged(nameof(EstaEditando));
        OnPropertyChanged(nameof(TituloFormulario));
    }

    partial void OnCategoriaIdChanged(
        int? value)
    {
        OnPropertyChanged(nameof(EditandoCategoria));
        OnPropertyChanged(nameof(TituloCategoria));
    }

    partial void OnMarcaIdChanged(
        int? value)
    {
        OnPropertyChanged(nameof(EditandoMarca));
        OnPropertyChanged(nameof(TituloMarca));
    }

    partial void OnModeloIdChanged(
        int? value)
    {
        OnPropertyChanged(nameof(EditandoModelo));
        OnPropertyChanged(nameof(TituloModelo));
    }

    partial void OnUnidadeIdChanged(
        int? value)
    {
        OnPropertyChanged(nameof(EditandoUnidade));
        OnPropertyChanged(nameof(TituloUnidade));
    }

    partial void OnMarcaSelecionadaParaModelosChanged(
        MarcaDto? value)
    {
        OnPropertyChanged(
            nameof(PossuiMarcaSelecionadaParaModelos));

        OnPropertyChanged(
            nameof(NomeMarcaModelos));
    }

    /*
     * CARREGAMENTO
     */

    [RelayCommand]
    private async Task CarregarAsync()
    {
        await ExecutarAsync(async () =>
        {
            await CarregarAuxiliaresAsync();
            await PesquisarInternoAsync();
        });
    }

    /*
     * EQUIPAMENTO
     */

    [RelayCommand]
    private async Task PesquisarAsync()
    {
        await ExecutarAsync(
            PesquisarInternoAsync);
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _equipamentoService.SalvarAsync(
                    new SalvarEquipamentoDto
                    {
                        Id = EquipamentoId,
                        Descricao = Descricao,
                        CategoriaEquipamentoId =
                            CategoriaSelecionada?.Id ?? 0,
                        MarcaId =
                            MarcaSelecionada?.Id ?? 0,
                        Modelo = Modelo,
                        UnidadeMedidaId =
                            UnidadeSelecionada?.Id ?? 0,
                        Observacao = Observacao,
                        Ativo = Ativo
                    });

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            LimparEquipamento();

            await PesquisarInternoAsync();
        });
    }

    [RelayCommand]
    private async Task EditarAsync(
        EquipamentoDto item)
    {
        EquipamentoId = item.Id;
        Descricao = item.Descricao;
        Modelo = item.Modelo;
        Observacao = item.Observacao;
        Ativo = item.Ativo;

        CategoriaSelecionada =
            Categorias.FirstOrDefault(
                x => x.Id == item.CategoriaEquipamentoId);

        MarcaSelecionada =
            Marcas.FirstOrDefault(
                x => x.Id == item.MarcaId);

        UnidadeSelecionada =
            Unidades.FirstOrDefault(
                x => x.Id == item.UnidadeMedidaId);

        if (CategoriaSelecionada is not null &&
            !CategoriasDisponiveis.Any(
                x => x.Id == CategoriaSelecionada.Id))
        {
            CategoriasDisponiveis.Add(
                CategoriaSelecionada);
        }

        if (MarcaSelecionada is not null &&
            !MarcasDisponiveis.Any(
                x => x.Id == MarcaSelecionada.Id))
        {
            MarcasDisponiveis.Add(
                MarcaSelecionada);
        }

        if (UnidadeSelecionada is not null &&
            !UnidadesDisponiveis.Any(
                x => x.Id == UnidadeSelecionada.Id))
        {
            UnidadesDisponiveis.Add(
                UnidadeSelecionada);
        }

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task AlterarStatusAsync(
        EquipamentoDto item)
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _equipamentoService.AlterarStatusAsync(
                    item.Id,
                    !item.Ativo);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (resultado.Sucesso)
            {
                await PesquisarInternoAsync();
            }
        });
    }

    [RelayCommand]
    private void Limpar()
    {
        LimparEquipamento();
    }

    private void LimparEquipamento()
    {
        EquipamentoId = null;
        Descricao = string.Empty;
        CategoriaSelecionada = null;
        MarcaSelecionada = null;
        Modelo = string.Empty;
        UnidadeSelecionada = null;
        Observacao = string.Empty;
        Ativo = true;
    }

    /*
     * CATEGORIA
     */

    [RelayCommand]
    private async Task SalvarCategoriaAsync()
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _categoriaService.SalvarAsync(
                    new SalvarCategoriaEquipamentoDto
                    {
                        Id = CategoriaId,
                        Descricao = CategoriaDescricao,
                        Observacao = CategoriaObservacao,
                        Ativo = CategoriaAtivo
                    });

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            ResetCategoria();

            await CarregarAuxiliaresAsync();
        });
    }

    [RelayCommand]
    private async Task EditarCategoriaAsync(
        CategoriaEquipamentoDto item)
    {
        CategoriaId = item.Id;
        CategoriaDescricao = item.Descricao;
        CategoriaObservacao = item.Observacao;
        CategoriaAtivo = item.Ativo;

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task AlterarStatusCategoriaAsync(
        CategoriaEquipamentoDto item)
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _categoriaService.AlterarStatusAsync(
                    item.Id,
                    !item.Ativo);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (resultado.Sucesso)
            {
                await CarregarAuxiliaresAsync();
            }
        });
    }

    [RelayCommand]
    private void LimparCategoria()
    {
        ResetCategoria();
    }

    private void ResetCategoria()
    {
        CategoriaId = null;
        CategoriaDescricao = string.Empty;
        CategoriaObservacao = string.Empty;
        CategoriaAtivo = true;
    }

    /*
     * MARCA
     */

    [RelayCommand]
    private async Task SalvarMarcaAsync()
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _marcaService.SalvarAsync(
                    new SalvarMarcaDto
                    {
                        Id = MarcaId,
                        Nome = MarcaNome,
                        Observacao = MarcaObservacao,
                        Ativo = MarcaAtivo
                    });

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            ResetMarca();

            await CarregarAuxiliaresAsync();
        });
    }

    [RelayCommand]
    private async Task EditarMarcaAsync(
        MarcaDto item)
    {
        MarcaId = item.Id;
        MarcaNome = item.Nome;
        MarcaObservacao = item.Observacao;
        MarcaAtivo = item.Ativo;

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task AlterarStatusMarcaAsync(
        MarcaDto item)
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _marcaService.AlterarStatusAsync(
                    item.Id,
                    !item.Ativo);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            await CarregarAuxiliaresAsync();

            if (MarcaSelecionadaParaModelos?.Id == item.Id)
            {
                MarcaSelecionadaParaModelos =
                    Marcas.FirstOrDefault(
                        x => x.Id == item.Id);
            }
        });
    }

    [RelayCommand]
    private async Task ExcluirMarcaAsync(
        MarcaDto item)
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _marcaService.ExcluirAsync(
                    item.Id);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            if (MarcaSelecionadaParaModelos?.Id == item.Id)
            {
                MarcaSelecionadaParaModelos = null;
                Modelos.Clear();
                ResetModelo();
            }

            ResetMarca();

            await CarregarAuxiliaresAsync();
        });
    }

    [RelayCommand]
    private async Task SelecionarMarcaModelosAsync(
        MarcaDto item)
    {
        MarcaSelecionadaParaModelos = item;
        MarcaSelecionadaAux = item;

        PesquisaModelo = string.Empty;
        FiltroStatusModelo = "Todos";

        ResetModelo();

        await ExecutarAsync(
            CarregarModelosAsync);
    }

    [RelayCommand]
    private void LimparMarca()
    {
        ResetMarca();
    }

    private void ResetMarca()
    {
        MarcaId = null;
        MarcaNome = string.Empty;
        MarcaObservacao = string.Empty;
        MarcaAtivo = true;
    }

    /*
     * MODELO
     */

    [RelayCommand]
    private async Task SalvarModeloAsync()
    {
        if (MarcaSelecionadaParaModelos is null)
        {
            ExibirMensagem(
                "Selecione uma marca para cadastrar o modelo.",
                true);

            return;
        }

        await ExecutarAsync(async () =>
        {
            var resultado =
                await _modeloService.SalvarAsync(
                    new SalvarModeloEquipamentoDto
                    {
                        Id = ModeloId,
                        MarcaId =
                            MarcaSelecionadaParaModelos.Id,
                        Nome = ModeloNome,
                        Observacao = ModeloObservacao,
                        Ativo = ModeloAtivo
                    });

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            ResetModelo();

            await CarregarModelosAsync();
        });
    }

    [RelayCommand]
    private async Task PesquisarModelosAsync()
    {
        if (MarcaSelecionadaParaModelos is null)
        {
            Modelos.Clear();
            return;
        }

        await ExecutarAsync(
            CarregarModelosAsync);
    }

    [RelayCommand]
    private async Task EditarModeloAsync(
        ModeloEquipamentoDto item)
    {
        ModeloId = item.Id;
        ModeloNome = item.Nome;
        ModeloObservacao = item.Observacao;
        ModeloAtivo = item.Ativo;
        ModeloSelecionadoAux = item;

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task AlterarStatusModeloAsync(
        ModeloEquipamentoDto item)
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _modeloService.AlterarStatusAsync(
                    item.Id,
                    !item.Ativo);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            ResetModelo();

            await CarregarModelosAsync();
        });
    }

    [RelayCommand]
    private async Task ExcluirModeloAsync(
        ModeloEquipamentoDto item)
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _modeloService.ExcluirAsync(
                    item.Id);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            ResetModelo();

            await CarregarModelosAsync();
        });
    }

    [RelayCommand]
    private void LimparModelo()
    {
        ResetModelo();
    }

    private void ResetModelo()
    {
        ModeloId = null;
        ModeloNome = string.Empty;
        ModeloObservacao = string.Empty;
        ModeloAtivo = true;
        ModeloSelecionadoAux = null;
    }

    private async Task CarregarModelosAsync()
    {
        Modelos.Clear();

        if (MarcaSelecionadaParaModelos is null)
        {
            return;
        }

        bool? ativoFiltro = FiltroStatusModelo switch
        {
            "Ativos" => true,
            "Inativos" => false,
            _ => null
        };

        var modelos =
            await _modeloService.ListarAsync(
                MarcaSelecionadaParaModelos.Id,
                PesquisaModelo,
                ativoFiltro);

        foreach (var item in modelos)
        {
            Modelos.Add(item);
        }
    }

    /*
     * UNIDADE
     */

    [RelayCommand]
    private async Task SalvarUnidadeAsync()
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _unidadeService.SalvarAsync(
                    new SalvarUnidadeMedidaDto
                    {
                        Id = UnidadeId,
                        Sigla = UnidadeSigla,
                        Descricao = UnidadeDescricao,
                        PermiteQuantidadeDecimal =
                            UnidadePermiteQuantidadeDecimal,
                        Ativo = UnidadeAtivo
                    });

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (!resultado.Sucesso)
            {
                return;
            }

            ResetUnidade();

            await CarregarAuxiliaresAsync();
        });
    }

    [RelayCommand]
    private async Task EditarUnidadeAsync(
        UnidadeMedidaDto item)
    {
        UnidadeId = item.Id;
        UnidadeSigla = item.Sigla;
        UnidadeDescricao = item.Descricao;
        UnidadePermiteQuantidadeDecimal =
            item.PermiteQuantidadeDecimal;
        UnidadeAtivo = item.Ativo;

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task AlterarStatusUnidadeAsync(
        UnidadeMedidaDto item)
    {
        await ExecutarAsync(async () =>
        {
            var resultado =
                await _unidadeService.AlterarStatusAsync(
                    item.Id,
                    !item.Ativo);

            ExibirMensagem(
                resultado.Mensagem,
                !resultado.Sucesso);

            if (resultado.Sucesso)
            {
                await CarregarAuxiliaresAsync();
            }
        });
    }

    [RelayCommand]
    private void LimparUnidade()
    {
        ResetUnidade();
    }

    private void ResetUnidade()
    {
        UnidadeId = null;
        UnidadeSigla = string.Empty;
        UnidadeDescricao = string.Empty;
        UnidadePermiteQuantidadeDecimal = false;
        UnidadeAtivo = true;
    }

    /*
     * CARREGAMENTO DOS AUXILIARES
     */

    private async Task CarregarAuxiliaresAsync()
    {
        var marcaModelosId =
            MarcaSelecionadaParaModelos?.Id;

        var categorias =
            await _categoriaService.ListarAsync(
                null,
                null);

        Categorias.Clear();

        foreach (var categoria in categorias)
        {
            Categorias.Add(categoria);
        }

        CategoriasDisponiveis.Clear();

        foreach (var categoria in Categorias.Where(x => x.Ativo))
        {
            CategoriasDisponiveis.Add(categoria);
        }

        var marcas =
            await _marcaService.ListarAsync(
                null,
                null);

        Marcas.Clear();

        foreach (var marca in marcas)
        {
            Marcas.Add(marca);
        }

        MarcasDisponiveis.Clear();

        foreach (var marca in Marcas.Where(x => x.Ativo))
        {
            MarcasDisponiveis.Add(marca);
        }

        if (marcaModelosId.HasValue)
        {
            MarcaSelecionadaParaModelos =
                Marcas.FirstOrDefault(
                    x => x.Id == marcaModelosId.Value);

            MarcaSelecionadaAux =
                MarcaSelecionadaParaModelos;
        }

        var unidades =
            await _unidadeService.ListarAsync(
                null,
                null);

        Unidades.Clear();

        foreach (var unidade in unidades)
        {
            Unidades.Add(unidade);
        }

        UnidadesDisponiveis.Clear();

        foreach (var unidade in Unidades.Where(x => x.Ativo))
        {
            UnidadesDisponiveis.Add(unidade);
        }
    }

    private async Task PesquisarInternoAsync()
    {
        bool? ativoFiltro = FiltroStatus switch
        {
            "Ativos" => true,
            "Inativos" => false,
            _ => null
        };

        var itens =
            await _equipamentoService.ListarAsync(
                Pesquisa,
                FiltroCategoria?.Id,
                FiltroMarca?.Id,
                ativoFiltro);

        Equipamentos.Clear();

        foreach (var item in itens)
        {
            Equipamentos.Add(item);
        }
    }

    /*
     * EXECUÇÃO E MENSAGENS
     */

    private async Task ExecutarAsync(
        Func<Task> acao)
    {
        if (EstaCarregando)
        {
            return;
        }

        EstaCarregando = true;
        Mensagem = string.Empty;

        try
        {
            await acao();
        }
        catch (Exception exception)
        {
            ExibirMensagem(
                $"Não foi possível concluir a operação. " +
                exception.Message,
                true);
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    private void ExibirMensagem(
        string texto,
        bool erro)
    {
        Mensagem = texto;
        MensagemEhErro = erro;
    }
}