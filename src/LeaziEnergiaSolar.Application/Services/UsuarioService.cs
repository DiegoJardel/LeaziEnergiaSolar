using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Validators;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAutenticacaoService _autenticacaoService;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IAutenticacaoService autenticacaoService)
    {
        _usuarioRepository = usuarioRepository;
        _autenticacaoService = autenticacaoService;
    }

    public async Task<IReadOnlyList<UsuarioDto>> ListarAsync(
        int usuarioLogadoId,
        string? pesquisa = null,
        CancellationToken cancellationToken = default)
    {
        await ValidarAdministradorAsync(
            usuarioLogadoId,
            cancellationToken);

        var usuarios = await _usuarioRepository.ListarAsync(
            pesquisa,
            cancellationToken);

        return usuarios.Select(Mapear).ToList();
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(
        int usuarioLogadoId,
        SalvarUsuarioDto usuario,
        CancellationToken cancellationToken = default)
    {
        var administrador = await ValidarAdministradorAsync(
            usuarioLogadoId,
            cancellationToken);

        var erros = UsuarioValidator.Validar(
            usuario,
            senhaObrigatoria: !usuario.Id.HasValue);

        if (erros.Count > 0)
        {
            return ResultadoOperacaoDto.Falha(
                string.Join(Environment.NewLine, erros));
        }

        var loginNormalizado = usuario.Login
            .Trim()
            .ToLowerInvariant();

        if (await _usuarioRepository.ExisteLoginAsync(
                loginNormalizado,
                usuario.Id,
                cancellationToken))
        {
            return ResultadoOperacaoDto.Falha(
                "Já existe um usuário cadastrado com este login.");
        }

        if (usuario.Id.HasValue)
        {
            var entidade = await _usuarioRepository.ObterAsync(
                usuario.Id.Value,
                cancellationToken);

            if (entidade is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "O usuário selecionado não foi encontrado.");
            }

            if (entidade.Id == administrador.Id && !usuario.Ativo)
            {
                return ResultadoOperacaoDto.Falha(
                    "O usuário logado não pode inativar a própria conta.");
            }

            if (entidade.Id == administrador.Id &&
                usuario.Perfil != PerfilUsuario.Administrador)
            {
                return ResultadoOperacaoDto.Falha(
                    "O usuário logado não pode remover o próprio perfil de administrador.");
            }

            if (entidade.Perfil == PerfilUsuario.Administrador &&
                entidade.Ativo &&
                (usuario.Perfil != PerfilUsuario.Administrador || !usuario.Ativo) &&
                await _usuarioRepository.ContarAdministradoresAtivosAsync(
                    cancellationToken) <= 1)
            {
                return ResultadoOperacaoDto.Falha(
                    "O sistema deve possuir pelo menos um administrador ativo.");
            }

            entidade.Nome = usuario.Nome.Trim();
            entidade.Login = loginNormalizado;
            entidade.Perfil = usuario.Perfil;
            entidade.Ativo = usuario.Ativo;

            if (!string.IsNullOrWhiteSpace(usuario.Senha))
            {
                entidade.SenhaHash = _autenticacaoService.GerarHashSenha(
                    usuario.Senha);
            }

            await _usuarioRepository.AtualizarAsync(
                entidade,
                cancellationToken);

            return ResultadoOperacaoDto.Ok(
                "Usuário atualizado com sucesso.");
        }

        var novoUsuario = new Usuario
        {
            Nome = usuario.Nome.Trim(),
            Login = loginNormalizado,
            SenhaHash = _autenticacaoService.GerarHashSenha(usuario.Senha),
            Perfil = usuario.Perfil,
            Ativo = usuario.Ativo
        };

        await _usuarioRepository.AdicionarAsync(
            novoUsuario,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Usuário cadastrado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int usuarioLogadoId,
        int usuarioId,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        await ValidarAdministradorAsync(
            usuarioLogadoId,
            cancellationToken);

        if (usuarioLogadoId == usuarioId && !ativo)
        {
            return ResultadoOperacaoDto.Falha(
                "O usuário logado não pode inativar a própria conta.");
        }

        var usuario = await _usuarioRepository.ObterAsync(
            usuarioId,
            cancellationToken);

        if (usuario is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O usuário selecionado não foi encontrado.");
        }

        if (!ativo &&
            usuario.Ativo &&
            usuario.Perfil == PerfilUsuario.Administrador &&
            await _usuarioRepository.ContarAdministradoresAtivosAsync(
                cancellationToken) <= 1)
        {
            return ResultadoOperacaoDto.Falha(
                "O sistema deve possuir pelo menos um administrador ativo.");
        }

        usuario.Ativo = ativo;

        await _usuarioRepository.AtualizarAsync(
            usuario,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            ativo
                ? "Usuário ativado com sucesso."
                : "Usuário inativado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> RedefinirSenhaAsync(
        int usuarioLogadoId,
        RedefinirSenhaDto redefinicao,
        CancellationToken cancellationToken = default)
    {
        await ValidarAdministradorAsync(
            usuarioLogadoId,
            cancellationToken);

        var erros = UsuarioValidator.ValidarNovaSenha(
            redefinicao.NovaSenha);

        if (erros.Count > 0)
        {
            return ResultadoOperacaoDto.Falha(
                string.Join(Environment.NewLine, erros));
        }

        var usuario = await _usuarioRepository.ObterAsync(
            redefinicao.UsuarioId,
            cancellationToken);

        if (usuario is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O usuário selecionado não foi encontrado.");
        }

        usuario.SenhaHash = _autenticacaoService.GerarHashSenha(
            redefinicao.NovaSenha);

        await _usuarioRepository.AtualizarAsync(
            usuario,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Senha redefinida com sucesso.");
    }

    private async Task<Usuario> ValidarAdministradorAsync(
        int usuarioLogadoId,
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterAsync(
            usuarioLogadoId,
            cancellationToken);

        if (usuario is null ||
            !usuario.Ativo ||
            usuario.Perfil != PerfilUsuario.Administrador)
        {
            throw new UnauthorizedAccessException(
                "Apenas administradores podem gerenciar usuários.");
        }

        return usuario;
    }

    private static UsuarioDto Mapear(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Login = usuario.Login,
            Perfil = usuario.Perfil,
            Ativo = usuario.Ativo
        };
    }
}
