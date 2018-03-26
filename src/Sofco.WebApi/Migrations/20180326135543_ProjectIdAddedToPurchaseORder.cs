using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ProjectIdAddedToPurchaseORder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_UserId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHistories_Invoices_InvoiceId",
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
                name: "FK_Solfacs_Users_UserApplicantId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Employees_EmployeeId",
                schema: "app",
                table: "Licenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Users_ManagerId",
                schema: "app",
                table: "Licenses");

            migrationBuilder.AddColumn<string>(
                name: "ProjectId",
                schema: "app",
                table: "PurchaseOrderFiles",
                maxLength: 150,
                nullable: true);

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
                name: "FK_InvoiceHistories_Invoices_InvoiceId",
                schema: "app",
                table: "InvoiceHistories",
                column: "InvoiceId",
                principalSchema: "app",
                principalTable: "Invoices",
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
                name: "FK_Solfacs_Users_UserApplicantId",
                schema: "app",
                table: "Solfacs",
                column: "UserApplicantId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Employees_EmployeeId",
                schema: "app",
                table: "Licenses",
                column: "EmployeeId",
                principalSchema: "app",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Users_ManagerId",
                schema: "app",
                table: "Licenses",
                column: "ManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_UserId",
                schema: "app",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHistories_Invoices_InvoiceId",
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
                name: "FK_Solfacs_Users_UserApplicantId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Employees_EmployeeId",
                schema: "app",
                table: "Licenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Users_ManagerId",
                schema: "app",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Users_UserId",
                schema: "app",
                table: "Invoices",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceHistories_Invoices_InvoiceId",
                schema: "app",
                table: "InvoiceHistories",
                column: "InvoiceId",
                principalSchema: "app",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderFiles_Users_CommercialManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "CommercialManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderFiles_Users_ManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "ManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_Users_UserApplicantId",
                schema: "app",
                table: "Solfacs",
                column: "UserApplicantId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Employees_EmployeeId",
                schema: "app",
                table: "Licenses",
                column: "EmployeeId",
                principalSchema: "app",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Users_ManagerId",
                schema: "app",
                table: "Licenses",
                column: "ManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
