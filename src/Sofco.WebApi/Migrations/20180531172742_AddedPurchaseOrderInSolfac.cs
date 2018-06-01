﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedPurchaseOrderInSolfac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractNumber",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId",
                schema: "app",
                table: "Solfacs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId1",
                schema: "app",
                table: "Solfacs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_PurchaseOrderId",
                schema: "app",
                table: "Solfacs",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_PurchaseOrderId1",
                schema: "app",
                table: "Solfacs",
                column: "PurchaseOrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_PurchaseOrderFiles_PurchaseOrderId",
                schema: "app",
                table: "Solfacs",
                column: "PurchaseOrderId",
                principalSchema: "app",
                principalTable: "PurchaseOrderFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_PurchaseOrderFiles_PurchaseOrderId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_PurchaseOrderFiles_PurchaseOrderId1",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropIndex(
                name: "IX_Solfacs_PurchaseOrderId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropIndex(
                name: "IX_Solfacs_PurchaseOrderId1",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId1",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                schema: "app",
                table: "Solfacs",
                maxLength: 1000,
                nullable: true);
        }
    }
}
