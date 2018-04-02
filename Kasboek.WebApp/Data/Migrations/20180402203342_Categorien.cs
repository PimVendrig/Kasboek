using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class Categorien : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategorieId",
                table: "Transacties",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandaardCategorieId",
                table: "Rekeningen",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categorieen",
                columns: table => new
                {
                    CategorieId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Omschrijving = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorieen", x => x.CategorieId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacties_CategorieId",
                table: "Transacties",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_Rekeningen_StandaardCategorieId",
                table: "Rekeningen",
                column: "StandaardCategorieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rekeningen_Categorieen_StandaardCategorieId",
                table: "Rekeningen",
                column: "StandaardCategorieId",
                principalTable: "Categorieen",
                principalColumn: "CategorieId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_Categorieen_CategorieId",
                table: "Transacties",
                column: "CategorieId",
                principalTable: "Categorieen",
                principalColumn: "CategorieId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rekeningen_Categorieen_StandaardCategorieId",
                table: "Rekeningen");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Categorieen_CategorieId",
                table: "Transacties");

            migrationBuilder.DropTable(
                name: "Categorieen");

            migrationBuilder.DropIndex(
                name: "IX_Transacties_CategorieId",
                table: "Transacties");

            migrationBuilder.DropIndex(
                name: "IX_Rekeningen_StandaardCategorieId",
                table: "Rekeningen");

            migrationBuilder.DropColumn(
                name: "CategorieId",
                table: "Transacties");

            migrationBuilder.DropColumn(
                name: "StandaardCategorieId",
                table: "Rekeningen");
        }
    }
}
