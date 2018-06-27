using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AdjustmentAddedToOc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Adjustment",
                schema: "app",
                table: "PurchaseOrderAmmountDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "Adjustment",
                schema: "app",
                table: "PurchaseOrders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adjustment",
                schema: "app",
                table: "PurchaseOrderAmmountDetails");

            migrationBuilder.DropColumn(
                name: "Adjustment",
                schema: "app",
                table: "PurchaseOrders");
        }
    }
}
