using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedPropertiesToSolfac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Analytic",
                table: "Solfacs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Solfacs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "Solfacs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Analytic",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Solfacs");
        }
    }
}
