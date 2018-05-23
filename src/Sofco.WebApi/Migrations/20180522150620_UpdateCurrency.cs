using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UpdateCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CrmDevelopmentId",
                schema: "app",
                table: "Currencies",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrmProductionId",
                schema: "app",
                table: "Currencies",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrmDevelopmentId",
                schema: "app",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CrmProductionId",
                schema: "app",
                table: "Currencies");
        }
    }
}
