using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAppoinment_Venue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Venue",
                table: "Appointments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHdORbpnFk2eNG0+NeI+lUAormIcAe6FHyc2GfJjGcYBbZVzojKQxfiwI6Ahm3RwfA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Venue",
                table: "Appointments");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEAWN0dBsKiFxynReJCsgR0wc2itnBJYXhWgeAmazOyFm2iHt5AWwYx8cfLgpw7/6Qw==");
        }
    }
}
