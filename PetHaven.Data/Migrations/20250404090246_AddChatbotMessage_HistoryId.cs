using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChatbotMessage_HistoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatBotMessages_ChatBotHistories_ChatBotHistoryId",
                table: "ChatBotMessages");

            migrationBuilder.AlterColumn<int>(
                name: "ChatBotHistoryId",
                table: "ChatBotMessages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEAWN0dBsKiFxynReJCsgR0wc2itnBJYXhWgeAmazOyFm2iHt5AWwYx8cfLgpw7/6Qw==");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBotMessages_ChatBotHistories_ChatBotHistoryId",
                table: "ChatBotMessages",
                column: "ChatBotHistoryId",
                principalTable: "ChatBotHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatBotMessages_ChatBotHistories_ChatBotHistoryId",
                table: "ChatBotMessages");

            migrationBuilder.AlterColumn<int>(
                name: "ChatBotHistoryId",
                table: "ChatBotMessages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEAKdMXo0j5fgq9he9Llc3mzRJ3ixXkBep80OgKFa0hVWnKgXp2ZXTU2jzfkG8QBn1g==");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBotMessages_ChatBotHistories_ChatBotHistoryId",
                table: "ChatBotMessages",
                column: "ChatBotHistoryId",
                principalTable: "ChatBotHistories",
                principalColumn: "Id");
        }
    }
}
