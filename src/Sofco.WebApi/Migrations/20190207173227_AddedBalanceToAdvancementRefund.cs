using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedBalanceToAdvancementRefund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                schema: "app",
                table: "Advancements");

            migrationBuilder.AddColumn<bool>(
                name: "UsedInAdvancement",
                schema: "app",
                table: "RefundDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedFromAdvancement",
                schema: "app",
                table: "AdvancementRefunds",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedInAdvancement",
                schema: "app",
                table: "RefundDetails");

            migrationBuilder.DropColumn(
                name: "DiscountedFromAdvancement",
                schema: "app",
                table: "AdvancementRefunds");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                schema: "app",
                table: "Advancements",
                nullable: true);
        }
    }
}
