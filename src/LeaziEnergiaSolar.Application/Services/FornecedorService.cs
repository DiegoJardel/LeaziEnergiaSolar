using LeaziEnergiaSolar.Application.DTOs;
using LeaziEnergiaSolar.Application.Interfaces;
using LeaziEnergiaSolar.Application.Validators;
using LeaziEnergiaSolar.Domain.Entities;
using LeaziEnergiaSolar.Domain.Enums;
using LeaziEnergiaSolar.Domain.Interfaces;

namespace LeaziEnergiaSolar.Application.Services;

public sealed class FornecedorService : IFornecedorService
{
    private readonly IFornecedorRepository _repository;
    public FornecedorService(IFornecedorRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<FornecedorDto>> ListarAsync(string? pesquisa = null, bool? ativo = null, CancellationToken cancellationToken = default) =>
        (await _repository.ListarAsync(pesquisa, ativo, cancellationToken)).Select(Mapear).ToList();

    public async Task<FornecedorDto?> ObterAsync(int id, CancellationToken cancellationToken = default)
    {
        var x = await _repository.ObterAsync(id, cancellationToken);
        return x is null ? null : Mapear(x);
    }

    public async Task<ResultadoOperacaoDto> SalvarAsync(SalvarFornecedorDto dto, CancellationToken cancellationToken = default)
    {
        dto = Normalizar(dto);
        var erros = FornecedorValidator.Validar(dto);
        if (erros.Count > 0) return ResultadoOperacaoDto.Falha(string.Join(Environment.NewLine, erros));

        var documento = DocumentoValidator.SomenteNumeros(dto.CpfCnpj);
        if (!string.IsNullOrWhiteSpace(documento) &&
            await _repository.ExisteDocumentoAsync(documento, dto.Id, cancellationToken))
            return ResultadoOperacaoDto.Falha("Já existe um fornecedor cadastrado com este CPF ou CNPJ.");

        if (dto.Id.HasValue)
        {
            var x = await _repository.ObterAsync(dto.Id.Value, cancellationToken);
            if (x is null) return ResultadoOperacaoDto.Falha("O fornecedor selecionado não foi encontrado.");
            Atualizar(x, dto, documento);
            x.DataAtualizacao = DateTime.Now;
            await _repository.AtualizarAsync(x, cancellationToken);
            return ResultadoOperacaoDto.Ok("Fornecedor atualizado com sucesso.");
        }

        var novo = new Fornecedor { DataCadastro = DateTime.Now };
        Atualizar(novo, dto, documento);
        novo.Ativo = true;
        await _repository.AdicionarAsync(novo, cancellationToken);
        return ResultadoOperacaoDto.Ok("Fornecedor cadastrado com sucesso.");
    }

    public async Task<ResultadoOperacaoDto> AlterarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default)
    {
        var x = await _repository.ObterAsync(id, cancellationToken);
        if (x is null) return ResultadoOperacaoDto.Falha("O fornecedor selecionado não foi encontrado.");
        x.Ativo = ativo; x.DataAtualizacao = DateTime.Now;
        await _repository.AtualizarAsync(x, cancellationToken);
        return ResultadoOperacaoDto.Ok(ativo ? "Fornecedor reativado com sucesso." : "Fornecedor inativado com sucesso.");
    }

    private static SalvarFornecedorDto Normalizar(SalvarFornecedorDto x) => new()
    {
        Id=x.Id, TipoPessoa=x.TipoPessoa, NomeRazaoSocial=Texto(x.NomeRazaoSocial),
        NomeFantasia=Texto(x.NomeFantasia), CpfCnpj=DocumentoValidator.SomenteNumeros(x.CpfCnpj),
        Telefone=DocumentoValidator.SomenteNumeros(x.Telefone),
        Email=string.IsNullOrWhiteSpace(x.Email)?string.Empty:x.Email.Trim().ToLowerInvariant(),
        ContatoResponsavel=Texto(x.ContatoResponsavel), Observacao=Texto(x.Observacao), Ativo=x.Ativo
    };

    private static void Atualizar(Fornecedor x, SalvarFornecedorDto d, string documento)
    {
        x.TipoPessoa=d.TipoPessoa; x.NomeRazaoSocial=d.NomeRazaoSocial; x.NomeFantasia=Nulo(d.NomeFantasia);
        x.CpfCnpj=Nulo(documento); x.Telefone=Nulo(d.Telefone); x.Email=Nulo(d.Email);
        x.ContatoResponsavel=Nulo(d.ContatoResponsavel); x.Observacao=Nulo(d.Observacao); x.Ativo=d.Ativo;
    }
    private static FornecedorDto Mapear(
        Fornecedor entidade) =>
        new()
        {
            Id = entidade.Id,
            TipoPessoa = entidade.TipoPessoa,
            NomeRazaoSocial = entidade.NomeRazaoSocial,
            NomeFantasia = entidade.NomeFantasia ?? string.Empty,
            CpfCnpj = entidade.CpfCnpj ?? string.Empty,
            Telefone = entidade.Telefone ?? string.Empty,
            Email = entidade.Email ?? string.Empty,
            ContatoResponsavel = entidade.ContatoResponsavel ?? string.Empty,
            Observacao = entidade.Observacao ?? string.Empty,
            Ativo = entidade.Ativo,
            DataCadastro = entidade.DataCadastro,
            DataAtualizacao = entidade.DataAtualizacao
        };
    private static string Texto(string? v)=>string.Join(" ",(v??string.Empty).Trim().Split(' ',StringSplitOptions.RemoveEmptyEntries)).ToUpperInvariant();
    private static string? Nulo(string? v)=>string.IsNullOrWhiteSpace(v)?null:v;
}
