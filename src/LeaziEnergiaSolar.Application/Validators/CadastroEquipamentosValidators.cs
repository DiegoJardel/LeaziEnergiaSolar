using LeaziEnergiaSolar.Application.DTOs;

namespace LeaziEnergiaSolar.Application.Validators;

public static class EquipamentoValidator
{
    public static IReadOnlyList<string> Validar(
        SalvarEquipamentoDto dto)
    {
        var erros =
            new List<string>();

        var modelo =
            Texto(dto.Modelo);

        var observacao =
            Texto(dto.Observacao);

        if (dto.CategoriaEquipamentoId <= 0)
        {
            erros.Add(
                "Selecione uma categoria de equipamento.");
        }

        if (dto.MarcaId <= 0)
        {
            erros.Add(
                "Selecione uma marca.");
        }

        if (string.IsNullOrWhiteSpace(modelo))
        {
            erros.Add(
                "Selecione um modelo.");
        }
        else if (modelo.Length > 100)
        {
            erros.Add(
                "O modelo deve possuir no máximo 100 caracteres.");
        }

        if (dto.UnidadeMedidaId <= 0)
        {
            erros.Add(
                "Selecione uma unidade de medida.");
        }

        if (dto.FornecedorId <= 0)
        {
            erros.Add(
                "Selecione um fornecedor.");
        }

        if (observacao.Length > 1000)
        {
            erros.Add(
                "A observação deve possuir no máximo 1000 caracteres.");
        }

        return erros;
    }

    internal static string Texto(
        string? valor)
    {
        return string.Join(
            " ",
            (valor ?? string.Empty)
                .Trim()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries));
    }
}

public static class CategoriaEquipamentoValidator
{
    public static IReadOnlyList<string> Validar(
        SalvarCategoriaEquipamentoDto dto)
    {
        var erros =
            new List<string>();

        var descricao =
            EquipamentoValidator.Texto(
                dto.Descricao);

        var observacao =
            EquipamentoValidator.Texto(
                dto.Observacao);

        if (string.IsNullOrWhiteSpace(descricao))
        {
            erros.Add(
                "Informe a descrição da categoria.");
        }
        else if (descricao.Length > 100)
        {
            erros.Add(
                "A descrição da categoria deve possuir " +
                "no máximo 100 caracteres.");
        }

        if (observacao.Length > 500)
        {
            erros.Add(
                "A observação deve possuir " +
                "no máximo 500 caracteres.");
        }

        return erros;
    }
}

public static class MarcaValidator
{
    public static IReadOnlyList<string> Validar(
        SalvarMarcaDto dto)
    {
        var erros =
            new List<string>();

        var nome =
            EquipamentoValidator.Texto(
                dto.Nome);

        var observacao =
            EquipamentoValidator.Texto(
                dto.Observacao);

        if (string.IsNullOrWhiteSpace(nome))
        {
            erros.Add(
                "Informe o nome da marca.");
        }
        else if (nome.Length > 100)
        {
            erros.Add(
                "O nome da marca deve possuir " +
                "no máximo 100 caracteres.");
        }

        if (observacao.Length > 500)
        {
            erros.Add(
                "A observação deve possuir " +
                "no máximo 500 caracteres.");
        }

        return erros;
    }
}

public static class UnidadeMedidaValidator
{
    public static IReadOnlyList<string> Validar(
        SalvarUnidadeMedidaDto dto)
    {
        var erros =
            new List<string>();

        var sigla =
            EquipamentoValidator.Texto(
                dto.Sigla);

        var descricao =
            EquipamentoValidator.Texto(
                dto.Descricao);

        if (string.IsNullOrWhiteSpace(sigla))
        {
            erros.Add(
                "Informe a sigla da unidade de medida.");
        }
        else if (sigla.Length > 10)
        {
            erros.Add(
                "A sigla deve possuir no máximo 10 caracteres.");
        }
        else if (sigla.Any(char.IsControl))
        {
            erros.Add(
                "A sigla contém caracteres inválidos.");
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            erros.Add(
                "Informe a descrição da unidade de medida.");
        }
        else if (descricao.Length > 100)
        {
            erros.Add(
                "A descrição deve possuir " +
                "no máximo 100 caracteres.");
        }

        return erros;
    }
}

public static class FornecedorValidator
{
    public static IReadOnlyList<string> Validar(
        SalvarFornecedorDto dto)
    {
        var erros =
            new List<string>();

        if (!Enum.IsDefined(dto.TipoPessoa))
        {
            erros.Add(
                "Selecione o tipo de pessoa.");
        }

        var nome =
            EquipamentoValidator.Texto(
                dto.NomeRazaoSocial);

        if (string.IsNullOrWhiteSpace(nome))
        {
            erros.Add(
                "Informe o nome ou razão social.");
        }
        else if (nome.Length < 3)
        {
            erros.Add(
                "O nome ou razão social deve possuir " +
                "pelo menos 3 caracteres.");
        }
        else if (nome.Length > 150)
        {
            erros.Add(
                "O nome ou razão social deve possuir " +
                "no máximo 150 caracteres.");
        }

        var documento =
            DocumentoValidator.SomenteNumeros(
                dto.CpfCnpj);

        if (documento.Length > 0)
        {
            var tamanhoEsperado =
                dto.TipoPessoa ==
                Domain.Enums.TipoPessoa.Fisica
                    ? 11
                    : 14;

            if (documento.Length != tamanhoEsperado ||
                !DocumentoValidator.ValidarCpfCnpj(
                    documento))
            {
                erros.Add(
                    dto.TipoPessoa ==
                    Domain.Enums.TipoPessoa.Fisica
                        ? "Informe um CPF válido."
                        : "Informe um CNPJ válido.");
            }
        }

        var telefone =
            DocumentoValidator.SomenteNumeros(
                dto.Telefone);

        if (!string.IsNullOrWhiteSpace(dto.Telefone) &&
            (telefone.Length < 10 ||
             telefone.Length > 11))
        {
            erros.Add(
                "Informe um telefone com DDD válido.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var email =
                dto.Email
                    .Trim()
                    .ToLowerInvariant();

            if (!EmailValidator.IsValid(email))
            {
                erros.Add(
                    "Informe um e-mail válido.");
            }
        }

        if (dto.NomeFantasia?.Length > 150)
        {
            erros.Add(
                "O nome fantasia deve possuir " +
                "no máximo 150 caracteres.");
        }

        if (dto.ContatoResponsavel?.Length > 150)
        {
            erros.Add(
                "O contato responsável deve possuir " +
                "no máximo 150 caracteres.");
        }

        if (dto.Observacao?.Length > 1000)
        {
            erros.Add(
                "A observação deve possuir " +
                "no máximo 1000 caracteres.");
        }

        return erros;
    }
}