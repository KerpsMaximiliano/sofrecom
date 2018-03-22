﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeContractNumberMaxLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_UserId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHistories_Users_UserId",
                schema: "app",
                table: "InvoiceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderFiles_Users_CommercialManagerId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderFiles_Users_ManagerId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_SolfacHistories_Users_UserId",
                schema: "app",
                table: "SolfacHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_LicenseHistories_Users_UserId",
                schema: "app",
                table: "LicenseHistories");

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                schema: "app",
                table: "Analytics",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Users_UserId",
                schema: "app",
                table: "Invoices",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceHistories_Users_UserId",
                schema: "app",
                table: "InvoiceHistories",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderFiles_Users_CommercialManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "CommercialManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderFiles_Users_ManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "ManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_SolfacHistories_Users_UserId",
                schema: "app",
                table: "SolfacHistories",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseHistories_Users_UserId",
                schema: "app",
                table: "LicenseHistories",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_UserId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHistories_Users_UserId",
                schema: "app",
                table: "InvoiceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderFiles_Users_CommercialManagerId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderFiles_Users_ManagerId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_SolfacHistories_Users_UserId",
                schema: "app",
                table: "SolfacHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_LicenseHistories_Users_UserId",
                schema: "app",
                table: "LicenseHistories");

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                schema: "app",
                table: "Analytics",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Users_UserId",
                schema: "app",
                table: "Invoices",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceHistories_Users_UserId",
                schema: "app",
                table: "InvoiceHistories",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_SolfacHistories_Users_UserId",
                schema: "app",
                table: "SolfacHistories",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseHistories_Users_UserId",
                schema: "app",
                table: "LicenseHistories",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
