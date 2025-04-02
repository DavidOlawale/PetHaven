using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Pet_Weight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "Pets",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEJiQD1bKZVo1G1sOJL+LFHvc8Qs44+g3xsQx4HPCM5FnVToMSMb57wt1J4LeWVFJTg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                table: "Pets",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEEcittd3ibeAypM3p3nzJ6y61NRPG+Yls1wMEBy1iwOOYjGkSX4dmomH6ee6dh/JUw==");
        }
    }
}
