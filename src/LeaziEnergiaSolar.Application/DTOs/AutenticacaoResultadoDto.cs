namespace LeaziEnergiaSolar.Application.DTOs;

public sealed class AutenticacaoResultadoDto
{
    public bool Sucesso { get; init; }

    public string Mensagem { get; init; } = string.Empty;

    public UsuarioAutenticadoDto? Usuario { get; init; }

    public static AutenticacaoResultadoDto Falha(string mensagem)
    {
        return new AutenticacaoResultadoDto
        {
            Sucesso = false,
            Mensagem = mensagem
        };
    }

    public static AutenticacaoResultadoDto Ok(UsuarioAutenticadoDto usuario)
    {
        return new AutenticacaoResultadoDto
        {
            Sucesso = true,
            Usuario = usuario
        };
    }
}
