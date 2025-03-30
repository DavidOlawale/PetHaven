using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seed_AdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PasswordHash", "Role", "ZipCode" },
                values: new object[] { 1, "pethaven_superadmin@gmail.com", "David", "Olaniran", "AQAAAAEAACcQAAAAELTI/Ud1Diwpcv2azGZKwvK/qLmzbMazW+oV81bueiUAk2aZH4w6TOIIX2B2QTc6iA==", "Administrator", "10027" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
