using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Payments_GatewayResponse_Nullible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GatewayResponse",
                table: "Payments",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEJi6EeowVKxP0P2XwOLC4BP68P7hZioPe4CK6/6l/4h1KaCscC/x1Ge3cHjh9G+pMw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "GatewayResponse",
                keyValue: null,
                column: "GatewayResponse",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "GatewayResponse",
                table: "Payments",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEPIR2u+MkfbQBorAF90OCE5ZMIp+8+nwhvVorMAlryFnlg5k7XGjcX6OxAqu0TccLw==");
        }
    }
}
