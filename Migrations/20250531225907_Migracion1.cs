using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_Periodo_2.Migrations
{
    /// <inheritdoc />
    public partial class CrearCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Apellido1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Apellido2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CantidadPrestamosDisponibles = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "EstadoPrestamos",
                columns: table => new
                {
                    _IdEstado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    _Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    _Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    _Activo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoPrestamos", x => x._IdEstado);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreDeUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Contrasena = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Apellido1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Apellido2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "EstadoPrestamos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
