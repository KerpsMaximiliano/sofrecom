using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class statemanagementreportremove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagementReports_BudgetType_StateId",
                schema: "app",
                table: "ManagementReports");

            migrationBuilder.DropIndex(
                name: "IX_ManagementReports_StateId",
                schema: "app",
                table: "ManagementReports");

            migrationBuilder.DropColumn(
                name: "StateId",
                schema: "app",
                table: "ManagementReports");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StateId",
                schema: "app",
                table: "ManagementReports",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagementReports_StateId",
                schema: "app",
                table: "ManagementReports",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagementReports_BudgetType_StateId",
                schema: "app",
                table: "ManagementReports",
                column: "StateId",
                principalSchema: "app",
                principalTable: "BudgetType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
