using Microsoft.EntityFrameworkCore.Migrations;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class InstellingenTransactieMeteenBewerken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TransactieMeteenBewerken",
                table: "Instellingen",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactieMeteenBewerken",
                table: "Instellingen");
        }
    }
}
