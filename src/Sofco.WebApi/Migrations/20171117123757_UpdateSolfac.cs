﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UpdateSolfac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Solfacs_SolfacId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Solfacs_SolfacId",
                schema: "app",
                table: "Invoices",
                column: "SolfacId",
                principalSchema: "app",
                principalTable: "Solfacs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Solfacs_SolfacId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Solfacs_SolfacId",
                schema: "app",
                table: "Invoices",
                column: "SolfacId",
                principalSchema: "app",
                principalTable: "Solfacs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
