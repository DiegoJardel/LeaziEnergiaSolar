using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaziEnergiaSolar.Infrastructure.Migrations;

public partial class AjustaAuditoriaCliente : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "DataAlteracao",
            table: "Clientes",
            newName: "DataAtualizacao");

        migrationBuilder.DropColumn(
            name: "DataNascimentoAbertura",
            table: "Clientes");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "DataNascimentoAbertura",
            table: "Clientes",
            type: "TEXT",
            nullable: true);

        migrationBuilder.RenameColumn(
            name: "DataAtualizacao",
            table: "Clientes",
            newName: "DataAlteracao");
    }
}
