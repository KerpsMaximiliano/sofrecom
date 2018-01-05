﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddCommercialManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommercialManager",
                schema: "app",
                table: "Analytics");

            migrationBuilder.AddColumn<int>(
                name: "CommercialManagerId",
                schema: "app",
                table: "Analytics",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_CommercialManagerId",
                schema: "app",
                table: "Analytics",
                column: "CommercialManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Analytics_Users_CommercialManagerId",
                schema: "app",
                table: "Analytics",
                column: "CommercialManagerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analytics_Users_CommercialManagerId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "IX_Analytics_CommercialManagerId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "CommercialManagerId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.AddColumn<string>(
                name: "CommercialManager",
                schema: "app",
                table: "Analytics",
                maxLength: 150,
                nullable: true);
        }
    }
}
