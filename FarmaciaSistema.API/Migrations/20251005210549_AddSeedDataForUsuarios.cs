using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmaciaSistema.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataForUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "NombreUsuario", "PasswordHash", "Rol" },
                values: new object[] { 1, "admin", "1234", "Administrador" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
