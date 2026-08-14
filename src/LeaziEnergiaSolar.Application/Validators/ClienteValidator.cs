using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Domain.Enums;

namespace LeaziEnergiaSolar.Application.Validators;

public static class ClienteValidator
{
    public static IReadOnlyList<string> Validar(
        SalvarClienteDto cliente)
    {
        var erros = new List<string>();

        if (!Enum.IsDefined(
                cliente.TipoPessoa))
        {
            erros.Add(
                "Selecione o tipo de pessoa.");
        }

        if (string.IsNullOrWhiteSpace(
                cliente.NomeRazaoSocial))
        {
            erros.Add(
                "Informe o nome ou razão social.");
        }
        else if (cliente
                     .NomeRazaoSocial
                     .Trim()
                     .Length < 3)
        {
            erros.Add(
                "O nome ou razão social deve possuir pelo menos 3 caracteres.");
        }

        var documento =
            DocumentoValidator.SomenteNumeros(
                cliente.CpfCnpj);

        var documentoValido =
            cliente.TipoPessoa switch
            {
                TipoPessoa.Fisica =>
                    documento.Length == 11 &&
                    DocumentoValidator
                        .ValidarCpfCnpj(
                            documento),

                TipoPessoa.Juridica =>
                    documento.Length == 14 &&
                    DocumentoValidator
                        .ValidarCpfCnpj(
                            documento),

                _ => false
            };

        if (!documentoValido)
        {
            erros.Add(
                cliente.TipoPessoa ==
                TipoPessoa.Fisica
                    ? "Informe um CPF válido."
                    : "Informe um CNPJ válido.");
        }

        if (!string.IsNullOrWhiteSpace(
                cliente.Email) &&
            !EmailValido(
                cliente.Email))
        {
            erros.Add(
                "Informe um e-mail válido.");
        }

        ValidarTelefone(
            cliente.Telefone,
            "Telefone",
            erros);

        ValidarTelefone(
            cliente.WhatsApp,
            "WhatsApp",
            erros);

        var cep =
            DocumentoValidator.SomenteNumeros(
                cliente.Cep);

        if (!string.IsNullOrWhiteSpace(
                cliente.Cep) &&
            cep.Length != 8)
        {
            erros.Add(
                "O CEP deve possuir 8 dígitos.");
        }

        var siglaUf =
            cliente.SiglaUf?.Trim()
            ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(
                siglaUf) &&
            siglaUf.Length != 2)
        {
            erros.Add(
                "Informe uma UF válida.");
        }

        var codigoIbgeCidade =
            DocumentoValidator.SomenteNumeros(
                cliente.CodigoIbgeCidade);

        if (string.IsNullOrWhiteSpace(
                codigoIbgeCidade))
        {
            erros.Add(
                "Selecione um município válido.");
        }

        if (string.IsNullOrWhiteSpace(
                cliente.Cidade))
        {
            erros.Add(
                "Informe o município.");
        }

        var codigoIbgeUf =
            DocumentoValidator.SomenteNumeros(
                cliente.CodigoIbgeUf);

        if (string.IsNullOrWhiteSpace(
                codigoIbgeUf))
        {
            erros.Add(
                "Selecione um estado válido.");
        }

        /*
         * Não validar MunicipioId aqui.
         *
         * O município selecionado na tela vem da API do IBGE.
         * Por isso, MunicipioDto.Id pode ser zero enquanto
         * CodigoIbgeCidade é válido.
         *
         * A associação com um município persistido localmente
         * deve ser resolvida na camada de serviço ou repositório.
         */

        return erros;
    }

    private static void ValidarTelefone(
        string valor,
        string nome,
        ICollection<string> erros)
    {
        if (string.IsNullOrWhiteSpace(
                valor))
        {
            return;
        }

        var numeros =
            DocumentoValidator.SomenteNumeros(
                valor);

        if (numeros.Length < 10 ||
            numeros.Length > 11)
        {
            erros.Add(
                $"Informe um {nome} com DDD válido.");
        }
    }

    private static bool EmailValido(
        string valor)
    {
        var email =
            valor.Trim();

        if (email.Length > 150 ||
            email.Contains(' ') ||
            email.Count(
                caractere =>
                    caractere == '@') != 1)
        {
            return false;
        }

        var partes =
            email.Split('@');

        return partes[0].Length > 0 &&
               partes[1].Contains('.') &&
               !partes[1].StartsWith('.') &&
               !partes[1].EndsWith('.');
    }
}