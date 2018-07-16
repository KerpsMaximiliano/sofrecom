using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class NewFieldsPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "app",
                table: "PurchaseOrders",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                schema: "app",
                table: "PurchaseOrders",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FicheDeSignature",
                schema: "app",
                table: "PurchaseOrders",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Margin",
                schema: "app",
                table: "PurchaseOrders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentForm",
                schema: "app",
                table: "PurchaseOrders",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "FicheDeSignature",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Margin",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "PaymentForm",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "app",
                table: "PurchaseOrders",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
