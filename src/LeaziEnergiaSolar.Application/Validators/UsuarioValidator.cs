using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.Validators;

public static class UsuarioValidator
{
    public static IReadOnlyList<string> Validar(
        SalvarUsuarioDto usuario,
        bool senhaObrigatoria)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(usuario.Nome))
        {
            erros.Add("Informe o nome do usuário.");
        }
        else if (usuario.Nome.Trim().Length < 3)
        {
            erros.Add("O nome deve possuir no mínimo 3 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Login))
        {
            erros.Add("Informe o login do usuário.");
        }
        else if (usuario.Login.Trim().Length < 3)
        {
            erros.Add("O login deve possuir no mínimo 3 caracteres.");
        }
        else if (usuario.Login.Any(char.IsWhiteSpace))
        {
            erros.Add("O login não pode possuir espaços.");
        }

        if (senhaObrigatoria || !string.IsNullOrWhiteSpace(usuario.Senha))
        {
            ValidarSenha(usuario.Senha, erros);
        }

        if (!Enum.IsDefined(typeof(PerfilUsuario), usuario.Perfil))
        {
            erros.Add("Selecione um perfil válido.");
        }

        return erros;
    }

    public static IReadOnlyList<string> ValidarNovaSenha(string senha)
    {
        var erros = new List<string>();
        ValidarSenha(senha, erros);
        return erros;
    }

    private static void ValidarSenha(
        string senha,
        ICollection<string> erros)
    {
        if (string.IsNullOrWhiteSpace(senha))
        {
            erros.Add("Informe a senha.");
            return;
        }

        if (senha.Length < 8)
        {
            erros.Add("A senha deve possuir no mínimo 8 caracteres.");
        }

        if (!senha.Any(char.IsUpper))
        {
            erros.Add("A senha deve possuir ao menos uma letra maiúscula.");
        }

        if (!senha.Any(char.IsLower))
        {
            erros.Add("A senha deve possuir ao menos uma letra minúscula.");
        }

        if (!senha.Any(char.IsDigit))
        {
            erros.Add("A senha deve possuir ao menos um número.");
        }
    }
}
