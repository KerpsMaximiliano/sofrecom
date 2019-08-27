using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class hasReal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasReal",
                schema: "app",
                table: "CostDetails",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasReal",
                schema: "app",
                table: "CostDetails");
        }
    }
}
