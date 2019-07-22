using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorCharges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                schema: "app",
                table: "SocialCharges");

            migrationBuilder.AddColumn<string>(
                name: "ChargesTotal",
                schema: "app",
                table: "SocialCharges",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalaryTotal",
                schema: "app",
                table: "SocialCharges",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "app",
                table: "SocialChargeItems",
                nullable: true,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargesTotal",
                schema: "app",
                table: "SocialCharges");

            migrationBuilder.DropColumn(
                name: "SalaryTotal",
                schema: "app",
                table: "SocialCharges");

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                schema: "app",
                table: "SocialCharges",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                schema: "app",
                table: "SocialChargeItems",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
