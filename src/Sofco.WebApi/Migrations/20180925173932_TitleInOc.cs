using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class TitleInOc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Proposal",
                schema: "app",
                table: "PurchaseOrders",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 600,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "app",
                table: "PurchaseOrders",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Proposal",
                schema: "app",
                table: "Analytics",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.AlterColumn<string>(
                name: "Proposal",
                schema: "app",
                table: "PurchaseOrders",
                maxLength: 600,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Proposal",
                schema: "app",
                table: "Analytics",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
