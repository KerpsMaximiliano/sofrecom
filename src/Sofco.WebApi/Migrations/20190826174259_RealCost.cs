using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RealCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReal",
                schema: "app",
                table: "CostDetailResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReal",
                schema: "app",
                table: "CostDetailProfiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReal",
                schema: "app",
                table: "CostDetailOthers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReal",
                schema: "app",
                table: "CostDetailResources");

            migrationBuilder.DropColumn(
                name: "IsReal",
                schema: "app",
                table: "CostDetailProfiles");

            migrationBuilder.DropColumn(
                name: "IsReal",
                schema: "app",
                table: "CostDetailOthers");
        }
    }
}
