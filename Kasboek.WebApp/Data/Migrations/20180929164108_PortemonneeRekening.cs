using Microsoft.EntityFrameworkCore.Migrations;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class PortemonneeRekening : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PortemonneeRekeningId",
                table: "Instellingen",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instellingen_PortemonneeRekeningId",
                table: "Instellingen",
                column: "PortemonneeRekeningId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instellingen_Rekeningen_PortemonneeRekeningId",
                table: "Instellingen",
                column: "PortemonneeRekeningId",
                principalTable: "Rekeningen",
                principalColumn: "RekeningId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instellingen_Rekeningen_PortemonneeRekeningId",
                table: "Instellingen");

            migrationBuilder.DropIndex(
                name: "IX_Instellingen_PortemonneeRekeningId",
                table: "Instellingen");

            migrationBuilder.DropColumn(
                name: "PortemonneeRekeningId",
                table: "Instellingen");
        }
    }
}
