using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class DashboardService : IDashboardService
{
    private const double AlturaMaximaGrafico = 145;

    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardDto> ObterAsync(
        int ano,
        int? mes = null,
        CancellationToken cancellationToken = default)
    {
        ValidarPeriodo(ano, mes);

        var inicio = mes.HasValue
            ? new DateTime(ano, mes.Value, 1)
            : new DateTime(ano, 1, 1);

        var fim = mes.HasValue
            ? inicio.AddMonths(1)
            : inicio.AddYears(1);

        var totalVendido = await _dashboardRepository.ObterTotalVendidoAsync(
            inicio,
            fim,
            cancellationToken);

        var totalComissao = await _dashboardRepository.ObterTotalComissaoAsync(
            inicio,
            fim,
            cancellationToken);

        var quantidadeRegistros = await _dashboardRepository.ContarLancamentosAsync(
            inicio,
            fim,
            cancellationToken: cancellationToken);

        var quantidadePagos = await _dashboardRepository.ContarLancamentosAsync(
            inicio,
            fim,
            StatusLancamento.Pago,
            cancellationToken);

        var quantidadePendentes = await _dashboardRepository.ContarLancamentosAsync(
            inicio,
            fim,
            StatusLancamento.Pendente,
            cancellationToken);

        var ultimosLancamentos = await _dashboardRepository.ObterUltimosLancamentosAsync(
            inicio,
            fim,
            8,
            cancellationToken);

        var lancamentosAno = await _dashboardRepository.ObterLancamentosDoAnoAsync(
            ano,
            cancellationToken);

        var resumoMensal = CriarResumoMensal(lancamentosAno);

        return new DashboardDto
        {
            TotalVendido = totalVendido,
            TotalComissao = totalComissao,
            QuantidadeRegistros = quantidadeRegistros,
            QuantidadePagos = quantidadePagos,
            QuantidadePendentes = quantidadePendentes,
            ResumoMensal = resumoMensal,
            UltimosLancamentos = ultimosLancamentos
                .Select(MapearLancamento)
                .ToList()
        };
    }

    private static IReadOnlyList<ResumoMensalDto> CriarResumoMensal(
        IReadOnlyList<Lancamento> lancamentos)
    {
        var resumos = Enumerable
            .Range(1, 12)
            .Select(mes =>
            {
                var registrosMes = lancamentos
                    .Where(lancamento => lancamento.DataVenda.Month == mes)
                    .ToList();

                return new ResumoMensalDto
                {
                    Mes = mes,
                    TotalVendido = registrosMes.Sum(lancamento =>
                        lancamento.ValorVenda),
                    TotalComissao = registrosMes.Sum(lancamento =>
                        lancamento.ValorComissao),
                    QuantidadeRegistros = registrosMes.Count
                };
            })
            .ToList();

        var maiorVenda = resumos.Max(resumo => resumo.TotalVendido);
        var maiorComissao = resumos.Max(resumo => resumo.TotalComissao);

        foreach (var resumo in resumos)
        {
            resumo.AlturaVendas = CalcularAltura(
                resumo.TotalVendido,
                maiorVenda);

            resumo.AlturaComissoes = CalcularAltura(
                resumo.TotalComissao,
                maiorComissao);
        }

        return resumos;
    }

    private static double CalcularAltura(decimal valor, decimal maiorValor)
    {
        if (valor <= 0 || maiorValor <= 0)
        {
            return 2;
        }

        return Math.Max(
            6,
            (double)(valor / maiorValor) * AlturaMaximaGrafico);
    }

    private static LancamentoDto MapearLancamento(Lancamento lancamento)
    {
        return new LancamentoDto
        {
            Id = lancamento.Id,
            DataVenda = lancamento.DataVenda,
            Cliente = lancamento.Cliente,
            CpfCnpjCliente = lancamento.CpfCnpjCliente ?? string.Empty,
            VendedorId = lancamento.VendedorId,
            VendedorNome = lancamento.Vendedor.Nome,
            ValorVenda = lancamento.ValorVenda,
            PercentualComissao = lancamento.PercentualComissao,
            ValorComissao = lancamento.ValorComissao,
            Status = lancamento.Status,
            Observacao = lancamento.Observacao ?? string.Empty
        };
    }

    private static void ValidarPeriodo(int ano, int? mes)
    {
        if (ano is < 2000 or > 2100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ano),
                "Informe um ano válido.");
        }

        if (mes.HasValue && mes.Value is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mes),
                "Informe um mês válido.");
        }
    }
}
