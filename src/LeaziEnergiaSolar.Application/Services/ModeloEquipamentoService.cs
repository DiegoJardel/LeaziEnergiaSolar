using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class ModeloEquipamentoService(
    IModeloEquipamentoRepository repository,
    IMarcaRepository marcaRepository)
    : IModeloEquipamentoService
{
    public async Task<IReadOnlyList<ModeloEquipamentoDto>> ListarAsync(
        int marcaId,
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default)
    {
        var modelos = await repository.ListarAsync(
            marcaId,
            pesquisa,
            ativo,
            cancellationToken);

        return modelos
            .Select(MapearDto)
            .ToList();
    }

    public async Task<ModeloEquipamentoDto?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var modelo = await repository.ObterAsync(
            id,
            cancellationToken);

        return modelo is null
            ? null
            : MapearDto(modelo);
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarModeloEquipamentoDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.MarcaId <= 0)
        {
            return ResultadoOperacaoDto.Falha(
                "Selecione uma marca válida.");
        }

        var marca = await marcaRepository.ObterAsync(
            dto.MarcaId,
            cancellationToken);

        if (marca is null)
        {
            return ResultadoOperacaoDto.Falha(
                "A marca selecionada não foi encontrada.");
        }

        if (!marca.Ativo)
        {
            return ResultadoOperacaoDto.Falha(
                "Selecione uma marca ativa.");
        }

        var nome = dto.Nome
            .Trim()
            .ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(nome))
        {
            return ResultadoOperacaoDto.Falha(
                "Informe o nome do modelo.");
        }

        if (nome.Length > 100)
        {
            return ResultadoOperacaoDto.Falha(
                "O nome do modelo deve possuir no máximo 100 caracteres.");
        }

        var observacao = dto.Observacao
            .Trim()
            .ToUpperInvariant();

        if (observacao.Length > 500)
        {
            return ResultadoOperacaoDto.Falha(
                "A observação deve possuir no máximo 500 caracteres.");
        }

        var modeloDuplicado = await repository.ExisteNomeAsync(
            dto.MarcaId,
            nome,
            dto.Id,
            cancellationToken);

        if (modeloDuplicado)
        {
            return ResultadoOperacaoDto.Falha(
                "Já existe um modelo com este nome para a marca selecionada.");
        }

        if (dto.Id.HasValue)
        {
            var modelo = await repository.ObterAsync(
                dto.Id.Value,
                cancellationToken);

            if (modelo is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "O modelo selecionado não foi encontrado.");
            }

            modelo.MarcaId = dto.MarcaId;
            modelo.Nome = nome;
            modelo.Observacao = observacao;
            modelo.Ativo = dto.Ativo;
            modelo.DataAtualizacao = DateTime.Now;

            await repository.AtualizarAsync(
                modelo,
                cancellationToken);

            return ResultadoOperacaoDto.Ok(
                "Modelo atualizado com sucesso.");
        }

        var novoModelo = new ModeloEquipamento
        {
            MarcaId = dto.MarcaId,
            Nome = nome,
            Observacao = observacao,
            Ativo = true,
            DataCadastro = DateTime.Now
        };

        await repository.AdicionarAsync(
            novoModelo,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Modelo cadastrado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        var modelo = await repository.ObterAsync(
            id,
            cancellationToken);

        if (modelo is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O modelo selecionado não foi encontrado.");
        }

        if (ativo && !modelo.Marca.Ativo)
        {
            return ResultadoOperacaoDto.Falha(
                "Não é possível ativar o modelo porque a marca está inativa.");
        }

        modelo.Ativo = ativo;
        modelo.DataAtualizacao = DateTime.Now;

        await repository.AtualizarAsync(
            modelo,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            ativo
                ? "Modelo ativado com sucesso."
                : "Modelo inativado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> ExcluirAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var modelo = await repository.ObterAsync(
            id,
            cancellationToken);

        if (modelo is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O modelo selecionado não foi encontrado.");
        }

        await repository.ExcluirAsync(
            modelo,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Modelo excluído com sucesso.");
    }

    private static ModeloEquipamentoDto MapearDto(
        ModeloEquipamento modelo)
    {
        return new ModeloEquipamentoDto
        {
            Id = modelo.Id,
            MarcaId = modelo.MarcaId,
            Marca = modelo.Marca?.Nome ?? string.Empty,
            Nome = modelo.Nome,
            Observacao = modelo.Observacao ?? string.Empty,
            Ativo = modelo.Ativo,
            DataCadastro = modelo.DataCadastro,
            DataAtualizacao = modelo.DataAtualizacao
        };
    }
}