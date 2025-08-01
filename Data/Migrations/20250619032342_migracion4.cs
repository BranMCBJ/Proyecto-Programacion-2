using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class migracion4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CopiaLibros_EstadoPrestamos_IdEstadoCopiaLibro",
                table: "CopiaLibros");

            migrationBuilder.DropForeignKey(
                name: "FK_CopiaLibros_Libros_IdLibro",
                table: "CopiaLibros");

            migrationBuilder.DropForeignKey(
                name: "FK_Libros_Stocks_IdStock",
                table: "Libros");

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Libros",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdStock",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "Libros",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaPublicacion",
                table: "Libros",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Libros",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClasificacionEdad",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdLibro",
                table: "CopiaLibros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdEstadoCopiaLibro",
                table: "CopiaLibros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CopiaLibros_EstadoCopiaLibro_IdEstadoCopiaLibro",
                table: "CopiaLibros",
                column: "IdEstadoCopiaLibro",
                principalTable: "EstadoCopiaLibro",
                principalColumn: "IdEstadoCopialibro",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CopiaLibros_Libros_IdLibro",
                table: "CopiaLibros",
                column: "IdLibro",
                principalTable: "Libros",
                principalColumn: "IdLibro",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Libros_Stocks_IdStock",
                table: "Libros",
                column: "IdStock",
                principalTable: "Stocks",
                principalColumn: "IdStock",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CopiaLibros_EstadoCopiaLibro_IdEstadoCopiaLibro",
                table: "CopiaLibros");

            migrationBuilder.DropForeignKey(
                name: "FK_CopiaLibros_Libros_IdLibro",
                table: "CopiaLibros");

            migrationBuilder.DropForeignKey(
                name: "FK_Libros_Stocks_IdStock",
                table: "Libros");

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "Stocks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Libros",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "IdStock",
                table: "Libros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "Libros",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaPublicacion",
                table: "Libros",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Libros",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<int>(
                name: "ClasificacionEdad",
                table: "Libros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdLibro",
                table: "CopiaLibros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdEstadoCopiaLibro",
                table: "CopiaLibros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CopiaLibros_EstadoPrestamos_IdEstadoCopiaLibro",
                table: "CopiaLibros",
                column: "IdEstadoCopiaLibro",
                principalTable: "EstadoPrestamos",
                principalColumn: "_IdEstado");

            migrationBuilder.AddForeignKey(
                name: "FK_CopiaLibros_Libros_IdLibro",
                table: "CopiaLibros",
                column: "IdLibro",
                principalTable: "Libros",
                principalColumn: "IdLibro");

            migrationBuilder.AddForeignKey(
                name: "FK_Libros_Stocks_IdStock",
                table: "Libros",
                column: "IdStock",
                principalTable: "Stocks",
                principalColumn: "IdStock");
        }
    }
}
