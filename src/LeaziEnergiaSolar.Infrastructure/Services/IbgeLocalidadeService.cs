using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Infrastructure.Services;

public sealed class IbgeLocalidadeService : IIbgeLocalidadeService
{
    private static readonly HttpClient HttpClient = CriarHttpClient();
    private readonly ILocalidadeRepository _localidadeRepository;

    public IbgeLocalidadeService(ILocalidadeRepository localidadeRepository)
    {
        _localidadeRepository = localidadeRepository;
    }

    public async Task<IReadOnlyList<EstadoDto>> ListarEstadosAsync(
        CancellationToken cancellationToken = default)
    {
        var locais = await _localidadeRepository.ListarEstadosAsync(cancellationToken);

        if (locais.Count > 0)
        {
            return locais.Select(MapearEstado).ToList();
        }

        try
        {
            var resposta = await HttpClient.GetFromJsonAsync<List<IbgeEstadoResponse>>(
                "estados?orderBy=nome",
                cancellationToken);

            if (resposta is not null && resposta.Count > 0)
            {
                var estados = resposta
                    .Where(estado => estado.Id > 0 && !string.IsNullOrWhiteSpace(estado.Sigla))
                    .OrderBy(estado => estado.Nome)
                    .Select(estado => new Estado
                    {
                        CodigoIbge = estado.Id.ToString(),
                        Nome = (estado.Nome ?? string.Empty).Trim().ToUpperInvariant(),
                        Sigla = (estado.Sigla ?? string.Empty).Trim().ToUpperInvariant(),
                        Ativo = true,
                        DataAtualizacao = DateTime.Now
                    })
                    .ToList();

                await _localidadeRepository.SalvarEstadosAsync(estados, cancellationToken);
                return estados.Select(MapearEstado).ToList();
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (HttpRequestException)
        {
        }

        return CriarEstadosFallback();
    }

    public async Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosAsync(
        string codigoIbgeEstado,
        CancellationToken cancellationToken = default)
    {
        var codigoEstado = SomenteNumeros(codigoIbgeEstado);

        if (string.IsNullOrWhiteSpace(codigoEstado))
        {
            return Array.Empty<MunicipioDto>();
        }

        var locais = await _localidadeRepository.ListarMunicipiosAsync(
            codigoEstado,
            cancellationToken);

        if (locais.Count > 0)
        {
            return locais.Select(MapearMunicipio).ToList();
        }

        var estado = await _localidadeRepository.ObterEstadoPorCodigoIbgeAsync(
            codigoEstado,
            cancellationToken);

        if (estado is null)
        {
            await ListarEstadosAsync(cancellationToken);
            estado = await _localidadeRepository.ObterEstadoPorCodigoIbgeAsync(
                codigoEstado,
                cancellationToken);
        }

        if (estado is null)
        {
            return Array.Empty<MunicipioDto>();
        }

        try
        {
            var resposta = await HttpClient.GetFromJsonAsync<List<IbgeMunicipioResponse>>(
                $"estados/{codigoEstado}/municipios?orderBy=nome",
                cancellationToken);

            if (resposta is null)
            {
                return Array.Empty<MunicipioDto>();
            }

            var municipios = resposta
                .Where(municipio => municipio.Id > 0 && !string.IsNullOrWhiteSpace(municipio.Nome))
                .OrderBy(municipio => municipio.Nome)
                .Select(municipio => new Municipio
                {
                    CodigoIbge = municipio.Id.ToString(),
                    Nome = (municipio.Nome ?? string.Empty).Trim().ToUpperInvariant(),
                    EstadoId = estado.Id,
                    Ativo = true,
                    DataAtualizacao = DateTime.Now
                })
                .ToList();

            await _localidadeRepository.SalvarMunicipiosAsync(municipios, cancellationToken);
            return municipios.Select(MapearMunicipio).ToList();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (HttpRequestException)
        {
            return Array.Empty<MunicipioDto>();
        }
    }

    private static EstadoDto MapearEstado(Estado estado) => new()
    {
        CodigoIbge = estado.CodigoIbge,
        Nome = estado.Nome,
        Sigla = estado.Sigla
    };

    private static MunicipioDto MapearMunicipio(Municipio municipio) => new()
    {
        CodigoIbge = municipio.CodigoIbge,
        Nome = municipio.Nome,
        CodigoIbgeEstado = municipio.Estado?.CodigoIbge ?? string.Empty
    };

    private static string SomenteNumeros(string? valor) =>
        new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());

    private static HttpClient CriarHttpClient()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://servicodados.ibge.gov.br/api/v1/localidades/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        client.DefaultRequestHeaders.UserAgent.ParseAdd("LeaziGestaoSolar/1.0");
        return client;
    }

    private static IReadOnlyList<EstadoDto> CriarEstadosFallback()
    {
        var estados = new[]
        {
            (11, "RONDÔNIA", "RO"), (12, "ACRE", "AC"), (13, "AMAZONAS", "AM"),
            (14, "RORAIMA", "RR"), (15, "PARÁ", "PA"), (16, "AMAPÁ", "AP"),
            (17, "TOCANTINS", "TO"), (21, "MARANHÃO", "MA"), (22, "PIAUÍ", "PI"),
            (23, "CEARÁ", "CE"), (24, "RIO GRANDE DO NORTE", "RN"), (25, "PARAÍBA", "PB"),
            (26, "PERNAMBUCO", "PE"), (27, "ALAGOAS", "AL"), (28, "SERGIPE", "SE"),
            (29, "BAHIA", "BA"), (31, "MINAS GERAIS", "MG"), (32, "ESPÍRITO SANTO", "ES"),
            (33, "RIO DE JANEIRO", "RJ"), (35, "SÃO PAULO", "SP"), (41, "PARANÁ", "PR"),
            (42, "SANTA CATARINA", "SC"), (43, "RIO GRANDE DO SUL", "RS"),
            (50, "MATO GROSSO DO SUL", "MS"), (51, "MATO GROSSO", "MT"),
            (52, "GOIÁS", "GO"), (53, "DISTRITO FEDERAL", "DF")
        };

        return estados.Select(estado => new EstadoDto
        {
            CodigoIbge = estado.Item1.ToString(),
            Nome = estado.Item2,
            Sigla = estado.Item3
        }).OrderBy(estado => estado.Nome).ToList();
    }

    private sealed class IbgeEstadoResponse
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("nome")] public string? Nome { get; init; }
        [JsonPropertyName("sigla")] public string? Sigla { get; init; }
    }

    private sealed class IbgeMunicipioResponse
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("nome")] public string? Nome { get; init; }
    }
}
