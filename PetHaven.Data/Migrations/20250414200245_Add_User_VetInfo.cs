using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_User_VetInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClinicName",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClinicName", "LicenseNumber", "PasswordHash", "Specialization" },
                values: new object[] { null, null, "AQAAAAEAACcQAAAAEHTMpAKC0cqHbDLl+38ksnVKH2Wj6O1C3EuJ7yzEFNMTOGNm1qTKvnfzQDv68wN7Jw==", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClinicName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHdORbpnFk2eNG0+NeI+lUAormIcAe6FHyc2GfJjGcYBbZVzojKQxfiwI6Ahm3RwfA==");
        }
    }
}
