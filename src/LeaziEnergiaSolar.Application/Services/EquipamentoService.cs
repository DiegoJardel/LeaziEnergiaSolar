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
    private readonly IModeloEquipamentoRepository _modeloRepository;
    private readonly IUnidadeMedidaRepository _unidadeRepository;
    private readonly IFornecedorRepository _fornecedorRepository;

    public EquipamentoService(
        IEquipamentoRepository repository,
        ICategoriaEquipamentoRepository categoriaRepository,
        IMarcaRepository marcaRepository,
        IModeloEquipamentoRepository modeloRepository,
        IUnidadeMedidaRepository unidadeRepository,
        IFornecedorRepository fornecedorRepository)
    {
        _repository =
            repository;

        _categoriaRepository =
            categoriaRepository;

        _marcaRepository =
            marcaRepository;

        _modeloRepository =
            modeloRepository;

        _unidadeRepository =
            unidadeRepository;

        _fornecedorRepository =
            fornecedorRepository;
    }

    public async Task<IReadOnlyList<EquipamentoDto>> ListarAsync(
        string? pesquisa = null,
        int? categoriaId = null,
        int? marcaId = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default)
    {
        var itens =
            await _repository.ListarAsync(
                pesquisa,
                categoriaId,
                marcaId,
                ativo,
                cancellationToken);

        return itens
            .Select(Mapear)
            .ToList();
    }

    public async Task<EquipamentoDto?> ObterAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var item =
            await _repository.ObterAsync(
                id,
                cancellationToken);

        return item is null
            ? null
            : Mapear(item);
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarEquipamentoDto dto,
        CancellationToken cancellationToken = default)
    {
        dto =
            Normalizar(dto);

        var erros =
            EquipamentoValidator.Validar(dto);

        if (erros.Count > 0)
        {
            return ResultadoOperacaoDto.Falha(
                string.Join(
                    Environment.NewLine,
                    erros));
        }

        Equipamento? existente = null;

        if (dto.Id.HasValue)
        {
            existente =
                await _repository.ObterAsync(
                    dto.Id.Value,
                    cancellationToken);

            if (existente is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "O equipamento selecionado não foi encontrado.");
            }
        }

        /*
         * CATEGORIA
         */

        var categoria =
            await _categoriaRepository.ObterAsync(
                dto.CategoriaEquipamentoId,
                cancellationToken);

        if (categoria is null)
        {
            return ResultadoOperacaoDto.Falha(
                "A categoria selecionada não foi encontrada.");
        }

        var categoriaFoiAlterada =
            existente is null ||
            existente.CategoriaEquipamentoId !=
            dto.CategoriaEquipamentoId;

        if (!categoria.Ativo &&
            categoriaFoiAlterada)
        {
            return ResultadoOperacaoDto.Falha(
                "Selecione uma categoria ativa válida.");
        }

        /*
         * MARCA
         */

        var marca =
            await _marcaRepository.ObterAsync(
                dto.MarcaId,
                cancellationToken);

        if (marca is null)
        {
            return ResultadoOperacaoDto.Falha(
                "A marca selecionada não foi encontrada.");
        }

        var marcaFoiAlterada =
            existente is null ||
            existente.MarcaId !=
            dto.MarcaId;

        if (!marca.Ativo &&
            marcaFoiAlterada)
        {
            return ResultadoOperacaoDto.Falha(
                "Selecione uma marca ativa válida.");
        }

        /*
         * MODELO
         */

        var modeloExiste =
            await _modeloRepository.ExisteNomeAsync(
                dto.MarcaId,
                dto.Modelo,
                null,
                cancellationToken);

        if (!modeloExiste)
        {
            return ResultadoOperacaoDto.Falha(
                "O modelo selecionado não pertence à marca informada.");
        }

        var modelosDaMarca =
            await _modeloRepository.ListarAsync(
                dto.MarcaId,
                dto.Modelo,
                null,
                cancellationToken);

        var modeloSelecionado =
            modelosDaMarca.FirstOrDefault(
                x => string.Equals(
                    x.Nome,
                    dto.Modelo,
                    StringComparison.OrdinalIgnoreCase));

        if (modeloSelecionado is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O modelo selecionado não foi encontrado.");
        }

        var modeloFoiAlterado =
            existente is null ||
            existente.MarcaId !=
            dto.MarcaId ||
            !string.Equals(
                existente.Modelo,
                dto.Modelo,
                StringComparison.OrdinalIgnoreCase);

        if (!modeloSelecionado.Ativo &&
            modeloFoiAlterado)
        {
            return ResultadoOperacaoDto.Falha(
                "Selecione um modelo ativo válido.");
        }

        /*
         * UNIDADE DE MEDIDA
         */

        var unidade =
            await _unidadeRepository.ObterAsync(
                dto.UnidadeMedidaId,
                cancellationToken);

        if (unidade is null)
        {
            return ResultadoOperacaoDto.Falha(
                "A unidade de medida selecionada não foi encontrada.");
        }

        var unidadeFoiAlterada =
            existente is null ||
            existente.UnidadeMedidaId !=
            dto.UnidadeMedidaId;

        if (!unidade.Ativo &&
            unidadeFoiAlterada)
        {
            return ResultadoOperacaoDto.Falha(
                "Selecione uma unidade de medida ativa válida.");
        }

        /*
         * FORNECEDOR
         */

        var fornecedor =
            await _fornecedorRepository.ObterAsync(
                dto.FornecedorId,
                cancellationToken);

        if (fornecedor is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O fornecedor selecionado não foi encontrado.");
        }

        var fornecedorFoiAlterado =
            existente is null ||
            existente.FornecedorId !=
            dto.FornecedorId;

        if (!fornecedor.Ativo &&
            fornecedorFoiAlterado)
        {
            return ResultadoOperacaoDto.Falha(
                "Selecione um fornecedor ativo válido.");
        }

        /*
         * DUPLICIDADE
         */

        var duplicado =
            await _repository.ExisteDuplicadoAsync(
                dto.CategoriaEquipamentoId,
                dto.MarcaId,
                dto.Modelo,
                dto.Id,
                cancellationToken);

        if (duplicado)
        {
            return ResultadoOperacaoDto.Falha(
                "Já existe um equipamento com a mesma categoria, " +
                "marca e modelo.");
        }

        /*
         * EDIÇÃO
         */

        if (dto.Id.HasValue)
        {
            existente!.CategoriaEquipamentoId =
                dto.CategoriaEquipamentoId;

            existente.MarcaId =
                dto.MarcaId;

            existente.Modelo =
                dto.Modelo;

            existente.UnidadeMedidaId =
                dto.UnidadeMedidaId;

            existente.FornecedorId =
                dto.FornecedorId;

            existente.Observacao =
                ValorNulo(dto.Observacao);

            existente.Ativo =
                dto.Ativo;

            existente.DataAtualizacao =
                DateTime.Now;

            await _repository.AtualizarAsync(
                existente,
                cancellationToken);

            return ResultadoOperacaoDto.Ok(
                "Equipamento atualizado com sucesso.");
        }

        /*
         * NOVO CADASTRO
         */

        var novo =
            new Equipamento
            {
                CategoriaEquipamentoId =
                    dto.CategoriaEquipamentoId,

                MarcaId =
                    dto.MarcaId,

                Modelo =
                    dto.Modelo,

                UnidadeMedidaId =
                    dto.UnidadeMedidaId,

                FornecedorId =
                    dto.FornecedorId,

                Observacao =
                    ValorNulo(dto.Observacao),

                Ativo =
                    dto.Ativo,

                DataCadastro =
                    DateTime.Now
            };

        await _repository.AdicionarAsync(
            novo,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Equipamento cadastrado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        var entidade =
            await _repository.ObterAsync(
                id,
                cancellationToken);

        if (entidade is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O equipamento selecionado não foi encontrado.");
        }

        /*
         * SOMENTE AO REATIVAR
         */

        if (ativo)
        {
            var categoria =
                await _categoriaRepository.ObterAsync(
                    entidade.CategoriaEquipamentoId,
                    cancellationToken);

            if (categoria is null ||
                !categoria.Ativo)
            {
                return ResultadoOperacaoDto.Falha(
                    "Não é possível reativar o equipamento porque " +
                    "a categoria está inativa.");
            }

            var marca =
                await _marcaRepository.ObterAsync(
                    entidade.MarcaId,
                    cancellationToken);

            if (marca is null ||
                !marca.Ativo)
            {
                return ResultadoOperacaoDto.Falha(
                    "Não é possível reativar o equipamento porque " +
                    "a marca está inativa.");
            }

            var modelos =
                await _modeloRepository.ListarAsync(
                    entidade.MarcaId,
                    entidade.Modelo,
                    null,
                    cancellationToken);

            var modelo =
                modelos.FirstOrDefault(
                    x => string.Equals(
                        x.Nome,
                        entidade.Modelo,
                        StringComparison.OrdinalIgnoreCase));

            if (modelo is null ||
                !modelo.Ativo)
            {
                return ResultadoOperacaoDto.Falha(
                    "Não é possível reativar o equipamento porque " +
                    "o modelo está inativo.");
            }

            var unidade =
                await _unidadeRepository.ObterAsync(
                    entidade.UnidadeMedidaId,
                    cancellationToken);

            if (unidade is null ||
                !unidade.Ativo)
            {
                return ResultadoOperacaoDto.Falha(
                    "Não é possível reativar o equipamento porque " +
                    "a unidade de medida está inativa.");
            }

            var fornecedor =
                await _fornecedorRepository.ObterAsync(
                    entidade.FornecedorId,
                    cancellationToken);

            if (fornecedor is null ||
                !fornecedor.Ativo)
            {
                return ResultadoOperacaoDto.Falha(
                    "Não é possível reativar o equipamento porque " +
                    "o fornecedor está inativo.");
            }
        }

        entidade.Ativo =
            ativo;

        entidade.DataAtualizacao =
            DateTime.Now;

        await _repository.AtualizarAsync(
            entidade,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            ativo
                ? "Equipamento reativado com sucesso."
                : "Equipamento inativado com sucesso.");
    }

    /*
     * MAPEAMENTO
     */

    private static EquipamentoDto Mapear(
        Equipamento entidade)
    {
        return new EquipamentoDto
        {
            Id =
                entidade.Id,

            CategoriaEquipamentoId =
                entidade.CategoriaEquipamentoId,

            Categoria =
                entidade.CategoriaEquipamento.Descricao,

            MarcaId =
                entidade.MarcaId,

            Marca =
                entidade.Marca.Nome,

            Modelo =
                entidade.Modelo,

            UnidadeMedidaId =
                entidade.UnidadeMedidaId,

            UnidadeMedida =
                $"{entidade.UnidadeMedida.Sigla} - " +
                entidade.UnidadeMedida.Descricao,

            FornecedorId =
                entidade.FornecedorId,

            Fornecedor =
                entidade.Fornecedor.NomeRazaoSocial,

            Observacao =
                entidade.Observacao ??
                string.Empty,

            Ativo =
                entidade.Ativo,

            DataCadastro =
                entidade.DataCadastro,

            DataAtualizacao =
                entidade.DataAtualizacao
        };
    }

    /*
     * NORMALIZAÇÃO
     */

    private static SalvarEquipamentoDto Normalizar(
        SalvarEquipamentoDto dto)
    {
        return new SalvarEquipamentoDto
        {
            Id =
                dto.Id,

            CategoriaEquipamentoId =
                dto.CategoriaEquipamentoId,

            MarcaId =
                dto.MarcaId,

            Modelo =
                NormalizarTexto(dto.Modelo),

            UnidadeMedidaId =
                dto.UnidadeMedidaId,

            FornecedorId =
                dto.FornecedorId,

            Observacao =
                NormalizarTexto(dto.Observacao),

            Ativo =
                dto.Ativo
        };
    }

    private static string NormalizarTexto(
        string? valor)
    {
        return string.Join(
                " ",
                (valor ?? string.Empty)
                    .Trim()
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();
    }

    private static string? ValorNulo(
        string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor;
    }
}