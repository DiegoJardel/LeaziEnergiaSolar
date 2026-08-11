using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Validators;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class VendedorService : IVendedorService
{
    private readonly IVendedorRepository _vendedorRepository;

    public VendedorService(IVendedorRepository vendedorRepository)
    {
        _vendedorRepository = vendedorRepository;
    }

    public async Task<IReadOnlyList<VendedorDto>> ListarAsync(
        string? pesquisa = null,
        CancellationToken cancellationToken = default)
    {
        var vendedores = await _vendedorRepository.ListarAsync(
            pesquisa,
            cancellationToken);

        return vendedores
            .Select(Mapear)
            .ToList();
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(
        SalvarVendedorDto vendedor,
        CancellationToken cancellationToken = default)
    {
        var erros = VendedorValidator.Validar(vendedor);

        if (erros.Count > 0)
        {
            return ResultadoOperacaoDto.Falha(string.Join(Environment.NewLine, erros));
        }

        var documento = DocumentoValidator.SomenteNumeros(vendedor.CpfCnpj);

        var documentoDuplicado = await _vendedorRepository.ExisteDocumentoAsync(
            documento,
            vendedor.Id,
            cancellationToken);

        if (documentoDuplicado)
        {
            return ResultadoOperacaoDto.Falha(
                "Já existe um vendedor cadastrado com este CPF ou CNPJ.");
        }

        if (vendedor.Id.HasValue)
        {
            var entidade = await _vendedorRepository.ObterAsync(
                vendedor.Id.Value,
                cancellationToken);

            if (entidade is null)
            {
                return ResultadoOperacaoDto.Falha(
                    "O vendedor selecionado não foi encontrado.");
            }

            AtualizarEntidade(entidade, vendedor, documento);

            await _vendedorRepository.AtualizarAsync(
                entidade,
                cancellationToken);

            return ResultadoOperacaoDto.Ok(
                "Vendedor atualizado com sucesso.");
        }

        var novoVendedor = new Vendedor();
        AtualizarEntidade(novoVendedor, vendedor, documento);

        await _vendedorRepository.AdicionarAsync(
            novoVendedor,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            "Vendedor cadastrado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        var vendedor = await _vendedorRepository.ObterAsync(
            id,
            cancellationToken);

        if (vendedor is null)
        {
            return ResultadoOperacaoDto.Falha(
                "O vendedor selecionado não foi encontrado.");
        }

        vendedor.Ativo = ativo;

        await _vendedorRepository.AtualizarAsync(
            vendedor,
            cancellationToken);

        return ResultadoOperacaoDto.Ok(
            ativo
                ? "Vendedor ativado com sucesso."
                : "Vendedor inativado com sucesso.");
    }

    private static void AtualizarEntidade(
        Vendedor entidade,
        SalvarVendedorDto vendedor,
        string documento)
    {
        entidade.Nome = vendedor.Nome.Trim();
        entidade.CpfCnpj = documento;
        entidade.Telefone = DocumentoValidator.SomenteNumeros(vendedor.Telefone);
        entidade.Email = vendedor.Email.Trim().ToLowerInvariant();
        entidade.PercentualComissao = vendedor.PercentualComissao;
        entidade.Ativo = vendedor.Ativo;
    }

    private static VendedorDto Mapear(Vendedor vendedor)
    {
        return new VendedorDto
        {
            Id = vendedor.Id,
            Nome = vendedor.Nome,
            CpfCnpj = vendedor.CpfCnpj ?? string.Empty,
            Telefone = vendedor.Telefone ?? string.Empty,
            Email = vendedor.Email ?? string.Empty,
            PercentualComissao = vendedor.PercentualComissao,
            Ativo = vendedor.Ativo
        };
    }
}
