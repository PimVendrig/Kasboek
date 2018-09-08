using Microsoft.EntityFrameworkCore.Migrations;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class TransactieOmschrijvingIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Omschrijving",
                table: "Transacties",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transacties_Omschrijving",
                table: "Transacties",
                column: "Omschrijving");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transacties_Omschrijving",
                table: "Transacties");

            migrationBuilder.AlterColumn<string>(
                name: "Omschrijving",
                table: "Transacties",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
