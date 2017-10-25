using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class AddProductToAnalytic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Product",
                schema: "app",
                table: "Analytics",
                newName: "ProductId");

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ProductId",
                schema: "app",
                table: "Analytics",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Products_ProductId",
                schema: "app",
                table: "Analytics",
                column: "ProductId",
                principalSchema: "app",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Products_ProductId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_ProductId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                schema: "app",
                table: "Analytics",
                newName: "Product");
        }
    }
}
