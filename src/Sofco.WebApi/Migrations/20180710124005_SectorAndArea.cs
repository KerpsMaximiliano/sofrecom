using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class SectorAndArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "Sectors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "app",
                table: "Sectors",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "app",
                table: "Sectors",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "Areas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "app",
                table: "Areas",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "app",
                table: "Areas",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "app",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "app",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "app",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "app",
                table: "Areas");
        }
    }
}
