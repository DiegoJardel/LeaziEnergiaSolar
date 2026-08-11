using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Interfaces;

public interface IAutenticacaoService
{
    Task<AutenticacaoResultadoDto> AutenticarAsync(
        string login,
        string senha,
        CancellationToken cancellationToken = default);

    string GerarHashSenha(string senha);

    bool VerificarSenha(string senha, string senhaHash);
}
