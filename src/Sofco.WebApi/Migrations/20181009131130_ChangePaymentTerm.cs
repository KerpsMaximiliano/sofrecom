using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangePaymentTerm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_PaymentTerms_PaymentTermId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "PaymentTermId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerm",
                schema: "app",
                table: "Solfacs",
                maxLength: 300,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentTerm",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.AddColumn<int>(
                name: "PaymentTermId",
                schema: "app",
                table: "Solfacs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_PaymentTerms_PaymentTermId",
                schema: "app",
                table: "Solfacs",
                column: "PaymentTermId",
                principalSchema: "app",
                principalTable: "PaymentTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
