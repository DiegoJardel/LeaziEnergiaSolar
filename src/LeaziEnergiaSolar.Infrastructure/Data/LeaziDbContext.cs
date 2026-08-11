using LeaziEnergiaSolar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaziEnergiaSolar.Infrastructure.Data;

public class LeaziDbContext : DbContext
{
    public LeaziDbContext(DbContextOptions<LeaziDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Vendedor> Vendedores => Set<Vendedor>();

    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

            entity.Property(vendedor => vendedor.PercentualComissao)
                .HasPrecision(5, 2);
        });

        modelBuilder.Entity<Lancamento>(entity =>
        {
            entity.Property(lancamento => lancamento.ValorVenda)
                .HasPrecision(18, 2);

            entity.Property(lancamento => lancamento.PercentualComissao)
                .HasPrecision(5, 2);

            entity.Property(lancamento => lancamento.ValorComissao)
                .HasPrecision(18, 2);

            entity.HasOne(lancamento => lancamento.Vendedor)
                .WithMany(vendedor => vendedor.Lancamentos)
                .HasForeignKey(lancamento => lancamento.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
