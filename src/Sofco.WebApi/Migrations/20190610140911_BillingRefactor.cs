using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class BillingRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BilledResources",
                schema: "app",
                table: "ManagementReportBillings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                schema: "app",
                table: "ManagementReportBillings",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EvalPropDifference",
                schema: "app",
                table: "ManagementReportBillings",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BilledResources",
                schema: "app",
                table: "ManagementReportBillings");

            migrationBuilder.DropColumn(
                name: "Comments",
                schema: "app",
                table: "ManagementReportBillings");

            migrationBuilder.DropColumn(
                name: "EvalPropDifference",
                schema: "app",
                table: "ManagementReportBillings");
        }
    }
}
