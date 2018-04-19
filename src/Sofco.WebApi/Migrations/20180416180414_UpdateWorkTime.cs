﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UpdateWorkTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceId",
                schema: "app",
                table: "WorkTimes");

            migrationBuilder.AddColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "WorkTimes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimes_AnalyticId",
                schema: "app",
                table: "WorkTimes",
                column: "AnalyticId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTimes_Analytics_AnalyticId",
                schema: "app",
                table: "WorkTimes",
                column: "AnalyticId",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTimes_Analytics_AnalyticId",
                schema: "app",
                table: "WorkTimes");

            migrationBuilder.DropIndex(
                name: "IX_WorkTimes_AnalyticId",
                schema: "app",
                table: "WorkTimes");

            migrationBuilder.DropColumn(
                name: "AnalyticId",
                schema: "app",
                table: "WorkTimes");

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                schema: "app",
                table: "WorkTimes",
                maxLength: 100,
                nullable: true);
        }
    }
}
