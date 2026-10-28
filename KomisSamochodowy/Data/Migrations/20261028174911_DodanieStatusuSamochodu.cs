using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KomisSamochodowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanieStatusuSamochodu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Samochody",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Samochody");
        }
    }
}
