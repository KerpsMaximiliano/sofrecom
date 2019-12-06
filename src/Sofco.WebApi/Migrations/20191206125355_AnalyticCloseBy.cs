using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AnalyticCloseBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                schema: "app",
                table: "Analytics",
                nullable: true,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                schema: "app",
                table: "Analytics",
                maxLength: 25,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                schema: "app",
                table: "Analytics");
        }
    }
}
