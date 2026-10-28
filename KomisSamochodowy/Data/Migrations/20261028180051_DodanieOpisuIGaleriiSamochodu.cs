using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KomisSamochodowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanieOpisuIGaleriiSamochodu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GaleriaZdjec",
                table: "Samochody",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Opis",
                table: "Samochody",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GaleriaZdjec",
                table: "Samochody");

            migrationBuilder.DropColumn(
                name: "Opis",
                table: "Samochody");
        }
    }
}
