using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Validators;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ILocalidadeRepository _localidadeRepository;

    public ClienteService(
        IClienteRepository clienteRepository,
        ILocalidadeRepository localidadeRepository)
    {
        _clienteRepository = clienteRepository;
        _localidadeRepository = localidadeRepository;
    }

    public async Task<IReadOnlyList<ClienteDto>> ListarAsync(
        string? pesquisa = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.ListarAsync(
            pesquisa,
            ativo,
            cancellationToken);

        return clientes
            .Select(Mapear)
            .ToList();
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarClienteDto cliente,
        CancellationToken cancellationToken = default)
    {
        var dados = Normalizar(cliente);
        var erros = ClienteValidator.Validar(dados);

        if (erros.Count > 0)
        {
            return ResultadoOperacaoDto.Falha(
                string.Join(Environment.NewLine, erros));
        }

        var documento = DocumentoValidator.SomenteNumeros(
            dados.CpfCnpj);

        if (dados.MunicipioId.HasValue)
        {
            var municipio = await _localidadeRepository.ObterMunicipioPorCodigoIbgeAsync(
                dados.CodigoIbgeCidade,
                cancellationToken);

            if (municipio is null || municipio.Id != dados.MunicipioId.Value)
            {
                return ResultadoOperacaoDto.Falha(
                    "O município selecionado não é válido.");
            }

            var estado = await _localidadeRepository.ObterEstadoPorCodigoIbgeAsync(
                dados.CodigoIbgeUf,
                cancellationToken);

            if (estado is null || estado.Id != municipio.EstadoId)
            {
                return ResultadoOperacaoDto.Falha(
                    "O município selecionado não pertence à UF informada.");
            }
        }

        var documentoDuplicado =
            await _clienteRepository.ExisteDocumentoAsync(
                documento,
                dados.Id,
                cancellationToken);

        if (documentoDuplicado)
        {
            return ResultadoOperacaoDto.Falha(
                "Já existe um cliente cadastrado com este CPF ou CNPJ.");
        }

        if (dados.Id.HasValue)
        {
            var entidade = await _clienteRepository.ObterAsync(
                dados.Id.Value,
                cancellationToken);

            if (entidade is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "O cliente selecionado não foi encontrado.");
            }

            AtualizarEntidade(entidade, dados, documento);
            await _clienteRepository.AtualizarAsync(
                entidade,
                cancellationToken);

            return ResultadoOperacaoDto.Ok(
                "Cliente atualizado com sucesso.");
        }

        var novoCliente = new Cliente();
        AtualizarEntidade(novoCliente, dados, documento);
        novoCliente.DataCadastro = DateTime.Now;

        await _clienteRepository.AdicionarAsync(
            novoCliente,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Cliente cadastrado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ResultadoOperacaoDto.Falha(
                "O cliente informado é inválido.");
        }

        var cliente = await _clienteRepository.ObterAsync(
            id,
            cancellationToken);

        if (cliente is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O cliente selecionado não foi encontrado.");
        }

        cliente.Ativo = ativo;
        cliente.DataAlteracao = DateTime.Now;

        await _clienteRepository.AtualizarAsync(
            cliente,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            ativo
                ? "Cliente reativado com sucesso."
                : "Cliente inativado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> ExcluirAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ResultadoOperacaoDto.Falha(
                "O cliente informado é inválido.");
        }

        var cliente = await _clienteRepository.ObterAsync(
            id,
            cancellationToken);

        if (cliente is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O cliente selecionado não foi encontrado.");
        }

        if (await _clienteRepository.PossuiLancamentosAsync(
                id,
                cancellationToken))
        {
            return ResultadoOperacaoDto.Falha(
                "Não é possível excluir este cliente porque existem lançamentos vinculados a ele. " +
                "Exclua ou desvincule os lançamentos antes de excluir o cadastro.");
        }

        await _clienteRepository.ExcluirAsync(
            cliente,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Cliente excluído com sucesso.");
    }

    private static SalvarClienteDto Normalizar(
        SalvarClienteDto cliente)
    {
        return new SalvarClienteDto
        {
            Id = cliente.Id,
            TipoPessoa = cliente.TipoPessoa,
            NomeRazaoSocial = NormalizarTexto(cliente.NomeRazaoSocial),
            NomeFantasia = NormalizarTexto(cliente.NomeFantasia),
            CpfCnpj = DocumentoValidator.SomenteNumeros(cliente.CpfCnpj),
            RgInscricaoEstadual = NormalizarTexto(cliente.RgInscricaoEstadual),
            DataNascimentoAbertura = cliente.DataNascimentoAbertura,
            Telefone = DocumentoValidator.SomenteNumeros(cliente.Telefone),
            WhatsApp = DocumentoValidator.SomenteNumeros(cliente.WhatsApp),
            Email = string.IsNullOrWhiteSpace(cliente.Email)
                ? string.Empty
                : cliente.Email.Trim().ToLowerInvariant(),
            Cep = DocumentoValidator.SomenteNumeros(cliente.Cep),
            Logradouro = NormalizarTexto(cliente.Logradouro),
            Numero = NormalizarTexto(cliente.Numero),
            Complemento = NormalizarTexto(cliente.Complemento),
            Bairro = NormalizarTexto(cliente.Bairro),
            Cidade = NormalizarTexto(cliente.Cidade),
            CodigoIbgeCidade = DocumentoValidator.SomenteNumeros(cliente.CodigoIbgeCidade),
            Estado = NormalizarTexto(cliente.Estado),
            SiglaUf = NormalizarTexto(cliente.SiglaUf),
            CodigoIbgeUf = DocumentoValidator.SomenteNumeros(cliente.CodigoIbgeUf),
            MunicipioId = cliente.MunicipioId,
            PontoReferencia = NormalizarTexto(cliente.PontoReferencia),
            Observacao = NormalizarTexto(cliente.Observacao),
            Ativo = cliente.Ativo
        };
    }

    private static void AtualizarEntidade(
        Cliente entidade,
        SalvarClienteDto dados,
        string documento)
    {
        entidade.TipoPessoa = dados.TipoPessoa;
        entidade.NomeRazaoSocial = dados.NomeRazaoSocial;
        entidade.NomeFantasia = ValorNulo(dados.NomeFantasia);
        entidade.CpfCnpj = documento;
        entidade.RgInscricaoEstadual = ValorNulo(dados.RgInscricaoEstadual);
        entidade.DataNascimentoAbertura = dados.DataNascimentoAbertura;
        entidade.Telefone = ValorNulo(dados.Telefone);
        entidade.WhatsApp = ValorNulo(dados.WhatsApp);
        entidade.Email = ValorNulo(dados.Email);
        entidade.Cep = ValorNulo(dados.Cep);
        entidade.Logradouro = ValorNulo(dados.Logradouro);
        entidade.Numero = ValorNulo(dados.Numero);
        entidade.Complemento = ValorNulo(dados.Complemento);
        entidade.Bairro = ValorNulo(dados.Bairro);
        entidade.Cidade = ValorNulo(dados.Cidade);
        entidade.CodigoIbgeCidade = ValorNulo(dados.CodigoIbgeCidade);
        entidade.Estado = ValorNulo(dados.Estado);
        entidade.SiglaUf = ValorNulo(dados.SiglaUf);
        entidade.CodigoIbgeUf = ValorNulo(dados.CodigoIbgeUf);
        entidade.MunicipioId = dados.MunicipioId;
        entidade.PontoReferencia = ValorNulo(dados.PontoReferencia);
        entidade.Observacao = ValorNulo(dados.Observacao);
        entidade.Ativo = dados.Ativo;
        entidade.DataAlteracao = DateTime.Now;
    }

    private static ClienteDto Mapear(Cliente cliente)
    {
        var endereco = string.Join(", ", new[]
        {
            cliente.Logradouro,
            cliente.Numero,
            cliente.Bairro
        }.Where(valor => !string.IsNullOrWhiteSpace(valor)));

        return new ClienteDto
        {
            Id = cliente.Id,
            TipoPessoa = cliente.TipoPessoa == Domain.Enums.TipoPessoa.Fisica
                ? "Pessoa Física"
                : "Pessoa Jurídica",
            NomeRazaoSocial = cliente.NomeRazaoSocial,
            NomeFantasia = cliente.NomeFantasia ?? string.Empty,
            CpfCnpj = cliente.CpfCnpj ?? string.Empty,
            RgInscricaoEstadual = cliente.RgInscricaoEstadual ?? string.Empty,
            DataNascimentoAbertura = cliente.DataNascimentoAbertura,
            Telefone = cliente.Telefone ?? string.Empty,
            WhatsApp = cliente.WhatsApp ?? string.Empty,
            Email = cliente.Email ?? string.Empty,
            Cep = cliente.Cep ?? string.Empty,
            Logradouro = cliente.Logradouro ?? string.Empty,
            Numero = cliente.Numero ?? string.Empty,
            Complemento = cliente.Complemento ?? string.Empty,
            Bairro = cliente.Bairro ?? string.Empty,
            Cidade = cliente.Cidade ?? string.Empty,
            SiglaUf = cliente.SiglaUf ?? string.Empty,
            CodigoIbgeCidade = cliente.CodigoIbgeCidade ?? string.Empty,
            CodigoIbgeUf = cliente.CodigoIbgeUf ?? string.Empty,
            MunicipioId = cliente.MunicipioId,
            PontoReferencia = cliente.PontoReferencia ?? string.Empty,
            Observacao = cliente.Observacao ?? string.Empty,
            EnderecoCompleto = endereco,
            CidadeUf = string.Join(" / ", new[]
            {
                cliente.Cidade,
                cliente.SiglaUf
            }.Where(valor => !string.IsNullOrWhiteSpace(valor))),
            Ativo = cliente.Ativo
        };
    }

    private static string NormalizarTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        return string.Join(
            " ",
            valor.Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();
    }

    private static string? ValorNulo(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor;
    }
}
