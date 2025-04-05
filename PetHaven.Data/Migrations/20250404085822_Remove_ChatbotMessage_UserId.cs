using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class Remove_ChatbotMessage_UserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatBotMessages_Users_UserId",
                table: "ChatBotMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatBotMessages_UserId",
                table: "ChatBotMessages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChatBotMessages");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEAKdMXo0j5fgq9he9Llc3mzRJ3ixXkBep80OgKFa0hVWnKgXp2ZXTU2jzfkG8QBn1g==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ChatBotMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEGS1ClKlMS2aG0y3a1K0m0TkSjUordGBwRICNHMKAh1i0ebhPJpw8O8n+dS62CFXbg==");

            migrationBuilder.CreateIndex(
                name: "IX_ChatBotMessages_UserId",
                table: "ChatBotMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBotMessages_Users_UserId",
                table: "ChatBotMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
