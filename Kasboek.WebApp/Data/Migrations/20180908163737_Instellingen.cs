using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class Instellingen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instellingen",
                columns: table => new
                {
                    InstellingenId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StandaardVanRekeningId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instellingen", x => x.InstellingenId);
                    table.ForeignKey(
                        name: "FK_Instellingen_Rekeningen_StandaardVanRekeningId",
                        column: x => x.StandaardVanRekeningId,
                        principalTable: "Rekeningen",
                        principalColumn: "RekeningId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instellingen_StandaardVanRekeningId",
                table: "Instellingen",
                column: "StandaardVanRekeningId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instellingen");
        }
    }
}
