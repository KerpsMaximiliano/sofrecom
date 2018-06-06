using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorOc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ammount",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.DropColumn(
                name: "Balance",
                schema: "app",
                table: "PurchaseOrderFiles");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyId",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "PurchaseOrderAmmountDetails",
                schema: "app",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    Ammount = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderAmmountDetails", x => new { x.PurchaseOrderId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAmmountDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAmmountDetails_PurchaseOrderFiles_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "app",
                        principalTable: "PurchaseOrderFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderAmmountDetails_CurrencyId",
                schema: "app",
                table: "PurchaseOrderAmmountDetails",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderAmmountDetails",
                schema: "app");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyId",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ammount",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                schema: "app",
                table: "PurchaseOrderFiles",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
