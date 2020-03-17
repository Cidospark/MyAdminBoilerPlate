using Microsoft.EntityFrameworkCore.Migrations;

namespace MyAdminBoilerPlate.Migrations
{
    public partial class seederMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "City", "ConcurrencyStamp", "Country", "DOB", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "Nationality", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Photo", "SecurityStamp", "Street", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4e9fa395-bb5b-4f20-9628-c37dc4447e6b", 0, null, "373b7e38-c281-40c7-b040-237d54d3ca90", null, 0, "sample@sample.com", false, "John", 0, "Doe", false, null, null, "SAMPLE@SAMPLE.COM", "SAMPLE@SAMPLE.COM", "8c97f2d4-a383-477d-bbb6-a701fc9fc8aa", null, false, null, "4ce7aac2-98ed-4465-b8da-d44d5e9ab9b6", null, false, "sample@sample.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4e9fa395-bb5b-4f20-9628-c37dc4447e6b");
        }
    }
}
