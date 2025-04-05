using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_ChabotMessage_Remove_UserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEGS1ClKlMS2aG0y3a1K0m0TkSjUordGBwRICNHMKAh1i0ebhPJpw8O8n+dS62CFXbg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEFvqAMZ18QLf9+mO0f7s+1kK6MNdlA/eAR/pthzIYNFcP/5WJ9wWnJVDZzUcjIqETQ==");
        }
    }
}
