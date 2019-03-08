using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedBalanceToAdvancementRefund2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedInAdvancement",
                schema: "app",
                table: "RefundDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsedInAdvancement",
                schema: "app",
                table: "RefundDetails",
                nullable: false,
                defaultValue: false);
        }
    }
}
