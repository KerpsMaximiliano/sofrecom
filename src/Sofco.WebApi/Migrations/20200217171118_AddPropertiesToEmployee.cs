using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddPropertiesToEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Activity",
                schema: "app",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ContractType",
                schema: "app",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                schema: "app",
                table: "Employees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activity",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ContractType",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Nationality",
                schema: "app",
                table: "Employees");
        }
    }
}
