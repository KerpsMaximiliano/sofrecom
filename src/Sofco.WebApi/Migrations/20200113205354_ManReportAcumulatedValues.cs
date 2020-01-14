using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ManReportAcumulatedValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AcumulatedCosts",
                schema: "app",
                table: "ManagementReports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcumulatedPeriod",
                schema: "app",
                table: "ManagementReports",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AcumulatedSales",
                schema: "app",
                table: "ManagementReports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcumulatedCosts",
                schema: "app",
                table: "ManagementReports");

            migrationBuilder.DropColumn(
                name: "AcumulatedPeriod",
                schema: "app",
                table: "ManagementReports");

            migrationBuilder.DropColumn(
                name: "AcumulatedSales",
                schema: "app",
                table: "ManagementReports");
        }
    }
}
