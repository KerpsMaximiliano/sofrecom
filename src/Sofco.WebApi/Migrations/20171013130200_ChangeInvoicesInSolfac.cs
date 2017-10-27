﻿// <auto-generated/>
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeInvoicesInSolfac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_Invoices_InvoiceId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropIndex(
                name: "IX_Solfacs_InvoiceId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.AddColumn<int>(
                name: "SolfacId",
                schema: "app",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SolfacId",
                schema: "app",
                table: "Invoices",
                column: "SolfacId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Solfacs_SolfacId",
                schema: "app",
                table: "Invoices",
                column: "SolfacId",
                principalSchema: "app",
                principalTable: "Solfacs",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Solfacs_SolfacId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_SolfacId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "SolfacId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                schema: "app",
                table: "Solfacs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_InvoiceId",
                schema: "app",
                table: "Solfacs",
                column: "InvoiceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_Invoices_InvoiceId",
                schema: "app",
                table: "Solfacs",
                column: "InvoiceId",
                principalSchema: "app",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction);
        }
    }
}
