using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class RekeningnummerHoofdletters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RekeningNummer",
                table: "Rekeningen",
                newName: "Rekeningnummer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rekeningnummer",
                table: "Rekeningen",
                newName: "RekeningNummer");
        }
    }
}
