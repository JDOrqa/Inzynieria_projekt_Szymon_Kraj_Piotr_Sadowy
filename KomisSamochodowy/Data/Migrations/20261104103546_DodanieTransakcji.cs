using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KomisSamochodowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanieTransakcji : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ZdjecieUrl",
                table: "Samochody",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "Transakcje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataTransakcji = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kwota = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SprzedawcaId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KlientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SamochodId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transakcje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transakcje_Samochody_SamochodId",
                        column: x => x.SamochodId,
                        principalTable: "Samochody",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transakcje_SamochodId",
                table: "Transakcje",
                column: "SamochodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transakcje");

            migrationBuilder.AlterColumn<string>(
                name: "ZdjecieUrl",
                table: "Samochody",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
