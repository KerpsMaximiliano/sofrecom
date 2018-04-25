﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class TypeEndReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeEndReasonId",
                schema: "app",
                table: "Employees",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeEndReason",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEndReason", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TypeEndReasonId",
                schema: "app",
                table: "Employees",
                column: "TypeEndReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_EmployeeEndReason_TypeEndReasonId",
                schema: "app",
                table: "Employees",
                column: "TypeEndReasonId",
                principalSchema: "app",
                principalTable: "EmployeeEndReason",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_EmployeeEndReason_TypeEndReasonId",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "EmployeeEndReason",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Employees_TypeEndReasonId",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TypeEndReasonId",
                schema: "app",
                table: "Employees");
        }
    }
}
