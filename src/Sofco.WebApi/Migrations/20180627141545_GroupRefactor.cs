﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class GroupRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Areas_AreaId",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                schema: "app",
                table: "PurchaseOrders",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "app",
                table: "Groups",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Areas_AreaId",
                schema: "app",
                table: "PurchaseOrders",
                column: "AreaId",
                principalSchema: "app",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Areas_AreaId",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                schema: "app",
                table: "PurchaseOrders",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "app",
                table: "Groups",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Areas_AreaId",
                schema: "app",
                table: "PurchaseOrders",
                column: "AreaId",
                principalSchema: "app",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
