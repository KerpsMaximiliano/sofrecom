using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class HitoRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Hitos",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Month",
                table: "Hitos",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Hitos");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Hitos");
        }
    }
}
