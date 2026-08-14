using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;

namespace LeaziEnergiaSolar.Infrastructure.Services;

public sealed class ViaCepService : ICepService
{
    private static readonly HttpClient HttpClient = CriarHttpClient();

    public async Task<EnderecoCepDto?> ConsultarAsync(
        string cep,
        CancellationToken cancellationToken = default)
    {
        var numeros = new string((cep ?? string.Empty)
            .Where(char.IsDigit)
            .ToArray());

        if (numeros.Length != 8)
        {
            return null;
        }

        try
        {
            using var response = await HttpClient.GetAsync(
                $"ws/{numeros}/json/",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var dados = await response.Content.ReadFromJsonAsync<ViaCepResponse>(
                cancellationToken: cancellationToken);

            if (dados is null || dados.Erro)
            {
                return null;
            }

            return new EnderecoCepDto
            {
                Cep = SomenteNumeros(dados.Cep),
                Logradouro = dados.Logradouro?.Trim() ?? string.Empty,
                Complemento = dados.Complemento?.Trim() ?? string.Empty,
                Bairro = dados.Bairro?.Trim() ?? string.Empty,
                Cidade = dados.Localidade?.Trim() ?? string.Empty,
                CodigoIbgeCidade = SomenteNumeros(dados.Ibge),
                SiglaUf = dados.Uf?.Trim().ToUpperInvariant() ?? string.Empty,
                Encontrado = true
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private static HttpClient CriarHttpClient()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://viacep.com.br/"),
            Timeout = TimeSpan.FromSeconds(8)
        };

        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "LeaziGestaoSolar/1.0");

        return client;
    }

    private static string SomenteNumeros(string? valor)
    {
        return new string((valor ?? string.Empty)
            .Where(char.IsDigit)
            .ToArray());
    }

    private sealed class ViaCepResponse
    {
        [JsonPropertyName("cep")]
        public string? Cep { get; init; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; init; }

        [JsonPropertyName("complemento")]
        public string? Complemento { get; init; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; init; }

        [JsonPropertyName("localidade")]
        public string? Localidade { get; init; }

        [JsonPropertyName("uf")]
        public string? Uf { get; init; }

        [JsonPropertyName("ibge")]
        public string? Ibge { get; init; }

        [JsonPropertyName("erro")]
        public bool Erro { get; init; }
    }
}
