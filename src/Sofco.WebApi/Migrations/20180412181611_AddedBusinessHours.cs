﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedBusinessHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessHours",
                schema: "app",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BusinessHoursDescription",
                schema: "app",
                table: "Employees",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimeApprovals_ApprovalUserId",
                schema: "app",
                table: "WorkTimeApprovals",
                column: "ApprovalUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTimeApprovals_Users_ApprovalUserId",
                schema: "app",
                table: "WorkTimeApprovals",
                column: "ApprovalUserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTimeApprovals_Users_ApprovalUserId",
                schema: "app",
                table: "WorkTimeApprovals");

            migrationBuilder.DropIndex(
                name: "IX_WorkTimeApprovals_ApprovalUserId",
                schema: "app",
                table: "WorkTimeApprovals");

            migrationBuilder.DropColumn(
                name: "BusinessHours",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BusinessHoursDescription",
                schema: "app",
                table: "Employees");
        }
    }
}
