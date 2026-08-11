namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class ResultadoOperacaoDto
{
    public bool Sucesso { get; init; }

    public string Mensagem { get; init; } = string.Empty;

    public static ResultadoOperacaoDto Ok(string mensagem)
    {
        return new ResultadoOperacaoDto
        {
            Sucesso = true,
            Mensagem = mensagem
        };
    }

    public static ResultadoOperacaoDto Falha(string mensagem)
    {
        return new ResultadoOperacaoDto
        {
            Sucesso = false,
            Mensagem = mensagem
        };
    }
}
