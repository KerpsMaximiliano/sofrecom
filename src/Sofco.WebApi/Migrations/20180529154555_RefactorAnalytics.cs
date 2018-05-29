using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorAnalytics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Currencies_CurrencyId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Users_DirectorId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Products_ProductId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_PurchaseOrders_PurchaseOrderId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "AmountEarned",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "AmountProject",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "BugsAccess",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "ClientProjectTfs",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "EvalProp",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.RenameColumn(
                name: "DirectorId",
                schema: "app",
                table: "Analytics",
                newName: "SectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Sectors_SectorId",
                schema: "app",
                table: "Analytics",
                column: "SectorId",
                principalSchema: "app",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Sectors_SectorId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.RenameColumn(
                name: "SectorId",
                schema: "app",
                table: "Analytics",
                newName: "DirectorId");

            migrationBuilder.AddColumn<string>(
                name: "AmountEarned",
                schema: "app",
                table: "Analytics",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AmountProject",
                schema: "app",
                table: "Analytics",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BugsAccess",
                schema: "app",
                table: "Analytics",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ClientProjectTfs",
                schema: "app",
                table: "Analytics",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "app",
                table: "Analytics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "app",
                table: "Analytics",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EvalProp",
                schema: "app",
                table: "Analytics",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                schema: "app",
                table: "Analytics",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId",
                schema: "app",
                table: "Analytics",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Currencies_CurrencyId",
                schema: "app",
                table: "Analytics",
                column: "CurrencyId",
                principalSchema: "app",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Users_DirectorId",
                schema: "app",
                table: "Analytics",
                column: "DirectorId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Products_ProductId",
                schema: "app",
                table: "Analytics",
                column: "ProductId",
                principalSchema: "app",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_PurchaseOrders_PurchaseOrderId",
                schema: "app",
                table: "Analytics",
                column: "PurchaseOrderId",
                principalSchema: "app",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
