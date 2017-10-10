using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeSolfacTaxes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "Iva21",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.AddColumn<bool>(
                name: "WithTax",
                schema: "app",
                table: "Solfacs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WithTax",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                schema: "app",
                table: "Solfacs",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Iva21",
                schema: "app",
                table: "Solfacs",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
