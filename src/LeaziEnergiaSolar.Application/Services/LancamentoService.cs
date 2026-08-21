using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Validators;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class LancamentoService : ILancamentoService
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly IVendedorRepository _vendedorRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public LancamentoService(
        ILancamentoRepository lancamentoRepository,
        IVendedorRepository vendedorRepository,
        IClienteRepository clienteRepository,
        IUsuarioRepository usuarioRepository)
    {
        _lancamentoRepository =
            lancamentoRepository;

        _vendedorRepository =
            vendedorRepository;

        _clienteRepository =
            clienteRepository;

        _usuarioRepository =
            usuarioRepository;
    }

    public async Task<IReadOnlyList<LancamentoDto>> ListarAsync(
        FiltroLancamentoDto filtro,
        CancellationToken cancellationToken = default)
    {
        var lancamentos =
            await _lancamentoRepository.ListarAsync(
                filtro.Pesquisa,
                filtro.DataInicial,
                filtro.DataFinal,
                filtro.VendedorId,
                filtro.Status,
                cancellationToken);

        return lancamentos
            .Select(Mapear)
            .ToList();
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarLancamentoDto lancamento,
        CancellationToken cancellationToken = default)
    {
        var erros =
            LancamentoValidator.Validar(
                lancamento);

        if (erros.Count > 0)
        {
            return ResultadoOperacaoDto.Falha(
                string.Join(
                    Environment.NewLine,
                    erros));
        }

        /*
         * VALIDAÇÃO DA DATA DE PAGAMENTO
         */

        if (lancamento.Status ==
            StatusLancamento.Pago)
        {
            if (!lancamento.DataPagamento.HasValue)
            {
                return ResultadoOperacaoDto.Falha(
                    "Informe a data do pagamento.");
            }

            if (lancamento.DataPagamento.Value.Date <
                lancamento.DataVenda.Date)
            {
                return ResultadoOperacaoDto.Falha(
                    "A data do pagamento não pode ser " +
                    "anterior à data da venda.");
            }

            if (lancamento.DataPagamento.Value.Date >
                DateTime.Today)
            {
                return ResultadoOperacaoDto.Falha(
                    "A data do pagamento não pode ser futura.");
            }
        }

        /*
         * VALIDAÇÃO DO VENDEDOR
         */

        var vendedor =
            await _vendedorRepository.ObterAsync(
                lancamento.VendedorId,
                cancellationToken);

        if (vendedor is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O vendedor selecionado não foi encontrado.");
        }

        if (!vendedor.Ativo &&
            !lancamento.Id.HasValue)
        {
            return ResultadoOperacaoDto.Falha(
                "Não é permitido criar lançamento " +
                "para vendedor inativo.");
        }

        /*
         * VALIDAÇÃO DO USUÁRIO
         */

        if (lancamento.UsuarioId.HasValue)
        {
            var usuario =
                await _usuarioRepository.ObterAsync(
                    lancamento.UsuarioId.Value,
                    cancellationToken);

            if (usuario is null ||
                !usuario.Ativo)
            {
                return ResultadoOperacaoDto.Falha(
                    "O usuário responsável pelo lançamento " +
                    "não está disponível.");
            }
        }

        /*
         * VALIDAÇÃO DO CLIENTE
         */

        if (lancamento.ClienteId.HasValue)
        {
            var clienteCadastro =
                await _clienteRepository.ObterAsync(
                    lancamento.ClienteId.Value,
                    cancellationToken);

            if (clienteCadastro is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "O cliente selecionado não foi encontrado.");
            }

            if (!clienteCadastro.Ativo &&
                !lancamento.Id.HasValue)
            {
                return ResultadoOperacaoDto.Falha(
                    "Não é permitido criar lançamento " +
                    "para cliente inativo.");
            }
        }

        /*
         * EDIÇÃO
         */

        if (lancamento.Id.HasValue)
        {
            var entidade =
                await _lancamentoRepository.ObterAsync(
                    lancamento.Id.Value,
                    cancellationToken);

            if (entidade is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "O lançamento selecionado não foi encontrado.");
            }

            AtualizarEntidade(
                entidade,
                lancamento,
                novoCadastro: false);

            await _lancamentoRepository.AtualizarAsync(
                entidade,
                cancellationToken);

            return ResultadoOperacaoDto.Ok(
                "Lançamento atualizado com sucesso.");
        }

        /*
         * NOVO CADASTRO
         */

        var novoLancamento =
            new Lancamento();

        AtualizarEntidade(
            novoLancamento,
            lancamento,
            novoCadastro: true);

        await _lancamentoRepository.AdicionarAsync(
            novoLancamento,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Lançamento cadastrado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        StatusLancamento status,
        CancellationToken cancellationToken = default)
    {
        var lancamento =
            await _lancamentoRepository.ObterAsync(
                id,
                cancellationToken);

        if (lancamento is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O lançamento selecionado não foi encontrado.");
        }

        var statusAnterior =
            lancamento.Status;

        lancamento.Status =
            status;

        /*
         * DATA AUTOMÁTICA AO ALTERAR DIRETAMENTE
         * O STATUS PELA LISTAGEM
         */

        if (status == StatusLancamento.Pago &&
            statusAnterior != StatusLancamento.Pago)
        {
            lancamento.DataPagamento =
                DateTime.Today;
        }
        else if (status ==
                 StatusLancamento.Pendente)
        {
            lancamento.DataPagamento =
                null;
        }

        /*
         * REGISTRA QUANDO O STATUS FOI ALTERADO
         */

        lancamento.DataAtualizacao =
            DateTime.Now;

        await _lancamentoRepository.AtualizarAsync(
            lancamento,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            status == StatusLancamento.Pago
                ? "Lançamento marcado como pago."
                : "Lançamento marcado como pendente.");
    }

    public async Task<ResultadoOperacaoDto> ExcluirAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var lancamento =
            await _lancamentoRepository.ObterAsync(
                id,
                cancellationToken);

        if (lancamento is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O lançamento selecionado não foi encontrado.");
        }

        /*
         * EXCLUSÃO FÍSICA
         *
         * O lançamento será removido definitivamente
         * do banco de dados.
         */

        await _lancamentoRepository.ExcluirAsync(
            lancamento,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Lançamento excluído com sucesso.");
    }

    public decimal CalcularComissao(
        decimal valorVenda,
        decimal percentualComissao)
    {
        if (valorVenda <= 0 ||
            percentualComissao <= 0)
        {
            return 0;
        }

        return Math.Round(
            valorVenda *
            percentualComissao /
            100,
            2,
            MidpointRounding.AwayFromZero);
    }

    private void AtualizarEntidade(
        Lancamento entidade,
        SalvarLancamentoDto lancamento,
        bool novoCadastro)
    {
        entidade.DataVenda =
            lancamento.DataVenda.Date;

        entidade.Cliente =
            lancamento.Cliente.Trim();

        entidade.CpfCnpjCliente =
            ValorNulo(
                DocumentoValidator.SomenteNumeros(
                    lancamento.CpfCnpjCliente));

        entidade.ClienteId =
            lancamento.ClienteId;

        entidade.UsuarioId =
            lancamento.UsuarioId;

        entidade.VendedorId =
            lancamento.VendedorId;

        entidade.ValorVenda =
            lancamento.ValorVenda;

        entidade.PercentualComissao =
            lancamento.PercentualComissao;

        entidade.ValorComissao =
            CalcularComissao(
                lancamento.ValorVenda,
                lancamento.PercentualComissao);

        entidade.Status =
            lancamento.Status;

        entidade.Observacao =
            ValorNulo(
                lancamento.Observacao);

        /*
         * DATA DE PAGAMENTO INFORMADA MANUALMENTE
         */

        if (lancamento.Status ==
            StatusLancamento.Pago)
        {
            entidade.DataPagamento =
                lancamento.DataPagamento?.Date
                ?? DateTime.Today;
        }
        else
        {
            entidade.DataPagamento =
                null;
        }

        /*
         * DATAS DE CADASTRO E ATUALIZAÇÃO
         */

        if (novoCadastro)
        {
            entidade.DataCadastro =
                DateTime.Now;

            entidade.DataAtualizacao =
                null;
        }
        else
        {
            entidade.DataAtualizacao =
                DateTime.Now;
        }
    }

    private static LancamentoDto Mapear(
        Lancamento lancamento)
    {
        return new LancamentoDto
        {
            Id =
                lancamento.Id,

            DataVenda =
                lancamento.DataVenda,

            Cliente =
                lancamento.Cliente,

            CpfCnpjCliente =
                lancamento.CpfCnpjCliente
                ?? string.Empty,

            ClienteId =
                lancamento.ClienteId,

            UsuarioId =
                lancamento.UsuarioId,

            VendedorId =
                lancamento.VendedorId,

            VendedorNome =
                lancamento.Vendedor.Nome,

            ValorVenda =
                lancamento.ValorVenda,

            PercentualComissao =
                lancamento.PercentualComissao,

            ValorComissao =
                lancamento.ValorComissao,

            Status =
                lancamento.Status,

            DataPagamento =
                lancamento.DataPagamento,

            DataCadastro =
                lancamento.DataCadastro,

            DataAtualizacao =
                lancamento.DataAtualizacao,

            Observacao =
                lancamento.Observacao
                ?? string.Empty
        };
    }

    private static string? ValorNulo(
        string? valor)
    {
        return string.IsNullOrWhiteSpace(
                valor)
            ? null
            : valor.Trim();
    }
}