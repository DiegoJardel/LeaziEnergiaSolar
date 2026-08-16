using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Validators;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class MarcaService : IMarcaService
{
    private readonly IMarcaRepository _repo;

    public MarcaService(
        IMarcaRepository repo) =>
        _repo = repo;

    public async Task<IReadOnlyList<MarcaDto>> ListarAsync(
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default) =>
        (await _repo.ListarAsync(
            pesquisa,
            ativo,
            cancellationToken))
        .Select(Mapear)
        .ToList();

    public async Task<MarcaDto?> ObterAsync(
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
        SalvarMarcaDto dto,
        CancellationToken cancellationToken = default)
    {
        dto = new SalvarMarcaDto
        {
            Id = dto.Id,
            Nome = EquipamentoValidator
                .Texto(dto.Nome)
                .ToUpperInvariant(),
            Observacao = EquipamentoValidator.Texto(dto.Observacao),
            Ativo = dto.Ativo
        };

        var erros = MarcaValidator.Validar(dto);
        if (erros.Count > 0)
        {
            return ResultadoOperacaoDto.Falha(
                string.Join(Environment.NewLine, erros));
        }

        if (await _repo.ExisteNomeAsync(
                dto.Nome,
                dto.Id,
                cancellationToken))
        {
            return ResultadoOperacaoDto.Falha(
                "Já existe uma marca com esse nome.");
        }

        if (dto.Id.HasValue)
        {
            var entidade = await _repo.ObterAsync(
                dto.Id.Value,
                cancellationToken);

            if (entidade is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "A marca selecionada não foi encontrada.");
            }

            entidade.Nome = dto.Nome;
            entidade.Observacao = string.IsNullOrWhiteSpace(dto.Observacao)
                ? null
                : dto.Observacao;
            entidade.Ativo = dto.Ativo;
            entidade.DataAtualizacao = DateTime.Now;

            await _repo.AtualizarAsync(
                entidade,
                cancellationToken);

            return ResultadoOperacaoDto.Ok(
                "Marca atualizada com sucesso.");
        }

        await _repo.AdicionarAsync(
            new Marca
            {
                Nome = dto.Nome,
                Observacao = string.IsNullOrWhiteSpace(dto.Observacao)
                    ? null
                    : dto.Observacao,
                Ativo = true,
                DataCadastro = DateTime.Now
            },
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Marca cadastrada com sucesso.");
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
                "A marca selecionada não foi encontrada.");
        }

        entidade.Ativo = ativo;
        entidade.DataAtualizacao = DateTime.Now;

        await _repo.AtualizarAsync(
            entidade,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            ativo
                ? "Marca reativada com sucesso."
                : "Marca inativada com sucesso.");
    }

    private static MarcaDto Mapear(
        Marca entidade) =>
        new()
        {
            Id = entidade.Id,
            Nome = entidade.Nome,
            Observacao = entidade.Observacao ?? string.Empty,
            Ativo = entidade.Ativo,
            DataCadastro = entidade.DataCadastro,
            DataAtualizacao = entidade.DataAtualizacao
        };
}
