using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class CostDetailResourceKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_CostDetailResources_CostDetailId",
            //    schema: "app",
            //    table: "CostDetailResources");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailResources_CostDetailId_EmployeeId_BudgetTypeId",
                schema: "app",
                table: "CostDetailResources",
                columns: new[] { "CostDetailId", "EmployeeId", "BudgetTypeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CostDetailResources_CostDetailId_EmployeeId_BudgetTypeId",
                schema: "app",
                table: "CostDetailResources");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CostDetailResources_CostDetailId",
            //    schema: "app",
            //    table: "CostDetailResources",
            //    column: "CostDetailId");
        }
    }
}
