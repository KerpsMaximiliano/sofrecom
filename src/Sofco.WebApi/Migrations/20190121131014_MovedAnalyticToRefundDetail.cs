using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class MovedAnalyticToRefundDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "RefundDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RefundDetails_AnalyticId",
                schema: "app",
                table: "RefundDetails",
                column: "AnalyticId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefundDetails_Analytics_AnalyticId",
                schema: "app",
                table: "RefundDetails",
                column: "AnalyticId",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefundDetails_Analytics_AnalyticId",
                schema: "app",
                table: "RefundDetails");

            migrationBuilder.DropIndex(
                name: "IX_RefundDetails_AnalyticId",
                schema: "app",
                table: "RefundDetails");

            migrationBuilder.DropColumn(
                name: "AnalyticId",
                schema: "app",
                table: "RefundDetails");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Analytics_AnalyticId",
                schema: "app",
                table: "Refunds",
                column: "AnalyticId",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
