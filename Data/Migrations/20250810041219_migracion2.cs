using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class migracion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Libros_Stocks_IdStock",
                table: "Libros");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Libros_IdStock",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "IdStock",
                table: "Libros");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Libros",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Libros");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Libros",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdStock",
                table: "Libros",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    IdStock = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activo = table.Column<bool>(type: "bit", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.IdStock);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Libros_IdStock",
                table: "Libros",
                column: "IdStock");

            migrationBuilder.AddForeignKey(
                name: "FK_Libros_Stocks_IdStock",
                table: "Libros",
                column: "IdStock",
                principalTable: "Stocks",
                principalColumn: "IdStock");
        }
    }
}
