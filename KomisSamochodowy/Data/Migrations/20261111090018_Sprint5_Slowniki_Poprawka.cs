using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KomisSamochodowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class Sprint5_Slowniki_Poprawka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marka",
                table: "Samochody");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Samochody");

            migrationBuilder.AddColumn<int>(
                name: "MarkaId",
                table: "Samochody",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModelId",
                table: "Samochody",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Marki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marki", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modele",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MarkaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modele", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modele_Marki_MarkaId",
                        column: x => x.MarkaId,
                        principalTable: "Marki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Samochody_MarkaId",
                table: "Samochody",
                column: "MarkaId");

            migrationBuilder.CreateIndex(
                name: "IX_Samochody_ModelId",
                table: "Samochody",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Modele_MarkaId",
                table: "Modele",
                column: "MarkaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samochody_Marki_MarkaId",
                table: "Samochody",
                column: "MarkaId",
                principalTable: "Marki",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Samochody_Modele_ModelId",
                table: "Samochody",
                column: "ModelId",
                principalTable: "Modele",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samochody_Marki_MarkaId",
                table: "Samochody");

            migrationBuilder.DropForeignKey(
                name: "FK_Samochody_Modele_ModelId",
                table: "Samochody");

            migrationBuilder.DropTable(
                name: "Modele");

            migrationBuilder.DropTable(
                name: "Marki");

            migrationBuilder.DropIndex(
                name: "IX_Samochody_MarkaId",
                table: "Samochody");

            migrationBuilder.DropIndex(
                name: "IX_Samochody_ModelId",
                table: "Samochody");

            migrationBuilder.DropColumn(
                name: "MarkaId",
                table: "Samochody");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Samochody");

            migrationBuilder.AddColumn<string>(
                name: "Marka",
                table: "Samochody",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Samochody",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
