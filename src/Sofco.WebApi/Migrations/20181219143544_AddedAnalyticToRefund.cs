using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedAnalyticToRefund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_Analytics_AnalyticId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropIndex(
                name: "IX_Advancements_AnalyticId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropColumn(
                name: "Contract",
                schema: "app",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "AnalyticId",
                schema: "app",
                table: "Advancements");

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

            migrationBuilder.AddColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "Advancements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_AnalyticId",
                schema: "app",
                table: "Advancements",
                column: "AnalyticId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_Analytics_AnalyticId",
                schema: "app",
                table: "Advancements",
                column: "AnalyticId",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
