using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FarmaciaSistema.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataForProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Descripcion", "FechaCaducidad", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 1, "Caja con 20 tabletas", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Paracetamol 500mg", 45.50m, 100 },
                    { 2, "Suspensión pediátrica 100ml", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Amoxicilina 250mg", 120.00m, 50 },
                    { 3, "Caja con 10 tabletas", new DateTime(2027, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Loratadina 10mg", 75.00m, 75 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
