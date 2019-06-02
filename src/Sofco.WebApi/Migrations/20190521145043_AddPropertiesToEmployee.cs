using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddPropertiesToEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BeneficiariesCount",
                schema: "app",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Prepaid",
                schema: "app",
                table: "Employees",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrepaidAmount",
                schema: "app",
                table: "Employees",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                schema: "app",
                table: "Employees",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeneficiariesCount",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Prepaid",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PrepaidAmount",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Salary",
                schema: "app",
                table: "Employees");
        }
    }
}
