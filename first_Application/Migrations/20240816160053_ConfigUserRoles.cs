using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace first_Application.Migrations
{
    /// <inheritdoc />
    public partial class ConfigUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a1f7a5bd-61f0-4208-adf5-fe51e6d2368c", null, "Admin", "ADMIN" },
                    { "f1abcd75-bed2-4a22-9dd1-f1f1853096bc", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a1f7a5bd-61f0-4208-adf5-fe51e6d2368c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f1abcd75-bed2-4a22-9dd1-f1f1853096bc");
        }
    }
}
