using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class migracion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    IdStock = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.IdStock);
                });

            migrationBuilder.CreateTable(
                name: "Libros",
                columns: table => new
                {
                    IdLibro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStock = table.Column<int>(type: "int", nullable: true),
                    ClasificacionEdad = table.Column<int>(type: "int", nullable: true),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ISBN = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.IdLibro);
                    table.ForeignKey(
                        name: "FK_Libros_Stocks_IdStock",
                        column: x => x.IdStock,
                        principalTable: "Stocks",
                        principalColumn: "IdStock");
                });

            migrationBuilder.CreateTable(
                name: "CopiaLibros",
                columns: table => new
                {
                    IdCopiaLibro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLibro = table.Column<int>(type: "int", nullable: true),
                    IdEstadoCopiaLibro = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CopiaLibros", x => x.IdCopiaLibro);
                    table.ForeignKey(
                        name: "FK_CopiaLibros_EstadoPrestamos_IdEstadoCopiaLibro",
                        column: x => x.IdEstadoCopiaLibro,
                        principalTable: "EstadoPrestamos",
                        principalColumn: "_IdEstado");
                    table.ForeignKey(
                        name: "FK_CopiaLibros_Libros_IdLibro",
                        column: x => x.IdLibro,
                        principalTable: "Libros",
                        principalColumn: "IdLibro");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CopiaLibros_IdEstadoCopiaLibro",
                table: "CopiaLibros",
                column: "IdEstadoCopiaLibro");

            migrationBuilder.CreateIndex(
                name: "IX_CopiaLibros_IdLibro",
                table: "CopiaLibros",
                column: "IdLibro");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_IdStock",
                table: "Libros",
                column: "IdStock");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CopiaLibros");

            migrationBuilder.DropTable(
                name: "Libros");

            migrationBuilder.DropTable(
                name: "Stocks");
        }
    }
}
