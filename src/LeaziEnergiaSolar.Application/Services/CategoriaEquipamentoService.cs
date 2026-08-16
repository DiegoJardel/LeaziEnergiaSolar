using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Validators;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class CategoriaEquipamentoService : ICategoriaEquipamentoService
{
    private readonly ICategoriaEquipamentoRepository _repo;

    public CategoriaEquipamentoService(
        ICategoriaEquipamentoRepository repo) =>
        _repo = repo;

    public async Task<IReadOnlyList<CategoriaEquipamentoDto>> ListarAsync(
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default) =>
        (await _repo.ListarAsync(
            pesquisa,
            ativo,
            cancellationToken))
        .Select(Mapear)
        .ToList();

    public async Task<CategoriaEquipamentoDto?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidade = await _repo.ObterAsync(
            id,
            cancellationToken);

        return entidade is null
            ? null
            : Mapear(entidade);
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarCategoriaEquipamentoDto dto,
        CancellationToken cancellationToken = default)
    {
        dto = new SalvarCategoriaEquipamentoDto
        {
            Id = dto.Id,
            Descricao = EquipamentoValidator
                .Texto(dto.Descricao)
                .ToUpperInvariant(),
            Observacao = EquipamentoValidator.Texto(dto.Observacao),
            Ativo = dto.Ativo
        };

        var erros = CategoriaEquipamentoValidator.Validar(dto);
        if (erros.Count > 0)
        {
            return ResultadoOperacaoDto.Falha(
                string.Join(Environment.NewLine, erros));
        }

        if (await _repo.ExisteDescricaoAsync(
                dto.Descricao,
                dto.Id,
                cancellationToken))
        {
            return ResultadoOperacaoDto.Falha(
                "Já existe uma categoria com essa descrição.");
        }

        if (dto.Id.HasValue)
        {
            var entidade = await _repo.ObterAsync(
                dto.Id.Value,
                cancellationToken);

            if (entidade is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "A categoria selecionada não foi encontrada.");
            }

            entidade.Descricao = dto.Descricao;
            entidade.Observacao = string.IsNullOrWhiteSpace(dto.Observacao)
                ? null
                : dto.Observacao;
            entidade.Ativo = dto.Ativo;
            entidade.DataAtualizacao = DateTime.Now;

            await _repo.AtualizarAsync(
                entidade,
                cancellationToken);

            return ResultadoOperacaoDto.Ok(
                "Categoria atualizada com sucesso.");
        }

        var novaCategoria = new CategoriaEquipamento
        {
            Descricao = dto.Descricao,
            Observacao = string.IsNullOrWhiteSpace(dto.Observacao)
                ? null
                : dto.Observacao,
            Ativo = true,
            DataCadastro = DateTime.Now
        };

        await _repo.AdicionarAsync(
            novaCategoria,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Categoria cadastrada com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        var entidade = await _repo.ObterAsync(
            id,
            cancellationToken);

        if (entidade is null)
        {
            return ResultadoOperacaoDto.Falha(
                "A categoria selecionada não foi encontrada.");
        }

        entidade.Ativo = ativo;
        entidade.DataAtualizacao = DateTime.Now;

        await _repo.AtualizarAsync(
            entidade,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            ativo
                ? "Categoria reativada com sucesso."
                : "Categoria inativada com sucesso.");
    }

    private static CategoriaEquipamentoDto Mapear(
        CategoriaEquipamento entidade) =>
        new()
        {
            Id = entidade.Id,
            Descricao = entidade.Descricao,
            Observacao = entidade.Observacao ?? string.Empty,
            Ativo = entidade.Ativo,
            DataCadastro = entidade.DataCadastro,
            DataAtualizacao = entidade.DataAtualizacao
        };
}
