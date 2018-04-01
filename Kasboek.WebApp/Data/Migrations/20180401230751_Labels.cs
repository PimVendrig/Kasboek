using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class Labels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    LabelId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Omschrijving = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.LabelId);
                });

            migrationBuilder.CreateTable(
                name: "RekeningLabels",
                columns: table => new
                {
                    RekeningId = table.Column<int>(nullable: false),
                    LabelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RekeningLabels", x => new { x.RekeningId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_RekeningLabels_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "LabelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RekeningLabels_Rekeningen_RekeningId",
                        column: x => x.RekeningId,
                        principalTable: "Rekeningen",
                        principalColumn: "RekeningId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactieLabels",
                columns: table => new
                {
                    TransactieId = table.Column<int>(nullable: false),
                    LabelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactieLabels", x => new { x.TransactieId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_TransactieLabels_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "LabelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactieLabels_Transacties_TransactieId",
                        column: x => x.TransactieId,
                        principalTable: "Transacties",
                        principalColumn: "TransactieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RekeningLabels_LabelId",
                table: "RekeningLabels",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactieLabels_LabelId",
                table: "TransactieLabels",
                column: "LabelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RekeningLabels");

            migrationBuilder.DropTable(
                name: "TransactieLabels");

            migrationBuilder.DropTable(
                name: "Labels");
        }
    }
}
