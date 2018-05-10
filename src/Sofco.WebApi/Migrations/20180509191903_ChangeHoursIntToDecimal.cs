using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeHoursIntToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Hours",
                schema: "app",
                table: "WorkTimes",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                schema: "app",
                table: "LicenseTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTimeApprovals_Analytics_AnalyticId",
                schema: "app",
                table: "WorkTimeApprovals",
                column: "AnalyticId",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTimeApprovals_Analytics_AnalyticId",
                schema: "app",
                table: "WorkTimeApprovals");

            migrationBuilder.DropColumn(
                name: "TaskId",
                schema: "app",
                table: "LicenseTypes");

            migrationBuilder.AlterColumn<int>(
                name: "Hours",
                schema: "app",
                table: "WorkTimes",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
