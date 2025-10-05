using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FarmaciaSistema.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataForProveedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Proveedores",
                columns: new[] { "Id", "Contacto", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, "Juan Pérez", "Proveedor Principal", "6621234567" },
                    { 2, "Ana García", "Bayer de México", "6627654321" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Proveedores",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Proveedores",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
