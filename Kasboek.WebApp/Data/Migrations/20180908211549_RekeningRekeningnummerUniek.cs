using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class RekeningRekeningnummerUniek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rekeningen_Rekeningnummer",
                table: "Rekeningen",
                column: "Rekeningnummer",
                unique: true,
                filter: "[Rekeningnummer] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rekeningen_Rekeningnummer",
                table: "Rekeningen");
        }
    }
}
