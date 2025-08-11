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
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1" });

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "3", 0, "1fa74e3e-13f8-467d-8c67-e98153ecf033", "cecilianojulian64@gmail.com", true, false, null, "CECILIANOJULIAN64@GMAIL.COM", "JULAI", "AQAAAAIAAYagAAAAECn33hCZY9WzqAE052hVEiF8TTTgjJE6XpNujrFbJen+WprZSt7+KYVqqsHkEvJI3w==", "12345678", false, "1049ef43-2e28-42db-8e2d-364a203807da", false, "Julai" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "3" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Apellido1", "Apellido2", "Cedula", "Nombre", "NombreUsuario", "UrlImagen" },
                values: new object[] { "3", true, "Ceciliano", "Picado", "305760805", "Julian", "Julai", "/Usuario/Imagenes/d724626d-b41f-47d7-acec-8b85fe3f8de5.jpg" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "3" });

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1", 0, "b5911177-5c30-4da4-b65d-ce2b6b7c14b5", "cecilianojulian64@gmail.com", false, false, null, null, null, "AQAAAAIAAYagAAAAEBGR+X53pT9PpC8ArEVzpaFsR6n5psbSifqnhL7knM1vO25ZUp1deAOkBV9+whHS2w==", "12345678", false, "6fc6bbcc-2976-4df9-9051-1a90f3b11753", false, "Julai" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Apellido1", "Apellido2", "Cedula", "Nombre", "NombreUsuario", "UrlImagen" },
                values: new object[] { "1", true, "Ceciliano", "Picado", "305760805", "Julian", "Julai", "/Usuario/Imagenes/d724626d - b41f - 47d7 - acec - 8b85fe3f8de5.jpg" });
        }
    }
}
