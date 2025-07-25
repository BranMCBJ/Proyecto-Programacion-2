using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_Periodo_2.Migrations
{
    /// <inheritdoc />
    public partial class migracion5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamo_EstadoPrestamos_IdEstadoPrestamo",
                table: "Prestamo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadoPrestamos",
                table: "EstadoPrestamos");

            migrationBuilder.RenameTable(
                name: "EstadoPrestamos",
                newName: "EstadoPrestamo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadoPrestamo",
                table: "EstadoPrestamo",
                column: "_IdEstado");

            migrationBuilder.CreateTable(
                name: "CopiasLibrosPrestamos",
                columns: table => new
                {
                    IdRelacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCopiaLibro = table.Column<int>(type: "int", nullable: false),
                    CopiaLibroIdCopiaLibro = table.Column<int>(type: "int", nullable: true),
                    IdPrestamo = table.Column<int>(type: "int", nullable: false),
                    PrestamoIdPrestamo = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CopiasLibrosPrestamos", x => x.IdRelacion);
                    table.ForeignKey(
                        name: "FK_CopiasLibrosPrestamos_CopiaLibros_CopiaLibroIdCopiaLibro",
                        column: x => x.CopiaLibroIdCopiaLibro,
                        principalTable: "CopiaLibros",
                        principalColumn: "IdCopiaLibro");
                    table.ForeignKey(
                        name: "FK_CopiasLibrosPrestamos_Prestamo_PrestamoIdPrestamo",
                        column: x => x.PrestamoIdPrestamo,
                        principalTable: "Prestamo",
                        principalColumn: "IdPrestamo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CopiasLibrosPrestamos_CopiaLibroIdCopiaLibro",
                table: "CopiasLibrosPrestamos",
                column: "CopiaLibroIdCopiaLibro");

            migrationBuilder.CreateIndex(
                name: "IX_CopiasLibrosPrestamos_PrestamoIdPrestamo",
                table: "CopiasLibrosPrestamos",
                column: "PrestamoIdPrestamo");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamo_EstadoPrestamo_IdEstadoPrestamo",
                table: "Prestamo",
                column: "IdEstadoPrestamo",
                principalTable: "EstadoPrestamo",
                principalColumn: "_IdEstado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamo_EstadoPrestamo_IdEstadoPrestamo",
                table: "Prestamo");

            migrationBuilder.DropTable(
                name: "CopiasLibrosPrestamos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadoPrestamo",
                table: "EstadoPrestamo");

            migrationBuilder.RenameTable(
                name: "EstadoPrestamo",
                newName: "EstadoPrestamos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadoPrestamos",
                table: "EstadoPrestamos",
                column: "_IdEstado");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamo_EstadoPrestamos_IdEstadoPrestamo",
                table: "Prestamo",
                column: "IdEstadoPrestamo",
                principalTable: "EstadoPrestamos",
                principalColumn: "_IdEstado");
        }
    }
}
