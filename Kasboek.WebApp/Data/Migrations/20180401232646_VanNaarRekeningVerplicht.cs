using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kasboek.WebApp.Data.Migrations
{
    public partial class VanNaarRekeningVerplicht : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Rekeningen_NaarRekeningId",
                table: "Transacties");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Rekeningen_VanRekeningId",
                table: "Transacties");

            migrationBuilder.AlterColumn<int>(
                name: "VanRekeningId",
                table: "Transacties",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NaarRekeningId",
                table: "Transacties",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_Rekeningen_NaarRekeningId",
                table: "Transacties",
                column: "NaarRekeningId",
                principalTable: "Rekeningen",
                principalColumn: "RekeningId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_Rekeningen_VanRekeningId",
                table: "Transacties",
                column: "VanRekeningId",
                principalTable: "Rekeningen",
                principalColumn: "RekeningId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Rekeningen_NaarRekeningId",
                table: "Transacties");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Rekeningen_VanRekeningId",
                table: "Transacties");

            migrationBuilder.AlterColumn<int>(
                name: "VanRekeningId",
                table: "Transacties",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "NaarRekeningId",
                table: "Transacties",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_Rekeningen_NaarRekeningId",
                table: "Transacties",
                column: "NaarRekeningId",
                principalTable: "Rekeningen",
                principalColumn: "RekeningId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_Rekeningen_VanRekeningId",
                table: "Transacties",
                column: "VanRekeningId",
                principalTable: "Rekeningen",
                principalColumn: "RekeningId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
