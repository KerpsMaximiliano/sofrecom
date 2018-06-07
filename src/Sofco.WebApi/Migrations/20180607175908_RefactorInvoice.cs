using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHistories_Invoices_InvoiceId",
                schema: "app",
                table: "InvoiceHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceHistories_Invoices_InvoiceId",
                schema: "app",
                table: "InvoiceHistories",
                column: "InvoiceId",
                principalSchema: "app",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHistories_Invoices_InvoiceId",
                schema: "app",
                table: "InvoiceHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceHistories_Invoices_InvoiceId",
                schema: "app",
                table: "InvoiceHistories",
                column: "InvoiceId",
                principalSchema: "app",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
