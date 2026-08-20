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
            entity.HasIndex(usuario => usuario.Login)
                .IsUnique();

            entity.Property(usuario => usuario.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(usuario => usuario.Login)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(usuario => usuario.SenhaHash)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasIndex(cliente => cliente.CpfCnpj)
                .IsUnique();

            entity.Property(cliente => cliente.NomeRazaoSocial)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(cliente => cliente.NomeFantasia)
                .HasMaxLength(150);

            entity.Property(cliente => cliente.CpfCnpj)
                .HasMaxLength(14);

            entity.Property(cliente => cliente.RgInscricaoEstadual)
                .HasMaxLength(20);

            entity.Property(cliente => cliente.Telefone)
                .HasMaxLength(11);

            entity.Property(cliente => cliente.WhatsApp)
                .HasMaxLength(11);

            entity.Property(cliente => cliente.Email)
                .HasMaxLength(150);

            entity.Property(cliente => cliente.Cep)
                .HasMaxLength(8);

            entity.Property(cliente => cliente.Logradouro)
                .HasMaxLength(150);

            entity.Property(cliente => cliente.Numero)
                .HasMaxLength(20);

            entity.Property(cliente => cliente.Complemento)
                .HasMaxLength(100);

            entity.Property(cliente => cliente.Bairro)
                .HasMaxLength(100);

            entity.Property(cliente => cliente.Cidade)
                .HasMaxLength(100);

            entity.Property(cliente => cliente.CodigoIbgeCidade)
                .HasMaxLength(7);

            entity.Property(cliente => cliente.Estado)
                .HasMaxLength(100);

            entity.Property(cliente => cliente.SiglaUf)
                .HasMaxLength(2);

            entity.Property(cliente => cliente.CodigoIbgeUf)
                .HasMaxLength(2);

            entity.Property(cliente => cliente.PontoReferencia)
                .HasMaxLength(200);

            entity.Property(cliente => cliente.Observacao)
                .HasMaxLength(1000);

            entity.HasOne(cliente => cliente.Municipio)
                .WithMany(municipio => municipio.Clientes)
                .HasForeignKey(cliente => cliente.MunicipioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasIndex(estado => estado.CodigoIbge)
                .IsUnique();

            entity.Property(estado => estado.CodigoIbge)
                .HasMaxLength(2)
                .IsRequired();

            entity.Property(estado => estado.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(estado => estado.Sigla)
                .HasMaxLength(2)
                .IsRequired();
        });

        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.HasIndex(municipio => municipio.CodigoIbge)
                .IsUnique();

            entity.Property(municipio => municipio.CodigoIbge)
                .HasMaxLength(7)
                .IsRequired();

            entity.Property(municipio => municipio.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasOne(municipio => municipio.Estado)
                .WithMany(estado => estado.Municipios)
                .HasForeignKey(municipio => municipio.EstadoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Vendedor>(entity =>
        {
            entity.HasIndex(vendedor => vendedor.CpfCnpj)
                .IsUnique();

            entity.Property(vendedor => vendedor.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(vendedor => vendedor.CpfCnpj)
                .HasMaxLength(14)
                .IsRequired();

            entity.Property(vendedor => vendedor.Telefone)
                .HasMaxLength(11)
                .IsRequired();

            entity.Property(vendedor => vendedor.Email)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(vendedor =>
                    vendedor.PercentualComissao)
                .HasPrecision(5, 2);
        });
        modelBuilder.Entity<CategoriaEquipamento>(entity =>
        {
            entity.HasIndex(x => x.Descricao).IsUnique();
            entity.Property(x => x.Descricao).HasMaxLength(100).IsRequired().UseCollation("NOCASE");
            entity.Property(x => x.Observacao).HasMaxLength(500);
            entity.Property(x => x.DataCadastro).IsRequired();
            entity.Property(x => x.DataAtualizacao);
        });

        modelBuilder.Entity<ModeloEquipamento>(entity =>
        {
            entity.HasIndex(x => new { x.MarcaId, x.Nome }).IsUnique();
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired().UseCollation("NOCASE");
            entity.Property(x => x.Observacao).HasMaxLength(500);
            entity.Property(x => x.DataCadastro).IsRequired();
            entity.Property(x => x.DataAtualizacao);

            entity
                .HasOne(x => x.Marca)
                .WithMany(x => x.Modelos)
                .HasForeignKey(x => x.MarcaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasIndex(x => x.Nome).IsUnique();
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired().UseCollation("NOCASE");
            entity.Property(x => x.Observacao).HasMaxLength(500);
            entity.Property(x => x.DataCadastro).IsRequired();
            entity.Property(x => x.DataAtualizacao);
        });

        modelBuilder.Entity<UnidadeMedida>(entity =>
        {
            entity.HasIndex(x => x.Sigla).IsUnique();
            entity.Property(x => x.Sigla).HasMaxLength(10).IsRequired().UseCollation("NOCASE");
            entity.Property(x => x.Descricao).HasMaxLength(100).IsRequired();
            entity.Property(x => x.DataCadastro).IsRequired();
            entity.Property(x => x.DataAtualizacao);
        });

        modelBuilder.Entity<Equipamento>(entity =>
        {
            entity.Property(x => x.Descricao).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Modelo).HasMaxLength(100);
            entity.Property(x => x.Observacao).HasMaxLength(1000);
            entity.Property(x => x.DataCadastro).IsRequired();
            entity.Property(x => x.DataAtualizacao);

            entity.HasIndex(x => new { x.Descricao, x.MarcaId, x.Modelo }).IsUnique();

            entity.HasOne(x => x.CategoriaEquipamento)
                .WithMany(x => x.Equipamentos)
                .HasForeignKey(x => x.CategoriaEquipamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Marca)
                .WithMany(x => x.Equipamentos)
                .HasForeignKey(x => x.MarcaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.UnidadeMedida)
                .WithMany(x => x.Equipamentos)
                .HasForeignKey(x => x.UnidadeMedidaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Fornecedor>(entity =>
        {
            entity.HasIndex(x => x.CpfCnpj).IsUnique();
            entity.Property(x => x.NomeRazaoSocial).HasMaxLength(150).IsRequired();
            entity.Property(x => x.NomeFantasia).HasMaxLength(150);
            entity.Property(x => x.CpfCnpj).HasMaxLength(14);
            entity.Property(x => x.Telefone).HasMaxLength(11);
            entity.Property(x => x.Email).HasMaxLength(150);
            entity.Property(x => x.ContatoResponsavel).HasMaxLength(150);
            entity.Property(x => x.Observacao).HasMaxLength(1000);
            entity.Property(x => x.DataCadastro).IsRequired();
            entity.Property(x => x.DataAtualizacao);
        });

        modelBuilder.Entity<Lancamento>(entity =>
        {
            entity.Property(lancamento =>
                    lancamento.ValorVenda)
                .HasPrecision(18, 2);

            entity.Property(lancamento =>
                    lancamento.PercentualComissao)
                .HasPrecision(5, 2);

            entity.Property(lancamento =>
                    lancamento.ValorComissao)
                .HasPrecision(18, 2);

            entity.HasOne(lancamento =>
                    lancamento.Vendedor)
                .WithMany(vendedor =>
                    vendedor.Lancamentos)
                .HasForeignKey(lancamento =>
                    lancamento.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(lancamento =>
                    lancamento.ClienteCadastro)
                .WithMany(cliente =>
                    cliente.Lancamentos)
                .HasForeignKey(lancamento =>
                    lancamento.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(lancamento =>
                    lancamento.Usuario)
                .WithMany()
                .HasForeignKey(lancamento =>
                    lancamento.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}