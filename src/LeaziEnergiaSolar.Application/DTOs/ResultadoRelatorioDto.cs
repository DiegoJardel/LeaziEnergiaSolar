namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class ResultadoRelatorioDto
{
    public bool Sucesso { get; init; }

    public string Mensagem { get; init; } =
        string.Empty;

    public string CaminhoArquivo { get; init; } =
        string.Empty;

    public static ResultadoRelatorioDto Ok(
        string caminhoArquivo)
    {
        return new ResultadoRelatorioDto
        {
            Sucesso =
                true,

            Mensagem =
                "PDF gerado com sucesso.",

            CaminhoArquivo =
                caminhoArquivo
        };
    }

    public static ResultadoRelatorioDto Falha(
        string mensagem)
    {
        return new ResultadoRelatorioDto
        {
            Sucesso =
                false,

            Mensagem =
                mensagem,

            CaminhoArquivo =
                string.Empty
        };
    }
}