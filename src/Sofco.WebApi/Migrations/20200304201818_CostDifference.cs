using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class CostDifference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostDifference",
                schema: "app",
                table: "PrepaidImportedData",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostDifference",
                schema: "app",
                table: "PrepaidImportedData");
        }
    }
}
