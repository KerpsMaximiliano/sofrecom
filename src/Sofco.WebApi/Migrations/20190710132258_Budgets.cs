using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class Budgets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_ManagementReports_ManagementReportId",
                schema: "app",
                table: "Budget");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budget",
                schema: "app",
                table: "Budget");

            migrationBuilder.RenameTable(
                name: "Budget",
                schema: "app",
                newName: "Budgets",
                newSchema: "app");

            migrationBuilder.RenameIndex(
                name: "IX_Budget_ManagementReportId",
                schema: "app",
                table: "Budgets",
                newName: "IX_Budgets_ManagementReportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budgets",
                schema: "app",
                table: "Budgets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_ManagementReports_ManagementReportId",
                schema: "app",
                table: "Budgets",
                column: "ManagementReportId",
                principalSchema: "app",
                principalTable: "ManagementReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_ManagementReports_ManagementReportId",
                schema: "app",
                table: "Budgets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budgets",
                schema: "app",
                table: "Budgets");

            migrationBuilder.RenameTable(
                name: "Budgets",
                schema: "app",
                newName: "Budget",
                newSchema: "app");

            migrationBuilder.RenameIndex(
                name: "IX_Budgets_ManagementReportId",
                schema: "app",
                table: "Budget",
                newName: "IX_Budget_ManagementReportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budget",
                schema: "app",
                table: "Budget",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_ManagementReports_ManagementReportId",
                schema: "app",
                table: "Budget",
                column: "ManagementReportId",
                principalSchema: "app",
                principalTable: "ManagementReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
