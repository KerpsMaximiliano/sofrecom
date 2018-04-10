﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddEmployeeCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Roles_RoleId",
                schema: "app",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                schema: "app",
                table: "Groups",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeCategories",
                schema: "app",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCategories", x => new { x.CategoryId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_EmployeeCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "app",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeCategories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCategories_EmployeeId",
                schema: "app",
                table: "EmployeeCategories",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Roles_RoleId",
                schema: "app",
                table: "Groups",
                column: "RoleId",
                principalSchema: "app",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Roles_RoleId",
                schema: "app",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "EmployeeCategories",
                schema: "app");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                schema: "app",
                table: "Groups",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Roles_RoleId",
                schema: "app",
                table: "Groups",
                column: "RoleId",
                principalSchema: "app",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
