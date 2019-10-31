using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AllocationHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                schema: "app",
                table: "Allocations",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "app",
                table: "Allocations",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "app",
                table: "Allocations");
        }
    }
}
