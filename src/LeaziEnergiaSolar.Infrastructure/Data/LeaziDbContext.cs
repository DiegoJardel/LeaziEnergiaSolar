using LeaziEnergiaSolar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Data;

public class LeaziDbContext : DbContext
{
    public LeaziDbContext(
        DbContextOptions<LeaziDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios =>
        Set<Usuario>();

    public DbSet<Vendedor> Vendedores =>
        Set<Vendedor>();

    public DbSet<Cliente> Clientes =>
        Set<Cliente>();

    public DbSet<Estado> Estados =>
        Set<Estado>();

    public DbSet<Municipio> Municipios =>
        Set<Municipio>();

    public DbSet<Lancamento> Lancamentos =>
        Set<Lancamento>();

    public DbSet<CategoriaEquipamento> CategoriasEquipamento =>
        Set<CategoriaEquipamento>();

    public DbSet<Marca> Marcas =>
        Set<Marca>();

    public DbSet<ModeloEquipamento> ModelosEquipamentos =>
        Set<ModeloEquipamento>();

    public DbSet<UnidadeMedida> UnidadesMedida =>
        Set<UnidadeMedida>();

    public DbSet<Equipamento> Equipamentos =>
        Set<Equipamento>();

    public DbSet<Fornecedor> Fornecedores =>
        Set<Fornecedor>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity
                .HasIndex(x => x.Login)
                .IsUnique();

            entity
                .Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity
                .Property(x => x.Login)
                .HasMaxLength(50)
                .IsRequired();

            entity
                .Property(x => x.SenhaHash)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity
                .HasIndex(x => x.CpfCnpj)
                .IsUnique();

            entity
                .Property(x => x.NomeRazaoSocial)
                .HasMaxLength(150)
                .IsRequired();

            entity
                .Property(x => x.NomeFantasia)
                .HasMaxLength(150);

            entity
                .Property(x => x.CpfCnpj)
                .HasMaxLength(14);

            entity
                .Property(x => x.RgInscricaoEstadual)
                .HasMaxLength(20);

            entity
                .Property(x => x.Telefone)
                .HasMaxLength(11);

            entity
                .Property(x => x.WhatsApp)
                .HasMaxLength(11);

            entity
                .Property(x => x.Email)
                .HasMaxLength(150);

            entity
                .Property(x => x.Cep)
                .HasMaxLength(8);

            entity
                .Property(x => x.Logradouro)
                .HasMaxLength(150);

            entity
                .Property(x => x.Numero)
                .HasMaxLength(20);

            entity
                .Property(x => x.Complemento)
                .HasMaxLength(100);

            entity
                .Property(x => x.Bairro)
                .HasMaxLength(100);

            entity
                .Property(x => x.Cidade)
                .HasMaxLength(100);

            entity
                .Property(x => x.CodigoIbgeCidade)
                .HasMaxLength(7);

            entity
                .Property(x => x.Estado)
                .HasMaxLength(100);

            entity
                .Property(x => x.SiglaUf)
                .HasMaxLength(2);

            entity
                .Property(x => x.CodigoIbgeUf)
                .HasMaxLength(2);

            entity
                .Property(x => x.PontoReferencia)
                .HasMaxLength(200);

            entity
                .Property(x => x.Observacao)
                .HasMaxLength(1000);

            entity
                .HasOne(x => x.Municipio)
                .WithMany(x => x.Clientes)
                .HasForeignKey(x => x.MunicipioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity
                .HasIndex(x => x.CodigoIbge)
                .IsUnique();

            entity
                .Property(x => x.CodigoIbge)
                .HasMaxLength(2)
                .IsRequired();

            entity
                .Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity
                .Property(x => x.Sigla)
                .HasMaxLength(2)
                .IsRequired();
        });

        modelBuilder.Entity<Municipio>(entity =>
        {
            entity
                .HasIndex(x => x.CodigoIbge)
                .IsUnique();

            entity
                .Property(x => x.CodigoIbge)
                .HasMaxLength(7)
                .IsRequired();

            entity
                .Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity
                .HasOne(x => x.Estado)
                .WithMany(x => x.Municipios)
                .HasForeignKey(x => x.EstadoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Vendedor>(entity =>
        {
            entity
                .HasIndex(x => x.CpfCnpj)
                .IsUnique();

            entity
                .Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity
                .Property(x => x.CpfCnpj)
                .HasMaxLength(14)
                .IsRequired();

            entity
                .Property(x => x.Telefone)
                .HasMaxLength(11)
                .IsRequired();

            entity
                .Property(x => x.Email)
                .HasMaxLength(150)
                .IsRequired();

            entity
                .Property(x => x.PercentualComissao)
                .HasPrecision(5, 2);
        });

        modelBuilder.Entity<CategoriaEquipamento>(entity =>
        {
            entity
                .HasIndex(x => x.Descricao)
                .IsUnique();

            entity
                .Property(x => x.Descricao)
                .HasMaxLength(100)
                .IsRequired()
                .UseCollation("NOCASE");

            entity
                .Property(x => x.Observacao)
                .HasMaxLength(500);

            entity
                .Property(x => x.DataCadastro)
                .IsRequired();

            entity
                .Property(x => x.DataAtualizacao);
        });

        modelBuilder.Entity<ModeloEquipamento>(entity =>
        {
            entity
                .HasIndex(x => new
                {
                    x.MarcaId,
                    x.Nome
                })
                .IsUnique();

            entity
                .Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired()
                .UseCollation("NOCASE");

            entity
                .Property(x => x.Observacao)
                .HasMaxLength(500);

            entity
                .Property(x => x.DataCadastro)
                .IsRequired();

            entity
                .Property(x => x.DataAtualizacao);

            entity
                .HasOne(x => x.Marca)
                .WithMany(x => x.Modelos)
                .HasForeignKey(x => x.MarcaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity
                .HasIndex(x => x.Nome)
                .IsUnique();

            entity
                .Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired()
                .UseCollation("NOCASE");

            entity
                .Property(x => x.Observacao)
                .HasMaxLength(500);

            entity
                .Property(x => x.DataCadastro)
                .IsRequired();

            entity
                .Property(x => x.DataAtualizacao);
        });

        modelBuilder.Entity<UnidadeMedida>(entity =>
        {
            entity
                .HasIndex(x => x.Sigla)
                .IsUnique();

            entity
                .Property(x => x.Sigla)
                .HasMaxLength(10)
                .IsRequired()
                .UseCollation("NOCASE");

            entity
                .Property(x => x.Descricao)
                .HasMaxLength(100)
                .IsRequired();

            entity
                .Property(x => x.DataCadastro)
                .IsRequired();

            entity
                .Property(x => x.DataAtualizacao);
        });

        modelBuilder.Entity<Equipamento>(entity =>
        {
            entity
                .Property(x => x.Modelo)
                .HasMaxLength(100)
                .IsRequired()
                .UseCollation("NOCASE");

            entity
                .Property(x => x.Observacao)
                .HasMaxLength(1000);

            entity
                .Property(x => x.DataCadastro)
                .IsRequired();

            entity
                .Property(x => x.DataAtualizacao);

            entity
                .HasIndex(x => new
                {
                    x.CategoriaEquipamentoId,
                    x.MarcaId,
                    x.Modelo
                })
                .IsUnique();

            entity
                .HasOne(x => x.CategoriaEquipamento)
                .WithMany(x => x.Equipamentos)
                .HasForeignKey(x => x.CategoriaEquipamentoId)
                .OnDelete(DeleteBehavior.Restrict);


            entity
                .HasOne(x => x.Fornecedor)
                .WithMany(x => x.Equipamentos)
                .HasForeignKey(x => x.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);


            entity
                .HasOne(x => x.Marca)
                .WithMany(x => x.Equipamentos)
                .HasForeignKey(x => x.MarcaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(x => x.UnidadeMedida)
                .WithMany(x => x.Equipamentos)
                .HasForeignKey(x => x.UnidadeMedidaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Fornecedor>(entity =>
        {
            entity
                .HasIndex(x => x.CpfCnpj)
                .IsUnique();

            entity
                .Property(x => x.NomeRazaoSocial)
                .HasMaxLength(150)
                .IsRequired();

            entity
                .Property(x => x.NomeFantasia)
                .HasMaxLength(150);

            entity
                .Property(x => x.CpfCnpj)
                .HasMaxLength(14);

            entity
                .Property(x => x.Telefone)
                .HasMaxLength(11);

            entity
                .Property(x => x.Email)
                .HasMaxLength(150);

            entity
                .Property(x => x.ContatoResponsavel)
                .HasMaxLength(150);

            entity
                .Property(x => x.Observacao)
                .HasMaxLength(1000);

            entity
                .Property(x => x.DataCadastro)
                .IsRequired();

            entity
                .Property(x => x.DataAtualizacao);
        });

        modelBuilder.Entity<Lancamento>(entity =>
        {
            entity
                .Property(x => x.DataVenda)
                .IsRequired();

            entity
                .Property(x => x.Cliente)
                .HasMaxLength(150)
                .IsRequired();

            entity
                .Property(x => x.CpfCnpjCliente)
                .HasMaxLength(14);

            entity
                .Property(x => x.ValorVenda)
                .HasPrecision(18, 2)
                .IsRequired();

            entity
                .Property(x => x.PercentualComissao)
                .HasPrecision(5, 2)
                .IsRequired();

            entity
                .Property(x => x.ValorComissao)
                .HasPrecision(18, 2)
                .IsRequired();

            entity
                .Property(x => x.Status)
                .IsRequired();

            entity
                .Property(x => x.DataPagamento);

            entity
                .Property(x => x.Observacao)
                .HasMaxLength(500);

            entity
                .Property(x => x.DataCadastro)
                .IsRequired();

            entity
                .Property(x => x.DataAtualizacao);

            entity
                .HasOne(x => x.Vendedor)
                .WithMany(x => x.Lancamentos)
                .HasForeignKey(x => x.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(x => x.ClienteCadastro)
                .WithMany(x => x.Lancamentos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}