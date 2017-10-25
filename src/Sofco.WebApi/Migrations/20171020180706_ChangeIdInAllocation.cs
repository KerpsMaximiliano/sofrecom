using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeIdInAllocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Allocations",
                schema: "app",
                table: "Allocations");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "app",
                table: "Allocations",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Allocations",
                schema: "app",
                table: "Allocations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_AnalyticId",
                schema: "app",
                table: "Allocations",
                column: "AnalyticId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Allocations",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropIndex(
                name: "IX_Allocations_AnalyticId",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "app",
                table: "Allocations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Allocations",
                schema: "app",
                table: "Allocations",
                columns: new[] { "AnalyticId", "EmployeeId" });
        }
    }
}
