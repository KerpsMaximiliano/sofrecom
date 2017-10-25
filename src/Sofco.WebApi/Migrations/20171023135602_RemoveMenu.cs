﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class RemoveMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Menus_MenuId",
                schema: "app",
                table: "Modules");

            migrationBuilder.DropTable(
                name: "Menus",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Modules_MenuId",
                schema: "app",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "MenuId",
                schema: "app",
                table: "Modules");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                schema: "app",
                table: "Modules",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Menus",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 5, nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    Url = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_MenuId",
                schema: "app",
                table: "Modules",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Menus_MenuId",
                schema: "app",
                table: "Modules",
                column: "MenuId",
                principalSchema: "app",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
