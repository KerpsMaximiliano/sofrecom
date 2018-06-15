using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RemoveOcFromCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Currencies_CurrencyId",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_CurrencyId",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "app",
                table: "PurchaseOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "app",
                table: "PurchaseOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CurrencyId",
                schema: "app",
                table: "PurchaseOrders",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Currencies_CurrencyId",
                schema: "app",
                table: "PurchaseOrders",
                column: "CurrencyId",
                principalSchema: "app",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
