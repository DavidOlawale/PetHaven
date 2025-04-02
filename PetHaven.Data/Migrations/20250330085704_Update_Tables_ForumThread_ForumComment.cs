using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Tables_ForumThread_ForumComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "ForumThreads",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ForumThreads",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ForumThreads",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ForumThreads",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ForumThreads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "ForumComments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ForumComments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ForumComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEEcittd3ibeAypM3p3nzJ6y61NRPG+Yls1wMEBy1iwOOYjGkSX4dmomH6ee6dh/JUw==");

            migrationBuilder.CreateIndex(
                name: "IX_ForumThreads_UserId",
                table: "ForumThreads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumComments_UserId",
                table: "ForumComments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumComments_Users_UserId",
                table: "ForumComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumThreads_Users_UserId",
                table: "ForumThreads",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumComments_Users_UserId",
                table: "ForumComments");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumThreads_Users_UserId",
                table: "ForumThreads");

            migrationBuilder.DropIndex(
                name: "IX_ForumThreads_UserId",
                table: "ForumThreads");

            migrationBuilder.DropIndex(
                name: "IX_ForumComments_UserId",
                table: "ForumComments");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "ForumThreads");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ForumThreads");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ForumThreads");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ForumThreads");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ForumThreads");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "ForumComments");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ForumComments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ForumComments");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEEaJzj8wZTuZebq49iWNmK/gqjbzSs1hTL2VoE6lekk1QmNqUrX+pWc/x6GS8nw/kQ==");
        }
    }
}
