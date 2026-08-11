using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class ControleAnualService : IControleAnualService
{
    private const double AlturaMaximaGrafico = 150;

    private readonly ILancamentoService _lancamentoService;

    public ControleAnualService(ILancamentoService lancamentoService)
    {
        _lancamentoService = lancamentoService;
    }

    public async Task<ControleAnualDto> ObterAsync(
        FiltroControleAnualDto filtro,
        CancellationToken cancellationToken = default)
    {
        ValidarFiltro(filtro);

        var dataInicial = new DateTime(filtro.Ano, 1, 1);
        var dataFinal = dataInicial.AddYears(1).AddDays(-1);

        var lancamentos = await _lancamentoService.ListarAsync(
            new FiltroLancamentoDto
            {
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                VendedorId = filtro.VendedorId
            },
            cancellationToken);

        var meses = CriarResumoMensal(lancamentos);

        return new ControleAnualDto
        {
            TotalVendido = lancamentos.Sum(lancamento =>
                lancamento.ValorVenda),
            TotalComissao = lancamentos.Sum(lancamento =>
                lancamento.ValorComissao),
            QuantidadeRegistros = lancamentos.Count,
            QuantidadePagos = lancamentos.Count(lancamento =>
                lancamento.Status == StatusLancamento.Pago),
            QuantidadePendentes = lancamentos.Count(lancamento =>
                lancamento.Status == StatusLancamento.Pendente),
            Meses = meses
        };
    }

    private static IReadOnlyList<ResumoAnualMesDto> CriarResumoMensal(
        IReadOnlyList<LancamentoDto> lancamentos)
    {
        var meses = Enumerable
            .Range(1, 12)
            .Select(mes =>
            {
                var registrosMes = lancamentos
                    .Where(lancamento => lancamento.DataVenda.Month == mes)
                    .ToList();

                return new ResumoAnualMesDto
                {
                    Mes = mes,
                    TotalVendido = registrosMes.Sum(lancamento =>
                        lancamento.ValorVenda),
                    TotalComissao = registrosMes.Sum(lancamento =>
                        lancamento.ValorComissao),
                    QuantidadeRegistros = registrosMes.Count,
                    QuantidadePagos = registrosMes.Count(lancamento =>
                        lancamento.Status == StatusLancamento.Pago),
                    QuantidadePendentes = registrosMes.Count(lancamento =>
                        lancamento.Status == StatusLancamento.Pendente)
                };
            })
            .ToList();

        var maiorVenda = meses.Max(mes => mes.TotalVendido);
        var maiorComissao = meses.Max(mes => mes.TotalComissao);

        foreach (var mes in meses)
        {
            mes.AlturaVendas = CalcularAltura(
                mes.TotalVendido,
                maiorVenda);

            mes.AlturaComissoes = CalcularAltura(
                mes.TotalComissao,
                maiorComissao);
        }

        return meses;
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

    private static void ValidarFiltro(FiltroControleAnualDto filtro)
    {
        if (filtro.Ano is < 2000 or > 2100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(filtro.Ano),
                "Informe um ano válido.");
        }
    }
}
