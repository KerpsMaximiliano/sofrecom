using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AmountEditableOnCostDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Provision",
                schema: "app",
                table: "CostDetails",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBilling",
                schema: "app",
                table: "CostDetails",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalProvisioned",
                schema: "app",
                table: "CostDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provision",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropColumn(
                name: "TotalBilling",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropColumn(
                name: "TotalProvisioned",
                schema: "app",
                table: "CostDetails");
        }
    }
}
