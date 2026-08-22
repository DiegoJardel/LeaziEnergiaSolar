using System.IO;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Domain.Interfaces;
using QuestPDF.Fluent;

namespace LeaziEnergiaSolar.Infrastructure.Reports;

public sealed class RelatorioComissaoService
    : IRelatorioComissaoService
{
    private readonly ILancamentoRepository
        _lancamentoRepository;

    public RelatorioComissaoService(
        ILancamentoRepository lancamentoRepository)
    {
        _lancamentoRepository =
            lancamentoRepository
            ?? throw new ArgumentNullException(
                nameof(lancamentoRepository));
    }

    public async Task<ResultadoRelatorioDto> GerarPdfAsync(
        FiltroRelatorioComissaoDto filtro,
        string caminhoArquivo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validacao =
                Validar(
                    filtro,
                    caminhoArquivo);

            if (!string.IsNullOrWhiteSpace(
                    validacao))
            {
                return ResultadoRelatorioDto.Falha(
                    validacao);
            }

            var status =
                ObterStatusObrigatorio(
                    filtro);

            var lancamentos =
                await _lancamentoRepository.ListarAsync(
                    filtro.Pesquisa,
                    filtro.DataVendaInicial,
                    filtro.DataVendaFinal,
                    filtro.VendedorId,
                    status,
                    cancellationToken);

            var itens =
                lancamentos
                    .Where(
                        item =>
                            !filtro.ClienteId.HasValue ||
                            item.ClienteId ==
                            filtro.ClienteId.Value)
                    .Where(
                        item =>
                            !filtro.DataPagamentoInicial.HasValue ||
                            item.DataPagamento.HasValue &&
                            item.DataPagamento.Value.Date >=
                            filtro.DataPagamentoInicial.Value.Date)
                    .Where(
                        item =>
                            !filtro.DataPagamentoFinal.HasValue ||
                            item.DataPagamento.HasValue &&
                            item.DataPagamento.Value.Date <=
                            filtro.DataPagamentoFinal.Value.Date)
                    .Select(
                        Mapear)
                    .ToList();

            if (itens.Count == 0)
            {
                return ResultadoRelatorioDto.Falha(
                    "Nenhum lançamento foi encontrado " +
                    "para os filtros informados.");
            }

            var diretorio =
                Path.GetDirectoryName(
                    caminhoArquivo);

            if (!string.IsNullOrWhiteSpace(
                    diretorio))
            {
                Directory.CreateDirectory(
                    diretorio);
            }

            var logo =
                CarregarLogo();

            switch (filtro.TipoRelatorio)
            {
                case TipoRelatorioComissao
                    .GeralComissoes:
                    {
                        var documento =
                            new RelatorioGeralComissoesDocument(
                                itens,
                                filtro,
                                logo);

                        documento.GeneratePdf(
                            caminhoArquivo);

                        break;
                    }

                case TipoRelatorioComissao
                    .ComissoesAPagar:
                    {
                        var documento =
                            new RelatorioComissoesAPagarDocument(
                                itens,
                                filtro,
                                logo);

                        documento.GeneratePdf(
                            caminhoArquivo);

                        break;
                    }

                case TipoRelatorioComissao
                    .ComissoesPagas:
                    {
                        var documento =
                            new RelatorioComissoesPagasDocument(
                                itens,
                                filtro,
                                logo);

                        documento.GeneratePdf(
                            caminhoArquivo);

                        break;
                    }

                case TipoRelatorioComissao
                    .ComissoesPorVendedor:
                    {
                        var documento =
                            new RelatorioComissoesPorVendedorDocument(
                                itens,
                                filtro,
                                logo);

                        documento.GeneratePdf(
                            caminhoArquivo);

                        break;
                    }

                case TipoRelatorioComissao
                    .ExtratoIndividualVendedor:
                    {
                        var documento =
                            new RelatorioExtratoIndividualVendedorDocument(
                                itens,
                                filtro,
                                logo);

                        documento.GeneratePdf(
                            caminhoArquivo);

                        break;
                    }

                case TipoRelatorioComissao
                    .ExecutivoVendasComissoes:
                    {
                        var documento =
                            new RelatorioExecutivoVendasComissoesDocument(
                                itens,
                                filtro,
                                logo);

                        documento.GeneratePdf(
                            caminhoArquivo);

                        break;
                    }

                default:
                    {
                        return ResultadoRelatorioDto.Falha(
                            "O tipo de relatório informado " +
                            "não é válido.");
                    }
            }

            if (!File.Exists(
                    caminhoArquivo))
            {
                return ResultadoRelatorioDto.Falha(
                    "O arquivo PDF não foi criado " +
                    "no local informado.");
            }

            return ResultadoRelatorioDto.Ok(
                caminhoArquivo);
        }
        catch (OperationCanceledException)
        {
            return ResultadoRelatorioDto.Falha(
                "A geração do relatório foi cancelada.");
        }
        catch (Exception exception)
        {
            var mensagemRaiz =
                exception
                    .GetBaseException()
                    .Message;

            return ResultadoRelatorioDto.Falha(
                "Não foi possível gerar o PDF. " +
                mensagemRaiz);
        }
    }

    private static string? Validar(
        FiltroRelatorioComissaoDto filtro,
        string caminhoArquivo)
    {
        if (filtro is null)
        {
            return "Os filtros do relatório " +
                   "não foram informados.";
        }

        if (string.IsNullOrWhiteSpace(
                caminhoArquivo))
        {
            return "Informe o local para salvar o PDF.";
        }

        if (!string.Equals(
                Path.GetExtension(
                    caminhoArquivo),
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            return "O arquivo do relatório deve possuir " +
                   "a extensão .pdf.";
        }

        if (filtro.DataVendaInicial.HasValue &&
            filtro.DataVendaFinal.HasValue &&
            filtro.DataVendaInicial.Value.Date >
            filtro.DataVendaFinal.Value.Date)
        {
            return "A data inicial da venda não pode ser " +
                   "maior que a data final.";
        }

        if (filtro.DataVendaFinal.HasValue &&
            filtro.DataVendaFinal.Value.Date >
            DateTime.Today)
        {
            return "A data final da venda não pode ser futura.";
        }

        if (filtro.DataPagamentoInicial.HasValue &&
            filtro.DataPagamentoFinal.HasValue &&
            filtro.DataPagamentoInicial.Value.Date >
            filtro.DataPagamentoFinal.Value.Date)
        {
            return "A data inicial do pagamento não pode ser " +
                   "maior que a data final.";
        }

        if (filtro.DataPagamentoFinal.HasValue &&
            filtro.DataPagamentoFinal.Value.Date >
            DateTime.Today)
        {
            return "A data final do pagamento não pode ser futura.";
        }

        if (filtro.TipoRelatorio ==
                TipoRelatorioComissao
                    .ExtratoIndividualVendedor &&
            !filtro.VendedorId.HasValue)
        {
            return "Selecione o vendedor para gerar " +
                   "o extrato individual.";
        }

        return null;
    }

    private static StatusLancamento? ObterStatusObrigatorio(
        FiltroRelatorioComissaoDto filtro)
    {
        return filtro.TipoRelatorio switch
        {
            TipoRelatorioComissao
                .ComissoesAPagar =>
                StatusLancamento.Pendente,

            TipoRelatorioComissao
                .ComissoesPagas =>
                StatusLancamento.Pago,

            _ =>
                filtro.Status
        };
    }

    private static LancamentoDto Mapear(
        Lancamento item)
    {
        return new LancamentoDto
        {
            Id =
                item.Id,

            DataVenda =
                item.DataVenda,

            Cliente =
                item.Cliente,

            CpfCnpjCliente =
                item.CpfCnpjCliente
                ?? string.Empty,

            ClienteId =
                item.ClienteId,

            UsuarioId =
                item.UsuarioId,

            VendedorId =
                item.VendedorId,

            VendedorNome =
                item.Vendedor?.Nome
                ?? string.Empty,

            ValorVenda =
                item.ValorVenda,

            PercentualComissao =
                item.PercentualComissao,

            ValorComissao =
                item.ValorComissao,

            Status =
                item.Status,

            DataPagamento =
                item.DataPagamento,

            DataCadastro =
                item.DataCadastro,

            DataAtualizacao =
                item.DataAtualizacao,

            Observacao =
                item.Observacao
                ?? string.Empty
        };
    }

    private static byte[]? CarregarLogo()
    {
        var caminhos =
            new[]
            {
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Assets",
                    "Images",
                    "logo-leazi.png"),

                Path.Combine(
                    AppContext.BaseDirectory,
                    "Assets",
                    "Images",
                    "logo.png"),

                Path.Combine(
                    AppContext.BaseDirectory,
                    "Assets",
                    "logo-leazi.png"),

                Path.Combine(
                    AppContext.BaseDirectory,
                    "Assets",
                    "logo.png")
            };

        var caminho =
            caminhos.FirstOrDefault(
                File.Exists);

        if (string.IsNullOrWhiteSpace(
                caminho))
        {
            return null;
        }

        return File.ReadAllBytes(
            caminho);
    }
}
