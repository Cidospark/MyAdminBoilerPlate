using Microsoft.EntityFrameworkCore.Migrations;

namespace MyAdminBoilerPlate.Migrations
{
    public partial class SeedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "City", "Country", "DOB", "Email", "FirstName", "Gender", "LastName", "Nationality", "PhoneNumber", "Photo", "Street" },
                values: new object[] { 1, null, null, 0, null, "John", 0, "Doe", null, null, null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }
    }
}
