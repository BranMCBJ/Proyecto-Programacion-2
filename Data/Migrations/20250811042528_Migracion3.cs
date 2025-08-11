using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Migracion3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 1, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin", "3" },
                    { 2, "Nombre", "Julian", "3" },
                    { 3, "NombreUsuario", "Julai", "3" },
                    { 4, "Apellido1", "Ceciliano", "3" },
                    { 5, "Apellido2", "Picado", "3" },
                    { 6, "Cedula", "305760805", "3" },
                    { 7, "UrlImagen", "/Usuario/Imagenes/Foto perfil.png", "3" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6c96503-dc5c-4e64-ad6f-b03defb017ba", "AQAAAAIAAYagAAAAEGbc2KKWAgDn6j62+nWTmx70piySlO93ZbaECxN8o0qL0rj6/nx+l4ijxY2G1GeywQ==", "6d928c39-f975-4c7a-b181-8afa5e463376" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1fa74e3e-13f8-467d-8c67-e98153ecf033", "AQAAAAIAAYagAAAAECn33hCZY9WzqAE052hVEiF8TTTgjJE6XpNujrFbJen+WprZSt7+KYVqqsHkEvJI3w==", "1049ef43-2e28-42db-8e2d-364a203807da" });
        }
    }
}
