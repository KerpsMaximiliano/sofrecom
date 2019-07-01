using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ManagementReportDetailClosed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Closed",
                schema: "app",
                table: "ManagementReportBillings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Closed",
                schema: "app",
                table: "CostDetails",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Closed",
                schema: "app",
                table: "ManagementReportBillings");

            migrationBuilder.DropColumn(
                name: "Closed",
                schema: "app",
                table: "CostDetails");
        }
    }
}
