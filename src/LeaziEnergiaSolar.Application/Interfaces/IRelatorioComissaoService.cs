using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IRelatorioComissaoService
{
    Task<ResultadoRelatorioDto> GerarPdfAsync(
        FiltroRelatorioComissaoDto filtro,
        string caminhoArquivo,
        CancellationToken cancellationToken = default);
}