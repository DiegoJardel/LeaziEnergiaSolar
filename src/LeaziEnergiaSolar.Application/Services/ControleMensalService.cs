using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class ControleMensalService : IControleMensalService
{
    private readonly ILancamentoService _lancamentoService;

    public ControleMensalService(ILancamentoService lancamentoService)
    {
        _lancamentoService = lancamentoService;
    }

    public async Task<ControleMensalDto> ObterAsync(
        FiltroControleMensalDto filtro,
        CancellationToken cancellationToken = default)
    {
        ValidarFiltro(filtro);

        var dataInicial = new DateTime(
            filtro.Ano,
            filtro.Mes,
            1);

        var dataFinal = dataInicial
            .AddMonths(1)
            .AddDays(-1);

        var lancamentos = await _lancamentoService.ListarAsync(
            new FiltroLancamentoDto
            {
                Pesquisa = filtro.Pesquisa,
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                VendedorId = filtro.VendedorId,
                Status = filtro.Status
            },
            cancellationToken);

        return new ControleMensalDto
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
            Lancamentos = lancamentos
        };
    }

    private static void ValidarFiltro(FiltroControleMensalDto filtro)
    {
        if (filtro.Mes is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(filtro.Mes),
                "Informe um mês válido.");
        }

        if (filtro.Ano is < 2000 or > 2100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(filtro.Ano),
                "Informe um ano válido.");
        }
    }
}
