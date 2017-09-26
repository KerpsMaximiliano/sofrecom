using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class SolfacIncludeInvoiceId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Solfacs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_InvoiceId",
                table: "Solfacs",
                column: "InvoiceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_Invoices_InvoiceId",
                table: "Solfacs",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_Invoices_InvoiceId",
                table: "Solfacs");

            migrationBuilder.DropIndex(
                name: "IX_Solfacs_InvoiceId",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Solfacs");
        }
    }
}
