﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class WorkTimeApprovalNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTimes_Users_ApprovalUserId",
                schema: "app",
                table: "WorkTimes");

            migrationBuilder.DropIndex(
                name: "IX_WorkTimes_ApprovalUserId",
                schema: "app",
                table: "WorkTimes");

            migrationBuilder.AlterColumn<int>(
                name: "ApprovalUserId",
                schema: "app",
                table: "WorkTimes",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ApprovalUserId",
                schema: "app",
                table: "WorkTimes",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimes_ApprovalUserId",
                schema: "app",
                table: "WorkTimes",
                column: "ApprovalUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTimes_Users_ApprovalUserId",
                schema: "app",
                table: "WorkTimes",
                column: "ApprovalUserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
