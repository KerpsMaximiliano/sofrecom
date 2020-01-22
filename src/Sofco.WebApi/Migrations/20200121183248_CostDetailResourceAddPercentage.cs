using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class CostDetailResourceAddPercentage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                schema: "app",
                table: "CostDetailResources",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageModified",
                schema: "app",
                table: "CostDetailResources",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                schema: "app",
                table: "CostDetailResources");

            migrationBuilder.DropColumn(
                name: "PercentageModified",
                schema: "app",
                table: "CostDetailResources");
        }
    }
}
