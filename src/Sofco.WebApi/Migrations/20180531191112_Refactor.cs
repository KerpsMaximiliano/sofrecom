﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class Refactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_PurchaseOrderFiles_PurchaseOrderId1",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropIndex(
                name: "IX_Solfacs_PurchaseOrderId1",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId1",
                schema: "app",
                table: "Solfacs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId1",
                schema: "app",
                table: "Solfacs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_PurchaseOrderId1",
                schema: "app",
                table: "Solfacs",
                column: "PurchaseOrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_PurchaseOrderFiles_PurchaseOrderId1",
                schema: "app",
                table: "Solfacs",
                column: "PurchaseOrderId1",
                principalSchema: "app",
                principalTable: "PurchaseOrderFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
