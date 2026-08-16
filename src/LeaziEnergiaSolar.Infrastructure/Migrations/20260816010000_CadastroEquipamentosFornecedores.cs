using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaziEnergiaSolar.Infrastructure.Migrations;

public partial class CadastroEquipamentosFornecedores : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CategoriasEquipamento",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Descricao = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, collation: "NOCASE"),
                Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_CategoriasEquipamento", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Marcas",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, collation: "NOCASE"),
                Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Marcas", x => x.Id));

        migrationBuilder.CreateTable(
            name: "UnidadesMedida",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Sigla = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, collation: "NOCASE"),
                Descricao = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                PermiteQuantidadeDecimal = table.Column<bool>(type: "INTEGER", nullable: false),
                Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_UnidadesMedida", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Fornecedores",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                TipoPessoa = table.Column<int>(type: "INTEGER", nullable: false),
                NomeRazaoSocial = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                NomeFantasia = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                CpfCnpj = table.Column<string>(type: "TEXT", maxLength: 14, nullable: true),
                Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                ContatoResponsavel = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                Observacao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Fornecedores", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Equipamentos",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Descricao = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                CategoriaEquipamentoId = table.Column<int>(type: "INTEGER", nullable: false),
                MarcaId = table.Column<int>(type: "INTEGER", nullable: true),
                Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                UnidadeMedidaId = table.Column<int>(type: "INTEGER", nullable: false),
                ValorCusto = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                EstoqueMinimo = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                Observacao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Equipamentos", x => x.Id);
                table.ForeignKey("FK_Equipamentos_CategoriasEquipamento_CategoriaEquipamentoId", x => x.CategoriaEquipamentoId, "CategoriasEquipamento", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_Equipamentos_Marcas_MarcaId", x => x.MarcaId, "Marcas", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_Equipamentos_UnidadesMedida_UnidadeMedidaId", x => x.UnidadeMedidaId, "UnidadesMedida", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(name: "IX_CategoriasEquipamento_Descricao", table: "CategoriasEquipamento", column: "Descricao", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Marcas_Nome", table: "Marcas", column: "Nome", unique: true);
        migrationBuilder.CreateIndex(name: "IX_UnidadesMedida_Sigla", table: "UnidadesMedida", column: "Sigla", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Fornecedores_CpfCnpj", table: "Fornecedores", column: "CpfCnpj", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Equipamentos_CategoriaEquipamentoId", table: "Equipamentos", column: "CategoriaEquipamentoId");
        migrationBuilder.CreateIndex(name: "IX_Equipamentos_MarcaId", table: "Equipamentos", column: "MarcaId");
        migrationBuilder.CreateIndex(name: "IX_Equipamentos_UnidadeMedidaId", table: "Equipamentos", column: "UnidadeMedidaId");
        migrationBuilder.CreateIndex(name: "IX_Equipamentos_Descricao_MarcaId_Modelo", table: "Equipamentos", columns: new[] { "Descricao", "MarcaId", "Modelo" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Equipamentos");
        migrationBuilder.DropTable(name: "Fornecedores");
        migrationBuilder.DropTable(name: "CategoriasEquipamento");
        migrationBuilder.DropTable(name: "Marcas");
        migrationBuilder.DropTable(name: "UnidadesMedida");
    }
}
