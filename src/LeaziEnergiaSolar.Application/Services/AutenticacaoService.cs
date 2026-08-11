using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class AutenticacaoService : IAutenticacaoService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AutenticacaoService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<AutenticacaoResultadoDto> AutenticarAsync(
        string login,
        string senha,
        CancellationToken cancellationToken = default)
    {
        var loginNormalizado = login.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(loginNormalizado) ||
            string.IsNullOrWhiteSpace(senha))
        {
            return AutenticacaoResultadoDto.Falha(
                "Informe o usuário e a senha.");
        }

        var usuario = await _usuarioRepository.ObterPorLoginAsync(
            loginNormalizado,
            cancellationToken);

        if (usuario is null ||
            !usuario.Ativo ||
            !VerificarSenha(senha, usuario.SenhaHash))
        {
            return AutenticacaoResultadoDto.Falha(
                "Usuário ou senha inválidos.");
        }

        return AutenticacaoResultadoDto.Ok(
            new UsuarioAutenticadoDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Login = usuario.Login,
                Perfil = usuario.Perfil
            });
    }

    public string GerarHashSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
        {
            throw new ArgumentException(
                "A senha é obrigatória.",
                nameof(senha));
        }

        return BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 12);
    }

    public bool VerificarSenha(string senha, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senha) ||
            string.IsNullOrWhiteSpace(senhaHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
