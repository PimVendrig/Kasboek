using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class CategorieOmschrijvingUniek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Categorieen_Omschrijving",
                table: "Categorieen",
                column: "Omschrijving",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categorieen_Omschrijving",
                table: "Categorieen");
        }
    }
}
