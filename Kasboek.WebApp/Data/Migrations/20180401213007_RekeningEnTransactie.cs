using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class RekeningEnTransactie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rekeningen",
                columns: table => new
                {
                    RekeningId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsEigenRekening = table.Column<bool>(nullable: false),
                    Naam = table.Column<string>(maxLength: 100, nullable: false),
                    RekeningNummer = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rekeningen", x => x.RekeningId);
                });

            migrationBuilder.CreateTable(
                name: "Transacties",
                columns: table => new
                {
                    TransactieId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Bedrag = table.Column<decimal>(nullable: false),
                    Datum = table.Column<DateTime>(type: "date", nullable: false),
                    NaarRekeningId = table.Column<int>(nullable: true),
                    Omschrijving = table.Column<string>(maxLength: 1000, nullable: false),
                    VanRekeningId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacties", x => x.TransactieId);
                    table.ForeignKey(
                        name: "FK_Transacties_Rekeningen_NaarRekeningId",
                        column: x => x.NaarRekeningId,
                        principalTable: "Rekeningen",
                        principalColumn: "RekeningId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacties_Rekeningen_VanRekeningId",
                        column: x => x.VanRekeningId,
                        principalTable: "Rekeningen",
                        principalColumn: "RekeningId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacties_NaarRekeningId",
                table: "Transacties",
                column: "NaarRekeningId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacties_VanRekeningId",
                table: "Transacties",
                column: "VanRekeningId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transacties");

            migrationBuilder.DropTable(
                name: "Rekeningen");
        }
    }
}
