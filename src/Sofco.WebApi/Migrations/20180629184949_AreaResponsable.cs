using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AreaResponsable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponsableUserId",
                schema: "app",
                table: "Areas",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Areas_ResponsableUserId",
                schema: "app",
                table: "Areas",
                column: "ResponsableUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Users_ResponsableUserId",
                schema: "app",
                table: "Areas",
                column: "ResponsableUserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Users_ResponsableUserId",
                schema: "app",
                table: "Areas");

            migrationBuilder.DropIndex(
                name: "IX_Areas_ResponsableUserId",
                schema: "app",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "ResponsableUserId",
                schema: "app",
                table: "Areas");
        }
    }
}
