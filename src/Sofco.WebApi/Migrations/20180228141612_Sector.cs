﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class Sector : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                schema: "app",
                table: "Licenses");

            migrationBuilder.AddColumn<int>(
                name: "SectorId",
                schema: "app",
                table: "Licenses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Sectors",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_SectorId",
                schema: "app",
                table: "Licenses",
                column: "SectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Sectors_SectorId",
                schema: "app",
                table: "Licenses",
                column: "SectorId",
                principalSchema: "app",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Sectors_SectorId",
                schema: "app",
                table: "Licenses");

            migrationBuilder.DropTable(
                name: "Sectors",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_SectorId",
                schema: "app",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "SectorId",
                schema: "app",
                table: "Licenses");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                schema: "app",
                table: "Licenses",
                nullable: true);
        }
    }
}
