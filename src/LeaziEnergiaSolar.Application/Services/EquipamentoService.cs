using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Validators;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class EquipamentoService : IEquipamentoService
{
    private readonly IEquipamentoRepository _repository;
    private readonly ICategoriaEquipamentoRepository _categoriaRepository;
    private readonly IMarcaRepository _marcaRepository;
    private readonly IUnidadeMedidaRepository _unidadeRepository;

    public EquipamentoService(IEquipamentoRepository repository, ICategoriaEquipamentoRepository categoriaRepository, IMarcaRepository marcaRepository, IUnidadeMedidaRepository unidadeRepository)
    {
        _repository = repository;
        _categoriaRepository = categoriaRepository;
        _marcaRepository = marcaRepository;
        _unidadeRepository = unidadeRepository;
    }

    public async Task<IReadOnlyList<EquipamentoDto>> ListarAsync(string? pesquisa = null, int? categoriaId = null, int? marcaId = null, bool? ativo = null, CancellationToken cancellationToken = default) =>
        (await _repository.ListarAsync(pesquisa, categoriaId, marcaId, ativo, cancellationToken)).Select(Mapear).ToList();

    public async Task<EquipamentoDto?> ObterAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _repository.ObterAsync(id, cancellationToken);
        return item is null ? null : Mapear(item);
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(SalvarEquipamentoDto dto, CancellationToken cancellationToken = default)
    {
        dto = Normalizar(dto);
        var erros = EquipamentoValidator.Validar(dto);
        if (erros.Count > 0) return ResultadoOperacaoDto.Falha(string.Join(Environment.NewLine, erros));

        Equipamento? existente = null;
        if (dto.Id.HasValue)
        {
            existente = await _repository.ObterAsync(dto.Id.Value, cancellationToken);
            if (existente is null)
                return ResultadoOperacaoDto.Falha("O equipamento selecionado não foi encontrado.");
        }

        var categoria = await _categoriaRepository.ObterAsync(dto.CategoriaEquipamentoId, cancellationToken);
        if (categoria is null || (!categoria.Ativo && (existente is null || existente.CategoriaEquipamentoId != dto.CategoriaEquipamentoId)))
            return ResultadoOperacaoDto.Falha("Selecione uma categoria ativa válida.");

        var unidade = await _unidadeRepository.ObterAsync(dto.UnidadeMedidaId, cancellationToken);
        if (unidade is null || (!unidade.Ativo && (existente is null || existente.UnidadeMedidaId != dto.UnidadeMedidaId)))
            return ResultadoOperacaoDto.Falha("Selecione uma unidade de medida ativa válida.");

        if (dto.MarcaId.HasValue)
        {
            var marca = await _marcaRepository.ObterAsync(dto.MarcaId.Value, cancellationToken);
            if (marca is null || (!marca.Ativo && (existente is null || existente.MarcaId != dto.MarcaId)))
                return ResultadoOperacaoDto.Falha("A marca selecionada não é válida.");
        }

        if (await _repository.ExisteDuplicadoAsync(dto.Descricao, dto.MarcaId, dto.Modelo, dto.Id, cancellationToken))
            return ResultadoOperacaoDto.Falha("Já existe um equipamento com a mesma descrição, marca e modelo.");

        if (dto.Id.HasValue)
        {
            var entidade = existente!;
            entidade.Descricao = dto.Descricao;
            entidade.CategoriaEquipamentoId = dto.CategoriaEquipamentoId;
            entidade.MarcaId = dto.MarcaId;
            entidade.Modelo = ValorNulo(dto.Modelo);
            entidade.UnidadeMedidaId = dto.UnidadeMedidaId;
            entidade.Observacao = ValorNulo(dto.Observacao);
            entidade.Ativo = dto.Ativo;
            entidade.DataAtualizacao = DateTime.Now;
            await _repository.AtualizarAsync(entidade, cancellationToken);
            return ResultadoOperacaoDto.Ok("Equipamento atualizado com sucesso.");
        }

        var novo = new Equipamento
        {
            Descricao = dto.Descricao,
            CategoriaEquipamentoId = dto.CategoriaEquipamentoId,
            MarcaId = dto.MarcaId,
            Modelo = ValorNulo(dto.Modelo),
            UnidadeMedidaId = dto.UnidadeMedidaId,
            Observacao = ValorNulo(dto.Observacao),
            Ativo = true,
            DataCadastro = DateTime.Now
        };
        await _repository.AdicionarAsync(novo, cancellationToken);
        return ResultadoOperacaoDto.Ok("Equipamento cadastrado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default)
    {
        var entidade = await _repository.ObterAsync(id, cancellationToken);
        if (entidade is null) return ResultadoOperacaoDto.Falha("O equipamento selecionado não foi encontrado.");
        entidade.Ativo = ativo;
        entidade.DataAtualizacao = DateTime.Now;
        await _repository.AtualizarAsync(entidade, cancellationToken);
        return ResultadoOperacaoDto.Ok(ativo ? "Equipamento reativado com sucesso." : "Equipamento inativado com sucesso.");
    }

    private static EquipamentoDto Mapear(
        Equipamento x) =>
        new()
        {
            Id = x.Id,
            Descricao = x.Descricao,
            CategoriaEquipamentoId = x.CategoriaEquipamentoId,
            Categoria = x.CategoriaEquipamento.Descricao,
            MarcaId = x.MarcaId,
            Marca = x.Marca?.Nome ?? string.Empty,
            Modelo = x.Modelo ?? string.Empty,
            UnidadeMedidaId = x.UnidadeMedidaId,
            UnidadeMedida = $"{x.UnidadeMedida.Sigla} - {x.UnidadeMedida.Descricao}",
            Observacao = x.Observacao ?? string.Empty,
            Ativo = x.Ativo,
            DataCadastro = x.DataCadastro,
            DataAtualizacao = x.DataAtualizacao
        };

    private static SalvarEquipamentoDto Normalizar(SalvarEquipamentoDto x) => new()
    {
        Id = x.Id, Descricao = NormalizarTexto(x.Descricao), CategoriaEquipamentoId = x.CategoriaEquipamentoId, MarcaId = x.MarcaId,
        Modelo = NormalizarTexto(x.Modelo), UnidadeMedidaId = x.UnidadeMedidaId,
        Observacao = NormalizarTexto(x.Observacao), Ativo = x.Ativo
    };

    private static string NormalizarTexto(string? value) => string.Join(" ", (value ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToUpperInvariant();
    private static string? ValorNulo(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
