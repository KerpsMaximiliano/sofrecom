using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedAnalyticToRefund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contract",
                schema: "app",
                table: "Refunds");

            migrationBuilder.AddColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "Refunds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_AnalyticId",
                schema: "app",
                table: "Refunds",
                column: "AnalyticId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Analytics_AnalyticId",
                schema: "app",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_AnalyticId",
                schema: "app",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "AnalyticId",
                schema: "app",
                table: "Refunds");

            migrationBuilder.AddColumn<string>(
                name: "Contract",
                schema: "app",
                table: "Refunds",
                maxLength: 300,
                nullable: true);
        }
    }
}
