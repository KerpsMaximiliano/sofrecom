using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangePurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderFiles_Users_CommercialManagerId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderFiles_Users_ManagerId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "CommercialManagerId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.RenameColumn(
                name: "Year",
                schema: "app",
                table: "PurchaseOrderFiles",
                newName: "CurrencyId");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "app",
                table: "PurchaseOrderFiles",
                newName: "OpportunityId");

            migrationBuilder.AddColumn<decimal>(
                name: "Ammount",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "app",
                table: "PurchaseOrderFiles",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OpportunityDescription",
                schema: "app",
                table: "PurchaseOrderFiles",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderFiles_CurrencyId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderFiles_Currencies_CurrencyId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "CurrencyId",
                principalSchema: "app",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderFiles_Currencies_CurrencyId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderFiles_CurrencyId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "Ammount",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "OpportunityDescription",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.RenameColumn(
                name: "OpportunityId",
                schema: "app",
                table: "PurchaseOrderFiles",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                schema: "app",
                table: "PurchaseOrderFiles",
                newName: "Year");

            migrationBuilder.AddColumn<int>(
                name: "CommercialManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderFiles_Users_CommercialManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "CommercialManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderFiles_Users_ManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "ManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
