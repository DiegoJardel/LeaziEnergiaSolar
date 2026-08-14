using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaziEnergiaSolar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VincularLancamentosClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Lancamentos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE Lancamentos
                SET ClienteId = (
                    SELECT Id
                    FROM Clientes
                    WHERE Clientes.CpfCnpj = Lancamentos.CpfCnpjCliente
                )
                WHERE CpfCnpjCliente IS NOT NULL
                  AND CpfCnpjCliente <> ''
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_ClienteId",
                table: "Lancamentos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamentos_Clientes_ClienteId",
                table: "Lancamentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lancamentos_Clientes_ClienteId",
                table: "Lancamentos");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_ClienteId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Lancamentos");
        }
    }
}
