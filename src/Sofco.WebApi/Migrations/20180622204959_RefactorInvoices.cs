using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorInvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcelFile",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ExcelFileCreatedDate",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ExcelFileName",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PdfFile",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PdfFileCreatedDate",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PdfFileName",
                schema: "app",
                table: "Invoices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ExcelFile",
                schema: "app",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExcelFileCreatedDate",
                schema: "app",
                table: "Invoices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ExcelFileName",
                schema: "app",
                table: "Invoices",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PdfFile",
                schema: "app",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PdfFileCreatedDate",
                schema: "app",
                table: "Invoices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PdfFileName",
                schema: "app",
                table: "Invoices",
                maxLength: 150,
                nullable: true);
        }
    }
}
