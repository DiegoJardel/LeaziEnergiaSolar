using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Validators;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class UnidadeMedidaService
    : IUnidadeMedidaService
{
    private readonly IUnidadeMedidaRepository
        _repo;

    private readonly IEquipamentoRepository
        _equipamentoRepository;

    public UnidadeMedidaService(
        IUnidadeMedidaRepository repo,
        IEquipamentoRepository equipamentoRepository)
    {
        _repo =
            repo
            ?? throw new ArgumentNullException(
                nameof(repo));

        _equipamentoRepository =
            equipamentoRepository
            ?? throw new ArgumentNullException(
                nameof(equipamentoRepository));
    }

    public async Task<IReadOnlyList<UnidadeMedidaDto>> ListarAsync(
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default)
    {
        var unidades =
            await _repo.ListarAsync(
                pesquisa,
                ativo,
                cancellationToken);

        return unidades
            .Select(
                Mapear)
            .ToList();
    }

    public async Task<UnidadeMedidaDto?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidade =
            await _repo.ObterAsync(
                id,
                cancellationToken);

        return entidade is null
            ? null
            : Mapear(
                entidade);
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarUnidadeMedidaDto dto,
        CancellationToken cancellationToken = default)
    {
        dto =
            new SalvarUnidadeMedidaDto
            {
                Id =
                    dto.Id,

                Sigla =
                    EquipamentoValidator
                        .Texto(
                            dto.Sigla)
                        .ToUpperInvariant(),

                Descricao =
                    EquipamentoValidator
                        .Texto(
                            dto.Descricao)
                        .ToUpperInvariant(),

                PermiteQuantidadeDecimal =
                    dto.PermiteQuantidadeDecimal,

                Ativo =
                    dto.Ativo
            };

        var erros =
            UnidadeMedidaValidator.Validar(
                dto);

        if (erros.Count > 0)
        {
            return ResultadoOperacaoDto.Falha(
                string.Join(
                    Environment.NewLine,
                    erros));
        }

        if (await _repo.ExisteSiglaAsync(
                dto.Sigla,
                dto.Id,
                cancellationToken))
        {
            return ResultadoOperacaoDto.Falha(
                "Já existe uma unidade de medida " +
                "com essa sigla.");
        }

        if (dto.Id.HasValue)
        {
            var entidade =
                await _repo.ObterAsync(
                    dto.Id.Value,
                    cancellationToken);

            if (entidade is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "A unidade de medida selecionada " +
                    "não foi encontrada.");
            }

            entidade.Sigla =
                dto.Sigla;

            entidade.Descricao =
                dto.Descricao;

            entidade.PermiteQuantidadeDecimal =
                dto.PermiteQuantidadeDecimal;

            entidade.Ativo =
                dto.Ativo;

            entidade.DataAtualizacao =
                DateTime.Now;

            await _repo.AtualizarAsync(
                entidade,
                cancellationToken);

            return ResultadoOperacaoDto.Ok(
                "Unidade de medida atualizada com sucesso.");
        }

        await _repo.AdicionarAsync(
            new UnidadeMedida
            {
                Sigla =
                    dto.Sigla,

                Descricao =
                    dto.Descricao,

                PermiteQuantidadeDecimal =
                    dto.PermiteQuantidadeDecimal,

                Ativo =
                    true,

                DataCadastro =
                    DateTime.Now
            },
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Unidade de medida cadastrada com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        var entidade =
            await _repo.ObterAsync(
                id,
                cancellationToken);

        if (entidade is null)
        {
            return ResultadoOperacaoDto.Falha(
                "A unidade de medida selecionada " +
                "não foi encontrada.");
        }

        entidade.Ativo =
            ativo;

        entidade.DataAtualizacao =
            DateTime.Now;

        await _repo.AtualizarAsync(
            entidade,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            ativo
                ? "Unidade de medida reativada com sucesso."
                : "Unidade de medida inativada com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> ExcluirAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidade =
            await _repo.ObterAsync(
                id,
                cancellationToken);

        if (entidade is null)
        {
            return ResultadoOperacaoDto.Falha(
                "A unidade de medida selecionada " +
                "não foi encontrada.");
        }

        var possuiEquipamentos =
            await _equipamentoRepository
                .ExistePorUnidadeAsync(
                    id,
                    cancellationToken);

        if (possuiEquipamentos)
        {
            return ResultadoOperacaoDto.Falha(
                "Não é possível excluir a unidade de medida " +
                "porque existem equipamentos vinculados.");
        }

        await _repo.ExcluirAsync(
            entidade,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Unidade de medida excluída com sucesso.");
    }

    private static UnidadeMedidaDto Mapear(
        UnidadeMedida entidade) =>
        new()
        {
            Id =
                entidade.Id,

            Sigla =
                entidade.Sigla,

            Descricao =
                entidade.Descricao,

            PermiteQuantidadeDecimal =
                entidade.PermiteQuantidadeDecimal,

            Ativo =
                entidade.Ativo,

            DataCadastro =
                entidade.DataCadastro,

            DataAtualizacao =
                entidade.DataAtualizacao
        };
}