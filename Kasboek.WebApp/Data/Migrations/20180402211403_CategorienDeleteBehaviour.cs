using Microsoft.EntityFrameworkCore.Migrations;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class CategorienDeleteBehaviour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rekeningen_Categorieen_StandaardCategorieId",
                table: "Rekeningen");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Categorieen_CategorieId",
                table: "Transacties");

            migrationBuilder.AddForeignKey(
                name: "FK_Rekeningen_Categorieen_StandaardCategorieId",
                table: "Rekeningen",
                column: "StandaardCategorieId",
                principalTable: "Categorieen",
                principalColumn: "CategorieId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_Categorieen_CategorieId",
                table: "Transacties",
                column: "CategorieId",
                principalTable: "Categorieen",
                principalColumn: "CategorieId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rekeningen_Categorieen_StandaardCategorieId",
                table: "Rekeningen");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Categorieen_CategorieId",
                table: "Transacties");

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
    }
}
