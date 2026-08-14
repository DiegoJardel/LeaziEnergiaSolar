using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaziEnergiaSolar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VincularLancamentosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Lancamentos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_UsuarioId",
                table: "Lancamentos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamentos_Usuarios_UsuarioId",
                table: "Lancamentos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lancamentos_Usuarios_UsuarioId",
                table: "Lancamentos");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_UsuarioId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Lancamentos");
        }
    }
}
