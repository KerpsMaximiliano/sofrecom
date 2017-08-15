using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "Modules",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_MenuId",
                table: "Modules",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Menus_MenuId",
                table: "Modules",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
