using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class costresourcetypebudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReal",
                schema: "app",
                table: "CostDetailResources");

            migrationBuilder.AddColumn<int>(
                name: "BudgetTypeId",
                schema: "app",
                table: "CostDetailResources",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailResources_BudgetTypeId",
                schema: "app",
                table: "CostDetailResources",
                column: "BudgetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetailResources_BudgetType_BudgetTypeId",
                schema: "app",
                table: "CostDetailResources",
                column: "BudgetTypeId",
                principalSchema: "app",
                principalTable: "BudgetType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetailResources_BudgetType_BudgetTypeId",
                schema: "app",
                table: "CostDetailResources");

            migrationBuilder.DropIndex(
                name: "IX_CostDetailResources_BudgetTypeId",
                schema: "app",
                table: "CostDetailResources");

            migrationBuilder.DropColumn(
                name: "BudgetTypeId",
                schema: "app",
                table: "CostDetailResources");

            migrationBuilder.AddColumn<bool>(
                name: "IsReal",
                schema: "app",
                table: "CostDetailResources",
                nullable: false,
                defaultValue: false);
        }
    }
}
