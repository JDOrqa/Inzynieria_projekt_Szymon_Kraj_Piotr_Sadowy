using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KomisSamochodowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanieSzczegolowSamochodu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Paliwo",
                table: "Samochody",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Przebieg",
                table: "Samochody",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ZdjecieUrl",
                table: "Samochody",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paliwo",
                table: "Samochody");

            migrationBuilder.DropColumn(
                name: "Przebieg",
                table: "Samochody");

            migrationBuilder.DropColumn(
                name: "ZdjecieUrl",
                table: "Samochody");
        }
    }
}
