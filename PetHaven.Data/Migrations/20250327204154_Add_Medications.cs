using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHaven.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Medications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkups_Pets_PetId",
                table: "Checkups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Checkups",
                table: "Checkups");

            migrationBuilder.RenameTable(
                name: "Checkups",
                newName: "Checkup");

            migrationBuilder.RenameIndex(
                name: "IX_Checkups_PetId",
                table: "Checkup",
                newName: "IX_Checkup_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checkup",
                table: "Checkup",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Dosage = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Frequency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Notes = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medications_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_PetId",
                table: "Medications",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkup_Pets_PetId",
                table: "Checkup",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkup_Pets_PetId",
                table: "Checkup");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Checkup",
                table: "Checkup");

            migrationBuilder.RenameTable(
                name: "Checkup",
                newName: "Checkups");

            migrationBuilder.RenameIndex(
                name: "IX_Checkup_PetId",
                table: "Checkups",
                newName: "IX_Checkups_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checkups",
                table: "Checkups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkups_Pets_PetId",
                table: "Checkups",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
