using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeInvoiceFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExcelFileId",
                schema: "app",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PdfFileId",
                schema: "app",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ExcelFileId",
                schema: "app",
                table: "Invoices",
                column: "ExcelFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PdfFileId",
                schema: "app",
                table: "Invoices",
                column: "PdfFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Files_ExcelFileId",
                schema: "app",
                table: "Invoices",
                column: "ExcelFileId",
                principalSchema: "app",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Files_PdfFileId",
                schema: "app",
                table: "Invoices",
                column: "PdfFileId",
                principalSchema: "app",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Files_ExcelFileId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Files_PdfFileId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ExcelFileId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PdfFileId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ExcelFileId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PdfFileId",
                schema: "app",
                table: "Invoices");
        }
    }
}
