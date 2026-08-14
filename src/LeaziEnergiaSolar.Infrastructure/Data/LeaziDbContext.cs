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
        });
    }
}