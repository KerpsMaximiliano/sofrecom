﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class HitoDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "app",
                table: "Hitos");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                schema: "app",
                table: "Hitos");

            migrationBuilder.CreateTable(
                name: "HitoDetails",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    HitoId = table.Column<int>(nullable: false),
                    Quantity = table.Column<short>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HitoDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HitoDetails_Hitos_HitoId",
                        column: x => x.HitoId,
                        principalSchema: "app",
                        principalTable: "Hitos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HitoDetails_HitoId",
                schema: "app",
                table: "HitoDetails",
                column: "HitoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HitoDetails",
                schema: "app");

            migrationBuilder.AddColumn<short>(
                name: "Quantity",
                schema: "app",
                table: "Hitos",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                schema: "app",
                table: "Hitos",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
